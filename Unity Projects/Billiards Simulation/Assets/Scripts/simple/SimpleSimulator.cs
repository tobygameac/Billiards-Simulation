using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleSimulator : MonoBehaviour {

  public Transform[] ballsTransform;
  public Transform[] wallsTransform;
  public Transform[] holesTransform;

  private Vector3[] initialPosition;
  private bool[] initialHolesStatus;

  private SimpleBall[] balls;

  private float up, right, left, down;
  private float wallThickness;
  private float holeThickness;

  public float dragScalePerSecond = 0.6f;

  private bool isDemoResult;
  public float demoEndTime;
  private float demoTime;

  private bool played = false;

  void Start() {
    balls = new SimpleBall[ballsTransform.Length];
    initialPosition = new Vector3[ballsTransform.Length];
    initialHolesStatus = new bool[ballsTransform.Length];

    // Random position
    ballsTransform[0].position = new Vector3(ballsTransform[0].position.x + Random.Range(-3.0f, 3.0f), ballsTransform[0].position.y, ballsTransform[0].position.z + Random.Range(-3.0f, 3.0f));

    for (int i = 0; i < ballsTransform.Length; ++i) {
      balls[i] = ballsTransform[i].GetComponent<SimpleBall>();

      initialPosition[i] = ballsTransform[i].position;
      initialHolesStatus[i] = false;

      float diameter = balls[i].radius * 2;
      ballsTransform[i].localScale = new Vector3(diameter, diameter, diameter);
    }

    wallThickness = wallsTransform[0].localScale.z * 0.5f;
    right = wallsTransform[0].position.x;
    left = wallsTransform[1].position.x;
    up = wallsTransform[2].position.z;
    down = wallsTransform[3].position.z;

    holeThickness = holesTransform[0].localScale.z * 0.5f;
  }

  void Optimize() {
    played = true;
    isDemoResult = false;

    Initialize();

    int bestCount = -1;
    float bestXVelocity = -1;
    float bestYVelocity = -1;
    float bestEndTime = 0;

    for (float xVelocity = 0f; xVelocity <= 90f; xVelocity += 15.0f) {
      for (float yVelocity = 0; yVelocity <= 180f; yVelocity += 30f) {
        if (yVelocity == 120) {
          yVelocity += 30;
        }
        Initialize();

        balls[0].velocity = balls[0].oldVelocity = new Vector3(xVelocity, 0, yVelocity);

        float endTime = Simulate(0.01f);

        int resultCount = CountOfBallsInHoles();
        if (resultCount > bestCount) {
          bestCount = resultCount;
          bestXVelocity = xVelocity;
          bestYVelocity = yVelocity;
          bestEndTime = endTime;
        }
      }
    }

    Debug.Log(bestCount + " Best velocity : " + bestXVelocity + " " + bestYVelocity);

    Initialize();

    balls[0].velocity = balls[0].oldVelocity = new Vector3(bestXVelocity, 0, bestYVelocity);

    demoTime = 0;
    demoEndTime = bestEndTime;
    isDemoResult = true;
  }

  void Update() {
    if (Input.GetMouseButton(0)) {
      if (!played) {
        Optimize();
      }
    }

    if (Input.GetMouseButton(1)) {
      Application.LoadLevel(0);
    }

    if (isDemoResult) {
      demoTime += 0.01f;
      if (demoTime >= demoEndTime) {
        isDemoResult = false;

        for (int i = 0; i < balls.Length; ++i) {
          initialPosition[i] = balls[i].position;
          initialHolesStatus[i] = balls[i].isInHoles;
        }

        Optimize();
        return;
      }

      SimulateUpdate(0.01f);
    }
  }

  int CountOfBallsInHoles() {
    int count = 0;
    for (int i = 0; i < balls.Length; ++i) {
      if (balls[i].isInHoles) {
        if (i == 0) {
          return -1;
        }
        count += (balls[i].isInHoles && !initialHolesStatus[i]) ? 1 : 0;
      }
    }
    return count;
  }

  void Initialize() {

    for (int i = 0; i < balls.Length; ++i) {
      balls[i].isInHoles = initialHolesStatus[i];
      balls[i].velocity = new Vector3(0, 0, 0);
      balls[i].position = initialPosition[i];
    }

    for (int i = 0; i < balls.Length; ++i) {
      balls[i].oldVelocity = balls[i].velocity;
    }
  }

  float Simulate(float deltaTime) {
    for (float time = 0; ; time += deltaTime) {
      SimulateUpdate(deltaTime);
      bool stopped = true;
      for (int i = 0; i < balls.Length && stopped; ++i) {
        if (balls[i].velocity.magnitude > 0.5f) {
          stopped = false;
        }
      }
      if (stopped) {
        return time;
      }
    }
  }

  void Simulate(float endTime, float deltaTime) {
    for (float time = 0; time < endTime; time += deltaTime) {
      SimulateUpdate(deltaTime);
    }
  }

  void SimulateUpdate(float deltaTime) {
    List<int>[] collisionList = new List<int>[ballsTransform.Length];

    for (int i = 0; i < ballsTransform.Length; ++i) {
      balls[i].oldVelocity = balls[i].velocity;

      collisionList[i] = new List<int>();

      if (initialHolesStatus[i]) {
        continue;
      }

      for (int j = 0; j < ballsTransform.Length; ++j) {
        if (i == j || initialHolesStatus[j]) {
          continue;
        }
        if (Vector3.Distance(balls[i].position, balls[j].position) < (balls[i].radius + balls[j].radius)) {
          collisionList[i].Add(j);
        }
      }
    }

    //collision detect
    for (int i = 0; i < collisionList.Length; ++i) {
      foreach (int j in collisionList[i]) {
        Vector3 direction = (balls[i].position - balls[j].position).normalized;
        Vector3 velocityDirection = (balls[i].oldVelocity - balls[j].oldVelocity).normalized;
        if (Vector3.Dot(direction, velocityDirection) > 0) {
          continue;
        }

        float iAmout = Vector3.Dot(balls[i].oldVelocity, direction);
        float jAmout = Vector3.Dot(balls[j].oldVelocity, direction);

        balls[i].velocity += (jAmout - iAmout) * direction;
      }
    }

    for (int i = 0; i < ballsTransform.Length; ++i) {
      // Move the ball
      balls[i].position += balls[i].velocity * deltaTime;

      // Check collision with walls
      if (balls[i].position.x + balls[i].radius + wallThickness >= right) {
        balls[i].velocity = new Vector3(-Mathf.Abs(balls[i].velocity.x), balls[i].velocity.y, balls[i].velocity.z);
      }

      if (balls[i].position.x - balls[i].radius - wallThickness <= left) {
        balls[i].velocity = new Vector3(Mathf.Abs(balls[i].velocity.x), balls[i].velocity.y, balls[i].velocity.z);
      }

      if (balls[i].position.z + balls[i].radius + wallThickness >= up) {
        balls[i].velocity = new Vector3(balls[i].velocity.x, balls[i].velocity.y, -Mathf.Abs(balls[i].velocity.z));
      }

      if (balls[i].position.z - balls[i].radius - wallThickness <= down) {
        balls[i].velocity = new Vector3(balls[i].velocity.x, balls[i].velocity.y, Mathf.Abs(balls[i].velocity.z));
      }

      // Check collision with holes
      for (int h = 0; h < holesTransform.Length; ++h) {
        if (Vector3.Distance(balls[i].position, holesTransform[h].position) < balls[i].radius + holeThickness) {
          balls[i].isInHoles = true;
          balls[i].velocity = Vector3.zero;
          balls[i].position = new Vector3(balls[i].position.x, -10, balls[i].position.z);
        }
      }

      // Rotation
      float xAxis = (balls[i].position.z - balls[i].oldPosition.z) / balls[i].radius;
      float yAxis = (balls[i].position.y - balls[i].oldPosition.y) / balls[i].radius;
      float zAxis = (balls[i].position.x - balls[i].oldPosition.x) / balls[i].radius;
      balls[i].rotation = 180 * new Vector3(xAxis, yAxis, zAxis) / Mathf.PI;

      // Drag velocity down
      float drag = Mathf.Log10(dragScalePerSecond) / (1 / deltaTime);
      drag = Mathf.Pow(10, drag);
      balls[i].velocity *= drag;
    }

    for (int i = 0; i < ballsTransform.Length; ++i) {
      // Set transform position
      ballsTransform[i].position = balls[i].position;
      ballsTransform[i].rotation = Quaternion.Euler(balls[i].rotation);
    }
  }

}

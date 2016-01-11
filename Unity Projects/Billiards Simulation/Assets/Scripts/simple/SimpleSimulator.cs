using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleSimulator : MonoBehaviour {

  public Transform[] ballsTransform;
  public Transform[] wallsTransform;
  public Transform[] holesTransform;

  private Vector3[] initialPosition;

  private SimpleBall[] balls;

  private float up, right, left, down;
  private float wallThickness;
  private float holeThickness;

  public float dragScalePerSecond = 0.8f;

  private bool isDemoResult;
  private float demoTime;

  void Start() {
    balls = new SimpleBall[ballsTransform.Length];
    initialPosition = new Vector3[ballsTransform.Length];

    for (int i = 0; i < ballsTransform.Length; ++i) {
      balls[i] = ballsTransform[i].GetComponent<SimpleBall>();

      initialPosition[i] = ballsTransform[i].position;

      float diameter = balls[i].radius * 2;
      ballsTransform[i].localScale = new Vector3(diameter, diameter, diameter);
    }

    wallThickness = wallsTransform[0].localScale.z * 0.5f;
    right = wallsTransform[0].position.x;
    left = wallsTransform[1].position.x;
    up = wallsTransform[2].position.z;
    down = wallsTransform[3].position.z;

    holeThickness = holesTransform[0].localScale.z * 0.5f;

    isDemoResult = false;

    int bestCount = -1;
    float bestXVelocity = -1;
    float bestYVelocity = -1;

    for (float xVelocity = -15f; xVelocity <= 15f; xVelocity += 1.0f) {
      for (float yVelocity = 5; yVelocity >= -80f; yVelocity -= 15f) {

        Initialize();

        balls[0].velocity = balls[0].oldVelocity = new Vector3(xVelocity, 0, yVelocity);

        Simulate(12, 0.01f);

        int resultCount = CountOfBallsInHoles();
        if (resultCount > bestCount) {
          bestCount = resultCount;
          bestXVelocity = xVelocity;
          bestYVelocity = yVelocity;
        }
      }
    }

    Debug.Log(bestCount + " Best velocity : " + bestXVelocity + " " + bestYVelocity);

    Initialize();

    balls[0].velocity = balls[0].oldVelocity = new Vector3(bestXVelocity, 0, bestYVelocity);

    demoTime = 0;
    isDemoResult = true;
  }

  void Update() {
    if (isDemoResult) {
      demoTime += 0.01f;
      if (demoTime >= 12) {
        isDemoResult = false;
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
        ++count;
      }
    }
    return count;
  }

  void Initialize() {

    for (int i = 0; i < balls.Length; ++i) {
      balls[i].isInHoles = false;
      balls[i].velocity = new Vector3(0, 0, 0);
      balls[i].position = initialPosition[i];
    }

    for (int i = 0; i < balls.Length; ++i) {
      balls[i].oldVelocity = balls[i].velocity;
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

      for (int j = 0; j < ballsTransform.Length; ++j) {
        if (i == j) {
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
      if (balls[i].position.x + balls[i].radius + wallThickness >= right || balls[i].position.x - balls[i].radius - wallThickness <= left) {
        balls[i].velocity = new Vector3(-balls[i].velocity.x, balls[i].velocity.y, balls[i].velocity.z);
      }

      if (balls[i].position.z + balls[i].radius + wallThickness >= up || balls[i].position.z - balls[i].radius - wallThickness <= down) {
        balls[i].velocity = new Vector3(balls[i].velocity.x, balls[i].velocity.y, -balls[i].velocity.z);
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleSimulator : MonoBehaviour
{

    public Transform[] balls;
    public Transform[] walls;
    public Transform[] holes;
    private float up, right, left, down;
    private float thickness;
    private SimpleBall[] simpleBalls;

    void Start()
    {
        simpleBalls = new SimpleBall[balls.Length];
        for (int i = 0; i < balls.Length; ++i)
        {
            simpleBalls[i] = balls[i].GetComponent<SimpleBall>();
            simpleBalls[i].oldV = simpleBalls[i].v;
            simpleBalls[i].x = balls[i].position;

            float r = simpleBalls[i].radius * 2;
            balls[i].localScale = new Vector3(r, r, r);
        }

        thickness = walls[0].localScale.z * 0.5f;
        right = walls[0].position.x;
        left = walls[1].position.x;
        up = walls[2].position.z;
        down = walls[3].position.z;

        //Simulate(15, 0.001f);
    }

    void Simulate(float endTime, float deltaTime)
    {
        for (float time = 0; time < endTime; time += deltaTime)
        {
            SimulateUpdate(deltaTime);
        }
    }

    void SimulateUpdate(float deltaTime)
    {
        List<int>[] collisionList = new List<int>[balls.Length];

        for (int i = 0; i < balls.Length; ++i)
        {
            simpleBalls[i].oldV = simpleBalls[i].v;

            collisionList[i] = new List<int>();

            for (int j = 0; j < balls.Length; ++j)
            {
                if (i == j)
                    continue;
                if (Vector3.Distance(simpleBalls[i].x, simpleBalls[j].x) < (simpleBalls[i].radius + simpleBalls[j].radius))
                {
                    collisionList[i].Add(j);
                }
            }
        }


        for (int i = 0; i < collisionList.Length; ++i) //collision detect
        {
            foreach (int j in collisionList[i])
            {
                Vector3 direction = (simpleBalls[i].x - simpleBalls[j].x).normalized;
                Vector3 vd = (simpleBalls[i].oldV - simpleBalls[j].oldV).normalized;
                //direction /= (simpleBalls[i].radius + simpleBalls[j].radius);
                if (Vector3.Dot(direction, vd) > 0) continue;

                Debug.Log(i + " : " + direction.z + "(" + simpleBalls[i].x.z + " - " + simpleBalls[j].x.z + ")");

                float iAmout = Vector3.Dot(simpleBalls[i].oldV, direction);
                float jAmout = Vector3.Dot(simpleBalls[j].oldV, direction);

                simpleBalls[i].v += (jAmout - iAmout) * direction;
                //simpleBalls[j].v += (iAmout - jAmout) * direction;

                //float ball1_x = balls[i].transform.position.x;
                //float ball1_y = balls[i].transform.position.z;
                //float ball2_x = balls[j].transform.position.x;
                //float ball2_y = balls[j].transform.position.z;
                //
                //float theta = Mathf.Atan2(ball2_y - ball1_y, ball2_x - ball1_x);
                //
                //float ball2_vny = simpleBalls[j].oldV.z * Mathf.Cos(Mathf.PI / 2 - theta) + simpleBalls[j].oldV.x * Mathf.Cos(Mathf.PI - theta) + simpleBalls[i].oldV.z * Mathf.Cos(Mathf.PI / 2 - theta) + simpleBalls[i].oldV.x * Mathf.Cos(Mathf.PI - theta);
                //float ball2_vnx = simpleBalls[j].oldV.z * Mathf.Cos(theta) + simpleBalls[j].oldV.x * Mathf.Cos(Mathf.PI / 2 - theta) + simpleBalls[i].oldV.z * Mathf.Cos(theta) + simpleBalls[i].oldV.x * Mathf.Cos(Mathf.PI / 2 - theta);
                //
                //float ball1_vny = -ball2_vny;
                //float ball1_vnx = -ball2_vnx;

                //float ball2_new_vy = ball2_vny * Mathf.Cos(Mathf.PI / 2 - theta) + ball2_vnx * Mathf.Cos(theta);
                //float ball2_new_vx = ball2_vny * Mathf.Cos(Mathf.PI - theta) + ball2_vnx * Mathf.Cos(Mathf.PI / 2 - theta);

                //float ball1_new_vy = ball1_vny * Mathf.Cos(Mathf.PI / 2 - theta) + ball1_vnx * Mathf.Cos(theta);
                //float ball1_new_vx = ball1_vny * Mathf.Cos(Mathf.PI - theta) + ball1_vnx * Mathf.Cos(Mathf.PI / 2 - theta);

                //simpleBalls[j].v -= new Vector3(ball2_new_vx, 0, ball2_new_vy);
                //simpleBalls[i].v += new Vector3(ball1_new_vx, 0, ball1_new_vy);
                //if (i == 0 || j == 0)
                //Debug.Log("bump : " + i + " " + j + "(" + simpleBalls[i].v + ") (" + simpleBalls[j].v + ")");
            }
        }

        for (int i = 0; i < balls.Length; ++i)
        {
            //v++
            simpleBalls[i].x += simpleBalls[i].v * deltaTime;

            // walls
            if (simpleBalls[i].x.x + simpleBalls[i].radius + thickness >= right || simpleBalls[i].x.x - simpleBalls[i].radius - thickness <= left)
            {
                simpleBalls[i].v = new Vector3(-simpleBalls[i].v.x, simpleBalls[i].v.y, simpleBalls[i].v.z);
            }

            if (simpleBalls[i].x.z + simpleBalls[i].radius + thickness >= up || simpleBalls[i].x.z - simpleBalls[i].radius - thickness <= down)
            {
                simpleBalls[i].v = new Vector3(simpleBalls[i].v.x, simpleBalls[i].v.y, -simpleBalls[i].v.z);
            }

            //holes
            for (int h = 0; h < holes.Length; ++h)
            {
                if (Vector3.Distance(simpleBalls[i].x, holes[h].position) < simpleBalls[i].radius + holes[h].transform.localScale.x * 0.5f)
                {
                    simpleBalls[i].score = true;
                    simpleBalls[i].v = Vector3.zero;
                    simpleBalls[i].x = new Vector3(simpleBalls[i].x.x, 10, simpleBalls[i].x.z);
                }
            }

            //v--
            float drag = Mathf.Log10(0.8f) / (1 / deltaTime);
            drag = Mathf.Pow(10, drag);
            simpleBalls[i].v *= drag;
        }

        for (int i = 0; i < balls.Length; ++i)
        {
            //pos
            balls[i].position = simpleBalls[i].x;
        }
    }

    void Update()
    {
        SimulateUpdate(0.01f);
    }
}

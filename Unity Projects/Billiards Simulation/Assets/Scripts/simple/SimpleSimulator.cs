using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleSimulator : MonoBehaviour
{

    public Transform[] balls;
    private SimpleBall[] simpleBalls;

    void Start()
    {
        simpleBalls = new SimpleBall[balls.Length];
        for (int i = 0; i < balls.Length; ++i)
        {
            simpleBalls[i] = balls[i].GetComponent<SimpleBall>();
            simpleBalls[i].oldV = simpleBalls[i].v;

            float r = simpleBalls[i].radius * 2;
            balls[i].transform.localScale = new Vector3(r, r, r);
        }
    }

    void Update()
    {
        List<int>[] collisionList = new List<int>[balls.Length];

        for (int i = 0; i < balls.Length; ++i)
        {
            simpleBalls[i].oldV = simpleBalls[i].v;

            collisionList[i] = new List<int>();

            for (int j = 0; j < balls.Length; ++j)
            {
                if (i == j) continue;
                if (Vector3.Distance(balls[i].transform.position, balls[j].transform.position) < (simpleBalls[i].radius + simpleBalls[j].radius))
                {
                    collisionList[i].Add(j);
                }
            }
        }
        
        
        for (int i = 0; i < collisionList.Length; ++i) //collision detect
        {
            foreach(int j in collisionList[i])
            {
                Vector3 direction = Vector3.Normalize(balls[i].transform.position - balls[j].transform.position);

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

        Debug.Log("0 : " + simpleBalls[0].v.z);
        Debug.Log("1 : " + simpleBalls[1].v);
        Debug.Log("2 : " + simpleBalls[2].v);

        float t = Time.deltaTime;
        for (int i = 0; i < balls.Length; ++i)
        {
            balls[i].transform.position += simpleBalls[i].v * t;
            //simpleBalls[i].v *= 0.995f;
        }
    }
}

using UnityEngine;
using System.Collections;

public class SimpleSimulator : MonoBehaviour {

  public Transform[] balls;

	void Start () {
	
	}
	
	void Update () {
    for (int i = 0; i < balls.Length; ++i) {
      balls[i].transform.position = new Vector3(i, 0, i);
    }
	}
}

﻿using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

  public GameObject gamePrefab;

  void Start() {
    float forceGap = 100.0f;
    int forceCount = (int)(400 / forceGap);
    float angleGap = 0.25f;
    int angleCount = (int)(10 / angleGap);

    int totalObjectCount = 0;
    for (int i = 0; i < forceCount; ++i) {
      for (int j = 0; j < angleCount; ++j) {
        GameObject game = Instantiate(gamePrefab, new Vector3((i - forceCount / 2) * 25, 0, (j - angleCount / 2) * 45), Quaternion.identity) as GameObject;
        game.GetComponent<Game>().Hit(i * forceGap, j * angleGap);

        ++totalObjectCount;
      }
    }
  }

  //void Update() {

  //  if (Input.GetMouseButton(1)) {
  //    Application.LoadLevel(0);
  //  }
  //}

}

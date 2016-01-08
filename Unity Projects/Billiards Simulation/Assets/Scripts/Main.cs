using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

  public GameObject gamePrefab;

  void Start() {
    float forceGap = 50.0f;
    int forceCount = (int)(500 / forceGap);
    float angleGap = 10.0f;
    int angleCount = (int)(360 / angleGap);

    int gridSize = (int)Mathf.Sqrt(angleCount);
    for (int i = 0; i < forceCount; ++i) {
      for (int j = 0; j < angleCount; ++j) {
        GameObject game = Instantiate(gamePrefab, new Vector3((i - forceCount / 2) * 25, 0, (j - angleCount / 2) * 45), Quaternion.identity) as GameObject;
        game.GetComponent<Game>().Hit(i * forceGap, j * angleGap);
      }
    }
  }

  //void Update() {

  //  if (Input.GetMouseButton(1)) {
  //    Application.LoadLevel(0);
  //  }
  //}

}

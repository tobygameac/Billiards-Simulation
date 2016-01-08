using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

  void Update() {

    if (Input.GetMouseButton(1)) {
      Application.LoadLevel(0);
    }
  }

}

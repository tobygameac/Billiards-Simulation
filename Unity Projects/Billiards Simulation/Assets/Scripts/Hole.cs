using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Hole : MonoBehaviour {


  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag == "Ball") {
      Destroy(collider.gameObject);
    }
  }
}

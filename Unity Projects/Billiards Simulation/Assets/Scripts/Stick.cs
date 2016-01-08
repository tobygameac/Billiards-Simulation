using UnityEngine;
using System.Collections;

public class Stick : MonoBehaviour {

  public float force = 10.0f;
  public float forceOffset = 1000.0f;

  private float stickRootY = 1.0f;

  public GameObject targetBall;
  public float stickOffsetScale;

  private Rigidbody targetBallRigidbody;

  void Start() {
    targetBallRigidbody = targetBall.GetComponent<Rigidbody>();
  }

  void Update() {

    if (Input.GetMouseButton(1)) {
      Application.LoadLevel(0);
    }

    //stickRootY += Input.GetAxis("Mouse ScrollWheel") * 0.5f;
    force += Input.GetAxis("Mouse ScrollWheel") * forceOffset;
    force = (force < 0) ? 0 : force;

    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit raycastHit;

    Physics.Raycast(ray, out raycastHit, 100);

    Vector3 stickRoot = new Vector3(raycastHit.point.x, stickRootY, raycastHit.point.z);

    Vector3 stickDirection = (targetBall.transform.position - stickRoot).normalized;

    transform.position = targetBall.transform.position - stickDirection * stickOffsetScale;
    transform.rotation = Quaternion.LookRotation(stickDirection);

    if (Input.GetMouseButton(0)) {
      targetBallRigidbody.AddForce(transform.forward * force);
    }

  }

}

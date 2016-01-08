using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

  public GameObject targetBall;
  private Rigidbody targetBallRigidbody;

  public GameObject informationTextGameObject;
  private Text informationText;

  private Vector3 stickZeroDirection = new Vector3(0, 0, -1);

  void Awake() {
    targetBallRigidbody = targetBall.GetComponent<Rigidbody>();
    informationText = informationTextGameObject.GetComponent<Text>();
  }

  public void Hit(float force, float angle) {
    informationText.text = "Force : " + force + "\n";
    informationText.text += "Angle : " + angle;
    Vector3 hitDirection = Quaternion.Euler(0, angle, 0) * stickZeroDirection;
    targetBallRigidbody.AddForce(hitDirection * force);
  }

}

using UnityEngine;
using UnityEngine.UI;

public class InformationDisplayer : MonoBehaviour {

  public GameObject stickGameObject;
  private Stick stick;

  public GameObject stickTextGameObject;
  private Text stickText;

  void Start() {
    stick = stickGameObject.GetComponent<Stick>();
    stickText = stickTextGameObject.GetComponent<Text>();
  }

  void Update() {
    stickText.text = "Force : " + stick.force + "\n";
    stickText.text += "Angle : " + stick.stickAngle;
  }
}

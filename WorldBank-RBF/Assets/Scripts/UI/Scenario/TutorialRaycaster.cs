using UnityEngine;

public class TutorialRaycaster : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// If mouse is in rect (mask), allow click behind GUI
		bool mouseInRect = RectTransformUtility.RectangleContainsScreenPoint(gameObject.GetComponent<RectTransform>(), Input.mousePosition, null);

		transform.parent.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = !mouseInRect;
	}
}

using UnityEngine;

public class TutorialRaycaster : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bool mouseInRect = RectTransformUtility.RectangleContainsScreenPoint(gameObject.GetComponent<RectTransform>(), Input.mousePosition, null);

		transform.parent.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = !mouseInRect;
	}
}

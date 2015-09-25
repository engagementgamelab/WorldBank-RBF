using UnityEngine;

public class ScrollbarFixer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// WHY, UNITY???!!?
		gameObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;
		gameObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
	}
	
}

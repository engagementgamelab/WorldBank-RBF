using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class ScrollbarEnabler2 : MonoBehaviour {

	public GameObject scrollbar;
	public RectTransform container;

	RectTransform myTransform = null;
	RectTransform MyTransform {
		get {
			if (myTransform == null) {
				myTransform = GetComponent<RectTransform> ();
			}
			return myTransform;
		}
	}

	float maxHeight;
	float MaxHeight {
		get { return container.sizeDelta.y; }
	}

	float MyHeight {
		get { return MyTransform.sizeDelta.y; }
	}

	bool enableScrollbar = false;

	void Update () {
		if (scrollbar == null) return;
		if (enableScrollbar != scrollbar.activeSelf)
			scrollbar.SetActive (enableScrollbar);
	}

	void OnRectTransformDimensionsChange () {
		if (container == null) return;
		enableScrollbar = MyHeight > MaxHeight;
	}
}

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

	ScrollRect scrollRect = null;
	ScrollRect ScrollRect {
		get {
			if (scrollRect == null) {
				scrollRect = container.GetComponent<ScrollRect> ();
			}
			return scrollRect;
		}
	}

	Scrollbar _scrollbar = null;
	Scrollbar Scrollbar {
		get {
			if (_scrollbar == null) {
				_scrollbar = scrollbar.GetComponent<Scrollbar> ();
			}
			return _scrollbar;
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
	bool toggleHorizontal;
	bool toggleVertical;

	void Awake () {
		toggleHorizontal = ScrollRect.horizontal == true;
		toggleVertical = ScrollRect.vertical == true;
		SetScrollEnabled ();
	}

	void Update () {
		if (scrollbar == null) return;
		if (enableScrollbar != scrollbar.activeSelf)
			SetScrollEnabled ();
	}

	void OnRectTransformDimensionsChange () {
		if (container == null) return;
		enableScrollbar = MyHeight > MaxHeight;
	}

	void SetScrollEnabled () {
		if (!enableScrollbar) Scrollbar.value = 1;
		scrollbar.SetActive (enableScrollbar);
		ScrollRect.horizontal = toggleHorizontal && enableScrollbar;
		ScrollRect.vertical = toggleVertical && enableScrollbar;
	}
}

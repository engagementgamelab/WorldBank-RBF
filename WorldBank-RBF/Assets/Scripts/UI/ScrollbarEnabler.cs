using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class ScrollbarEnabler : MonoBehaviour {

	/*RectTransform container = null;
	RectTransform Container {
		get {
			if (container == null) {
				container = GetComponent<RectTransform> ();
			}
			return container;
		}
	}

	RectTransform content;
	RectTransform Content {
		get {
			if (content == null) {
				ScrollRect scroll = GetComponent<ScrollRect> ();
				content = scroll.content;
			}
			return content;
		}
	}

	RectOffset contentPadding;
	RectOffset ContentPadding {
		get {
			if (contentPadding == null) {
				contentPadding =Content.GetComponent<VerticalLayoutGroup> ().padding;
			}
			return contentPadding;
		}
	}

	Scrollbar scrollBar;
	Scrollbar Scrollbar {
		get {
			if (scrollBar == null) {
				ScrollRect scroll = GetComponent<ScrollRect> ();
				scrollBar = scroll.verticalScrollbar;
			}
			return scrollBar;
		}
	}

	RectTransform scrollbarTransform;
	RectTransform ScrollbarTransform {
		get {
			if (scrollbarTransform == null) {
				scrollbarTransform = (RectTransform)Scrollbar.transform;
			}
			return scrollbarTransform;
		}
	}*/

	/*RectOffset contentPadding;
	RectOffset ContentPadding {
		get {
			if (contentPadding == null) {
				contentPadding = content.GetComponent<VerticalLayoutGroup> ().padding;
			}
			return contentPadding;
		}
	}

	RectTransform scrollbarTransform;
	RectTransform ScrollbarTransform {
		get {
			if (scrollbarTransform == null) {
				scrollbarTransform = (RectTransform)scrollbar.transform;
			}
			return scrollbarTransform;
		}
	}*/

	[SerializeField] RectTransform container;
	[SerializeField] RectTransform content;
	[SerializeField] Scrollbar scrollbar;

	public int leftPadding = 10;
	bool enableScrollbar = false;

	/*void Update () {
		if (enableScrollbar != scrollbar.gameObject.activeSelf)
			scrollbar.gameObject.SetActive (enableScrollbar);
	}

	void OnRectTransformDimensionsChange () {
		enableScrollbar = container.rect.height < content.rect.height;
	}*/

	void Update () {
		if (enableScrollbar != scrollbar.gameObject.activeSelf)
			SetScrollbarActive (enableScrollbar);
	}

	void OnRectTransformDimensionsChange () {
		enableScrollbar = container.rect.height < content.rect.height;
		Debug.Log (enableScrollbar);
	}

	void SetScrollbarActive (bool active) {
		scrollbar.gameObject.SetActive (active);
		/*if (active) {
			ContentPadding.left = leftPadding + (int)ScrollbarTransform.rect.width;
		} else {
			ContentPadding.left = leftPadding;
		}*/
		// Debug.Log (ContentPadding.left);
	}
}

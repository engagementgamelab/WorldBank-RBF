using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ContainerContentFitter : MonoBehaviour {

	public RectTransform contentArea;
	public float maxHeight;
	
	LayoutElement layout = null;
	LayoutElement Layout {
		get {
			if (layout == null) {
				layout = GetComponent<LayoutElement> ();
			}
			return layout;
		}
	}

	void Update () {
		#if UNITY_EDITOR
		if (contentArea == null) return;
		#endif
		Layout.minHeight = Mathf.Min (maxHeight, contentArea.sizeDelta.y);
	}
}

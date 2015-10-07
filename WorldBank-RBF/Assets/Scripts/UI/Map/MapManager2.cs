using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapManager2 : NotebookCanvas {

	Canvas canvas = null;
	Canvas Canvas {
		get {
			if (canvas == null) {
				canvas = GetComponent<Canvas> ();
			}
			return canvas;
		}
	}

	public RectTransform scrollView;
	public RectTransform background;
	public RectTransform routes;
	public RectTransform cities;

	bool setScale = false;

	void OnEnable () {
		if (!setScale) {
			float aspectRatio = MainCamera.Instance.Aspect;
			if (Mathf.Approximately (aspectRatio, 2f)) {
				SetScale (1.5f);
			} else if (aspectRatio > 1.77f) {
				SetScale (1.33f);
			} else if (aspectRatio > 1.59f) {
				SetScale (1.2f);
			} else if (aspectRatio > 1.49f) {
				SetScale (1.12f);
			} else if (aspectRatio > 1.32f) {
				SetScale (1f);
			} else if (aspectRatio > 1.24f) {
				SetScale (1f);
			} else {
				SetScale (1f);
			}
			setScale = true;
		}
	}

	void SetScale (float scale) {
		scrollView.SetLocalScaleY (scale);
		routes.localScale = new Vector3 (scale, 1, 1);
		cities.localScale = new Vector3 (scale, 1, 1);
	}

	public override void Open () {
		Canvas.enabled = true;
	}

	public override void Close () {
		Canvas.enabled = false;
	}
}

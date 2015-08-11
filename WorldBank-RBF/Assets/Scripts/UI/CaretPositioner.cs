using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CaretPositioner : MB, ISelectHandler {

	RectTransform caret;
	RectTransform Caret {
		get {
			if (caret == null) {
				RectTransform child = (RectTransform)Transform.GetChild (0);
				if (child.name.Contains ("Caret"))
					caret = child;
			}
			return caret;
		}
	}

	public void OnSelect (BaseEventData eventData) {
		if (Caret != null)
			Caret.offsetMax = new Vector2 (Caret.offsetMax.x, 4);
	}
}

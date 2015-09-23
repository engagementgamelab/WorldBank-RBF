using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CaretPositioner : MB, ISelectHandler {

	RectTransform caret;
	RectTransform Caret {
		get {
			if (caret == null) {
				foreach (RectTransform child in Transform) {
					if (child.name.Contains ("Caret"))
						caret = child;
				}
			}
			return caret;
		}
	}

	public void OnSelect (BaseEventData eventData) {
		StartCoroutine (CoSetCaretPosition ());
	}

	IEnumerator CoSetCaretPosition () {
		while (gameObject.activeSelf && Caret == null)
			yield return null;
		Caret.offsetMax = new Vector2 (Caret.offsetMax.x, 4);
	}
}

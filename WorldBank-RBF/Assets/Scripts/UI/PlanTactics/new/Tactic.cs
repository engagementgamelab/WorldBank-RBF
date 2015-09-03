using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Tactic : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	Transform planPanel;
	Transform PlanPanel {
		get {
			if (planPanel == null) {
				planPanel = GameObject.Find ("PlanPanel").transform;
			}
			return planPanel;
		}
	}

	CanvasGroup canvasGroup = null;
	CanvasGroup CanvasGroup {
		get {
			if (canvasGroup == null) {
				canvasGroup = GetComponent<CanvasGroup> ();
			}
			return canvasGroup;
		}
	}

	Vector2 dragPosition;

	#region IBeginDragHandler, IDragHandler, IEndDragHandler implementation
	public void OnBeginDrag (PointerEventData eventData) {
		dragPosition = transform.position - Input.mousePosition;
		CanvasGroup.blocksRaycasts = false;
		transform.SetParent (PlanPanel);
	}

	public void OnDrag (PointerEventData eventData) {
		transform.position = dragPosition + (Vector2)Input.mousePosition;
	}

	public void OnEndDrag (PointerEventData eventData) {
		CanvasGroup.blocksRaycasts = true;
	}
	#endregion
}

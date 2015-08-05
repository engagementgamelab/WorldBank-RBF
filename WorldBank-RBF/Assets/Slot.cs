using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {
	public GameObject item {
		get {
			if (transform.childCount>0) {
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if (!item) {
			UITactic.itemBeingDragged.transform.SetParent (transform);
			return;
		} else {

		if (item) {
			UITactic.itemBeingDragged.transform.SetParent (transform);

			item.transform.SetParent (UITactic.startParent);

			UITactic tactic = UITactic.itemBeingDragged.GetComponent<UITactic>();
			UITacticSlot slot = transform.GetComponent<UITacticSlot>();
			
			// Tell tactic it has moved
			tactic.MoveToSlot(slot);
				
		}
	}
	#endregion
	}
}
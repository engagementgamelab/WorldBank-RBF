using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UITacticSlot : TacticButton, IDropHandler {

	int siblingIndex;
	public int SiblingIndex {
		get { return siblingIndex;}
	}

	public GameObject item {
		get {
			if (transform.childCount>0) {
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}

	public override void Init (Column column, Transform contentParent, string content) {
		base.Init (column, contentParent, content);
		siblingIndex = Transform.GetSiblingIndex ();
	}
	
	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if (!item) {
			TacticItem tacticItem = UITactic.itemBeingDragged.GetComponent<UITactic>().Tactic;

			Events.instance.Raise (new TacticSlotEvent (tacticItem, SiblingIndex));
			
			UITactic.itemBeingDragged.transform.SetParent (transform);
		}
	}
	#endregion
}

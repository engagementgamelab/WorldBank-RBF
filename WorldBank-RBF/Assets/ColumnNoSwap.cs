using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ColumnNoSwap : MonoBehaviour, IDropHandler {
	
	public GameObject targetMenu;
	
	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		// HACK
		if(UITactic.itemBeingDragged == null)
			return;
			
		UITactic.itemBeingDragged.transform.SetParent (targetMenu.transform);

		TacticItem tacticItem = UITactic.itemBeingDragged.GetComponent<UITactic>().Tactic;
		
		// Broadcast to say removed from plan
		Events.instance.Raise (new TacticSlotEvent (tacticItem));

	}
	#endregion

}
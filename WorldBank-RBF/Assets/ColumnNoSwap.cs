using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ColumnNoSwap : MonoBehaviour, IDropHandler {
	public GameObject targetMenu ;
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
		
//		if (!item) {
			ThisDragHandler.itemBeingDragged.transform.SetParent (targetMenu.transform);
//			return;
//		} else {
//			
////			if (item) {
//				ThisDragHandler.itemBeingDragged.transform.SetParent (transform);
//				//			item.transform.SetParent (DragHandler.startParent);
//				
//				ThisDragHandler.itemBeingDragged.transform.SetParent (ThisDragHandler.startParent);
//				
//				//			print (DragHandler.startParent);
//			}
//		}
		#endregion
	}
}
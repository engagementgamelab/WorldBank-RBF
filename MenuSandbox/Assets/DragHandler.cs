using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public static GameObject itemBeingDragged;
	public static Vector3 startPosition;
	public static Transform startParent;

	public void OnBeginDrag (PointerEventData eventData)
	{
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;

		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	
	public void OnDrag (PointerEventData eventData)
	{
		transform.position = Input.mousePosition;
		//can use pointer event data.
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		itemBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	
		if(transform.parent != startParent) {
			transform.position = startPosition;
			return;
			}

		if(transform.parent = startParent) {			
			transform.position = startPosition;
		}
	}
}
//
//
//using UnityEngine;
//using System.Collections;
//using UnityEngine.EventSystems;
//
//public class DragHandler : MonoBehaviour, IBeginDragHandler,IDragHandler, IEndDragHandler {
//	public static GameObject itemBeingDragged;
//	Vector3 startPosition;
//	Transform startParent;
//	
//	#region IBeginDragHandler implementation
//	
//	public void OnBeginDrag (PointerEventData eventData)
//	{
//		itemBeingDragged = gameObject;
//		startPosition = transform.position;
//		startParent = transform.parent;
//		GetComponent<CanvasGroup>().blocksRaycasts = false;
//	}
//	
//	#endregion
//	
//	#region IDragHandler implementation
//	
//	public void OnDrag (PointerEventData eventData)
//	{
//		transform.position = Input.mousePosition;
//		//can use pointer event data.
//	}
//	
//	#endregion
//	
//	#region IEndDragHandler implementation
//	
//	public void OnEndDrag (PointerEventData eventData)
//	{
//		itemBeingDragged = null;
//		GetComponent<CanvasGroup> ().blocksRaycasts = true;
//		
//		if(transform.parent != startParent) {
//			transform.position = startPosition;
//			return;
//		}
//		
//		if(transform.parent = startParent) {			
//			transform.position = startPosition;
//		}
//		
//		#endregion
//		
//	}
//	
//}

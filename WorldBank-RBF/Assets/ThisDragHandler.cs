using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ThisDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public static GameObject itemBeingDragged;
	public static Vector3 startPosition;
	public static Transform startParent;

	public GameObject disableText;
	public GameObject disablePic;
	public GameObject disableQuote;

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
	}

	public void OnEndDrag (PointerEventData eventData)
	{

		itemBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;

		if(transform.parent != startParent) {
			transform.position = startPosition;

			disableText.gameObject.SetActive (false);
			disablePic.gameObject.SetActive (false);
			disableQuote.gameObject.SetActive (false);

			if (transform.parent.gameObject.tag == "Player"){
			disableText.gameObject.SetActive (true);
			disablePic.gameObject.SetActive (true);
			disableQuote.gameObject.SetActive (true);
			}

			return;
			}

		if(transform.parent = startParent) {			
			transform.position = startPosition;


		}
	}
}
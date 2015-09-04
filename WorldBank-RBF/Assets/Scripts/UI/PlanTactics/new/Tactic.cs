using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Tactic : MB, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler {

	public static Tactic selected = null;

	public PortraitTextBox portrait;
	public Text titleText;
	public Text descriptionText;
	public Text contextText;

	Vector2 dragPosition;
	bool dragging = false;
	
	TacticItem item;
	public TacticItem Item {
		get { return item; }
	}

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

	void Awake () {
		CanvasGroup.blocksRaycasts = false;
		Events.instance.AddListener<ScrollDirectionEvent> (OnScrollDirectionEvent);
	}

	public void Init (TacticItem item) {
		this.item = item;
		titleText.text = item.Title;
		descriptionText.text = item.Description;
		contextText.text = item.Context;
		CanvasGroup.blocksRaycasts = false;
	}

	void OnScrollDirectionEvent (ScrollDirectionEvent e) {
		CanvasGroup.blocksRaycasts = !e.Vertical;
		Debug.Log (CanvasGroup.blocksRaycasts);
	}

	void BeginDrag () {
		if (!dragging) {
			Debug.Log ("heard");
			dragging = true;
			selected = this;
			dragPosition = transform.position - Input.mousePosition;
			CanvasGroup.blocksRaycasts = false;
			transform.SetParent (PlanPanel);
			StartCoroutine(CoDrag ());
		}
	}

	IEnumerator CoDrag () {
		while (dragging) {
			transform.position = dragPosition + (Vector2)Input.mousePosition;
			yield return null;
		}
	}

	void Update () {
         RectTransform objectRectTransform = gameObject.GetComponent<RectTransform> (); 
         float width = objectRectTransform.rect.width;
         float height = objectRectTransform.rect.height;
         float xpos = transform.position.x;
         float ypos = transform.position.y;
         if (Input.mousePosition.x > xpos - width * 0.5f
         	&& Input.mousePosition.x < xpos + width * 0.5f
         	&& Input.mousePosition.y < ypos
			&& Input.mousePosition.y > ypos - height) {
             // PerformRaycast ();
         	if (CanvasGroup.blocksRaycasts) {
         		BeginDrag ();
         	}
         }
     }

     public void OnPointerUp (PointerEventData eventData) {
     	Debug.Log ("up!!!!");
     	CanvasGroup.blocksRaycasts = false;
     }
	
	/*void PerformRaycast () {

		PointerEventData cursor = new PointerEventData(EventSystem.current);                            // This section prepares a list for all objects hit with the raycast
		cursor.position = Input.mousePosition;
		List<RaycastResult> objectsHit = new List<RaycastResult> ();
		EventSystem.current.RaycastAll(cursor, objectsHit);
		int count = objectsHit.Count;
		int x = 0;

		if (objectsHit[x].gameObject == this.gameObject) {    
			
		}
     }*/


	#region IBeginDragHandler, IDragHandler, IEndDragHandler implementation
	public void OnBeginDrag (PointerEventData eventData) {
		
		// Drag horizontally to select plan
		// if (Mathf.Abs (eventData.delta.x) < Mathf.Abs (eventData.delta.y)) {

		/*if (TacticScrollView.verticalScroll) {
			CanvasGroup.blocksRaycasts = false;
			return;
		}

		dragging = true;
		selected = this;
		dragPosition = transform.position - Input.mousePosition;
		CanvasGroup.blocksRaycasts = false;
		transform.SetParent (PlanPanel);*/
	}

	public void OnDrag (PointerEventData eventData) {
		// Debug.Log (CanvasGroup.blocksRaycasts);
		/*Debug.Log (TacticScrollView.verticalScroll);
		if (dragging) {
			transform.position = dragPosition + (Vector2)Input.mousePosition;
		}*/
	}

	public void OnEndDrag (PointerEventData eventData) {
		dragging = false;
		// CanvasGroup.blocksRaycasts = true;
	}
	#endregion
}

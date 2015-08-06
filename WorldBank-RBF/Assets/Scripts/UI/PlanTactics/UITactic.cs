using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class UITactic : TacticButton, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static UITactic selectedTactic = null;

	public Text titleText;
	public Text descriptionText;
	public Text contextText;

	public static GameObject itemBeingDragged;
	public static Vector3 startPosition;
	public static Transform startParent;

	LayoutElement layoutElement = null;
	LayoutElement Layout {
		get {
			if (layoutElement == null) {
				layoutElement = GetComponent<LayoutElement> ();
			}
			return layoutElement;
		}
	}

	public TacticItem Tactic { get { return tactic; } }

	TacticItem tactic;
	UITacticSlot slot = null;

	public void Init (Column column, Transform contentParent, TacticItem tactic) {
		this.tactic = tactic;

		// Set various text elements
		titleText.text = tactic.Title;
		descriptionText.text = tactic.Description;
		contextText.text = tactic.Context;
		
		Init (column, contentParent, tactic.Title);
	}

	public override void OnClick () {
		selectedTactic = this;

		// Used to set description
		Events.instance.Raise (new SelectTacticEvent (tactic));
	}

	public void OnClickRemove () {
		Events.instance.Raise (new TacticSlotEvent (tactic));
		// slot = null;
	}

	public void MoveToSlot (UITacticSlot newSlot) {
		// slot = newSlot;
		Events.instance.Raise (new TacticSlotEvent (this.tactic, newSlot.SiblingIndex));
		selectedTactic = null;
	}

	#region IBeginDragHandler, IDragHandler, IEndDragHandler implementation
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
		GetComponent<CanvasGroup>().blocksRaycasts = true;

		if(transform.parent != startParent) {
			transform.position = startPosition;

			return;
		}

		if(transform.parent == startParent) {			
			transform.position = startPosition;

		}
	}
	
	#endregion
	
}

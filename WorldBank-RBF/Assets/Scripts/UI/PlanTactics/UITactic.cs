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

	public ScrollRect ParentScrollRect {

		set {
			scrollParent = value;
		}

	}

	ScrollRect scrollParent;
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

	public void DisableLayout() {

		Layout.enabled = false;

	}

	#region IBeginDragHandler, IDragHandler, IEndDragHandler implementation
	public void OnBeginDrag (PointerEventData eventData)
	{
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;

		scrollParent.gameObject.GetComponent<Mask>().enabled = false;
		scrollParent.gameObject.GetComponent<Image>().enabled = false;

		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	
	public void OnDrag (PointerEventData eventData)
	{
		transform.position = Input.mousePosition;

		// foreach(GameObject hved in eventData.hovered)
		// 	Debug.Log(hved);
	}

	public void OnEndDrag (PointerEventData eventData)
	{

		itemBeingDragged = null;

		scrollParent.gameObject.GetComponent<Mask>().enabled = true;
		scrollParent.gameObject.GetComponent<Image>().enabled = true;

		GetComponent<CanvasGroup>().blocksRaycasts = true;

		Layout.enabled = (transform.parent == scrollParent.transform.GetChild(0).transform);

		if(transform.parent != startParent) {
			// transform.position = startPosition;

			return;
		}

		if(transform.parent == startParent)
			transform.position = startPosition;
	
	}
	
	#endregion
	
}

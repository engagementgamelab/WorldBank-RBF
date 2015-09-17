using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TacticScrollView : MB, IBeginDragHandler, IDragHandler, IDropHandler, IPointerUpHandler {

	public TacticsContainer container;
	
	bool verticalScroll = false;

	ScrollRect scroll = null;
	ScrollRect Scroll {
		get {
			if (scroll == null) {
				scroll = GetComponent<ScrollRect> ();
			}
			return scroll;
		}
	}

	void Awake () {
		Events.instance.AddListener<BeginDragTacticEvent> (OnBeginDragTacticEvent);
		Events.instance.AddListener<EndDragTacticEvent> (OnEndDragTacticEvent);
	}


	void OnBeginDragTacticEvent (BeginDragTacticEvent e) {
		Scroll.vertical = false;
	}

	void OnEndDragTacticEvent (EndDragTacticEvent e) {
		Scroll.vertical = true;
	}

	bool IsVerticalDrag (Vector2 delta, float range=0.25f) {
		if (delta.magnitude < 5f) return false;
		Vector2 deltaNormalized = delta.normalized;
		return deltaNormalized.x > -range && deltaNormalized.x < range;
	}

	#region IBeginDragHandler
	public void OnBeginDrag (PointerEventData eventData) {
		bool isVertical = IsVerticalDrag (eventData.delta);
		Debug.Log (isVertical);
		if (verticalScroll != isVertical) {
			verticalScroll = isVertical;
			Events.instance.Raise (new ScrollDirectionEvent (verticalScroll));
		}
	}

	public void OnDrag (PointerEventData eventData) {
		
		/*if (verticalScroll) {
			bool isVertical = IsVerticalDrag (eventData.delta, 0.1f);
			if (!isVertical) {
				verticalScroll = isVertical;
				Events.instance.Raise (new ScrollDirectionEvent (verticalScroll));
			}		
		}*/
	}
	#endregion

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData) {
		if (Tactic.selected != null)
			Tactic.selected.SetDropLocation (container);
	}
	#endregion

	#region IPointerUpHandler implementation
	public void OnPointerUp (PointerEventData eventData) {
		verticalScroll = false;
		Events.instance.Raise (new ScrollDirectionEvent (verticalScroll));
	}	
	#endregion
}

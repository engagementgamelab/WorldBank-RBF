using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TacticScrollView : MB, IBeginDragHandler, IDragHandler, IDropHandler {

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

	#region IBeginDragHandler
	public void OnBeginDrag (PointerEventData eventData) {
		bool isVertical = Mathf.Abs (eventData.delta.x) < Mathf.Abs (eventData.delta.y);
		if (verticalScroll != isVertical) {
			verticalScroll = isVertical;
			Events.instance.Raise (new ScrollDirectionEvent (verticalScroll));
		}
	}

	public void OnDrag (PointerEventData eventData) {}
	#endregion

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData) {
		if (Tactic.selected != null)
			Tactic.selected.SetDropLocation (container);
	}
	#endregion
}

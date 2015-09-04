using UnityEngine;
using UnityEngine.EventSystems;

public class TacticScrollView : MB, IBeginDragHandler, IDragHandler {

	bool verticalScroll = false;

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
}

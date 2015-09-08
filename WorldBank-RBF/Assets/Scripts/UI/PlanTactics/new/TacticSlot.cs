using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// http://answers.unity3d.com/questions/865191/unity-new-ui-drag-and-drop.html

public class TacticSlot : MB, IDropHandler, IBeginDragHandler, IDragHandler {

	public Text text;
	public TacticsContainer container;

	TacticItem tacticItem;

	#region IBeginDragHandler, IDropHandler implementation
	public void OnBeginDrag (PointerEventData eventData) {
		Tactic t = ObjectPool.Instantiate<Tactic> ();
		Vector3 createPosition = Position;
		createPosition.y += t.Height * 0.5f;
		t.Position = createPosition;
		t.StartDragging ();
		tacticItem = null;
		text.text = "";
	}

	public void OnDrag (PointerEventData eventData) {}
	#endregion

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData) {
		if (Tactic.selected == null)
			return;
		Tactic tactic = Tactic.selected;
		tacticItem = tactic.Item;
		if (tacticItem != null)
		 	text.text = tacticItem.Title;
	 	Events.instance.Raise (new DropTacticEvent (tactic));
	}
	#endregion
}

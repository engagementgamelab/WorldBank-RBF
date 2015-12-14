using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TacticSlot : DragLocation, IDropHandler, IPointerDownHandler {

	public int index;
	public Text text;
	public TacticsContainer container;

	Tactic currentTactic;
	TacticItem tacticItem;

	bool dropEnabled = true;
	public bool DropEnabled {
		get { return dropEnabled; }
		set { 
			dropEnabled = value; 
			Button.enabled = value;
		}
	}

	bool HasTactic {
		get { return currentTactic != null; }
	}

	Button button = null;
	Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}

	public void ClearSlot () {
		currentTactic = null;
	}

	public void FillSlot (Tactic tactic) {
		if (tactic.Item == null)
			return;
		if (HasTactic) {
			TradeSlot (tactic);
		} else {
			FillEmptySlot (tactic);
		}
		currentTactic = tactic;
		tacticItem.Priority = index;
		PlayerData.TacticPriorityGroup.Add (tacticItem);
	}

	void TradeSlot (Tactic tactic) {
		Tactic newTactic = CreateTactic ();
		newTactic.Init (tacticItem);
		newTactic.ForceFromSlot (tactic);
		FillEmptySlot (tactic);
	}

	void FillEmptySlot (Tactic tactic) {
		tacticItem = tactic.Item;
	 	text.text = tacticItem.Title;
	}

	Tactic CreateTactic () {
		Tactic t = ObjectPool.Instantiate<Tactic> ();
		Vector3 createPosition = Position;
		t.Position = createPosition;
		t.Init (tacticItem);
		t.DragData.FromLocation = this;
		t.DragData.ToLocation = this;
		return t;
	}

	#region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData) {
		if (!DropEnabled) return;
		if (currentTactic == null)
			return;
		Tactic t = CreateTactic ();
		t.StartDragging (this);
		currentTactic = null;
		tacticItem.Priority = -1;
		PlayerData.TacticPriorityGroup.Remove (tacticItem);
		tacticItem = null;
		text.text = "";
	}
	#endregion

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData) {
		if (!DropEnabled) return;
		Tactic selectedTactic = Tactic.selected;
		if (selectedTactic == null)
			return;
		FillSlot (selectedTactic);
	 	selectedTactic.SetDropLocation (this);
	}
	#endregion
}
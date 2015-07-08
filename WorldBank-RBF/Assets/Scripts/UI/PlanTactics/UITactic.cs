using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITactic : TacticButton {

	public static UITactic selectedTactic = null;

	Button removeButton = null;
	Button RemoveButton {
		get {
			if (removeButton == null) {
				removeButton = Transform.GetChild (1).GetComponent<Button> ();
			}
			return removeButton;
		}
	}

	LayoutElement layoutElement = null;
	LayoutElement Layout {
		get {
			if (layoutElement == null) {
				layoutElement = GetComponent<LayoutElement> ();
			}
			return layoutElement;
		}
	}

	PlanTacticItem tactic;
	public PlanTacticItem Tactic { get { return tactic; } }

	UITacticSlot slot = null;

	public void Init (Column column, Transform contentParent, PlanTacticItem tactic) {
		this.tactic = tactic;
		RemoveButton.gameObject.SetActive (tactic.Priority > -1);	
		Init (column, contentParent, tactic.Title);
	}

	public override void OnClick () {
		selectedTactic = this;

		// Used to set description
		Events.instance.Raise (new SelectTacticEvent (tactic));
	}

	public void OnClickRemove () {
		Events.instance.Raise (new TacticSlotEvent (tactic));
		slot = null;
	}

	public void MoveToSlot (UITacticSlot newSlot) {
		slot = newSlot;
		Events.instance.Raise (new TacticSlotEvent (tactic, slot.SiblingIndex));
		selectedTactic = null;
	}
}

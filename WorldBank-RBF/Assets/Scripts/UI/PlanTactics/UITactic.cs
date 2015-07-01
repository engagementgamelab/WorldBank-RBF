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

	void Awake () {
		Text.text = "incentivise improvement in patient/provider relations";
		SetRemoveButtonActive (false);
	}

	public void Init (Column column, Transform contentParent, PlanTacticItem tactic) {
		this.tactic = tactic;
		Init (column, contentParent, tactic.Title);
	}

	public override void OnClick () {
		selectedTactic = this;
		Events.instance.Raise (new SelectTacticEvent (tactic));
	}

	public void OnClickRemove () {
		if (slot == null) return;
		slot.OnRemoveTactic ();
		SetParent (contentParent);
		Transform.SetAsLastSibling ();
		SetRemoveButtonActive (false);
		slot = null;
	}

	public void MoveToSlot (UITacticSlot newSlot) {
		if (slot != null) { slot.OnRemoveTactic (); }
		slot = newSlot;
		Parent = newSlot.Parent;
		Transform.SetSiblingIndex (newSlot.SiblingIndex);
		SetRemoveButtonActive (true);
		selectedTactic = null;
	}

	void SetRemoveButtonActive (bool active) {
		RemoveButton.gameObject.SetActive (active);	
	}
}

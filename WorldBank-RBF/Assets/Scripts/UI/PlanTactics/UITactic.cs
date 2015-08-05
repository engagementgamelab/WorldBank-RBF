using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITactic : TacticButton {

	public static UITactic selectedTactic = null;

	public Text titleText;
	public Text descriptionText;
	public Text contextText;

	LayoutElement layoutElement = null;
	LayoutElement Layout {
		get {
			if (layoutElement == null) {
				layoutElement = GetComponent<LayoutElement> ();
			}
			return layoutElement;
		}
	}

	TacticItem tactic;
	public TacticItem Tactic { get { return tactic; } }

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
		slot = null;
	}

	public void MoveToSlot (UITacticSlot newSlot) {
		slot = newSlot;
		Events.instance.Raise (new TacticSlotEvent (tactic, slot.SiblingIndex));
		selectedTactic = null;
	}
}

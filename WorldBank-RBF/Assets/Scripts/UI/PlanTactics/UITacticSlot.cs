using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITacticSlot : TacticButton {

	int siblingIndex;
	public int SiblingIndex {
		get { return siblingIndex;}
	}

	public override void Init (Column column, Transform contentParent, string content) {
		base.Init (column, contentParent, content);
		siblingIndex = Transform.GetSiblingIndex ();
	}

	public override void OnClick () {
		if (UITactic.selectedTactic != null) {
			UITactic.selectedTactic.MoveToSlot (this);
			gameObject.SetActive (false);
			Transform.SetAsLastSibling ();

			// Event broadcast
			Events.instance.Raise(new TacticSlotEvent(SiblingIndex));
		}
	}

	public void OnRemoveTactic () {

		gameObject.SetActive (true);
		Transform.SetSiblingIndex (siblingIndex);

		// Event broadcast
		Events.instance.Raise(new TacticSlotEvent(SiblingIndex, false));
	}
}

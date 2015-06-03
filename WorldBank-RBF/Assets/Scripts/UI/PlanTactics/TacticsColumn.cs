using UnityEngine;
using System.Collections;

public class TacticsColumn : MB {

	public RectTransform content;
	Inventory inventory;
	PlanTacticGroup _tactics;

	void Awake () {
		inventory = new Inventory ();

		// testing
		_tactics = new PlanTacticGroup ();
		inventory.Add (_tactics);
		_tactics.Add (new PlanTacticItem (null, 1));
		_tactics.Add (new PlanTacticItem (null, 2));
		_tactics.Add (new PlanTacticItem (null, 3));

		SetTactics (_tactics);
	}

	public void SetTactics (PlanTacticGroup tactics) {
		ObjectPool.DestroyAll<UITactic> ();
		foreach (PlanTacticItem tactic in tactics.Items) {
			UITactic uiTactic = ObjectPool.Instantiate<UITactic> ();
			uiTactic.SetContent (tactic.Title);
			uiTactic.Parent = content;
			uiTactic.Transform.Reset ();
		}
	}

	public void DragTactic (UITactic uiTactic) {
		uiTactic.Parent = null;
		UITacticPlaceholder placeholder = ObjectPool.Instantiate<UITacticPlaceholder> ();
		placeholder.Parent = content;
		placeholder.Transform.Reset ();
	}
}

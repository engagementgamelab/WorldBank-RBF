using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TacticsColumn : Column {

	List<UITactic> uiTactics;

	void Awake () {
		// TODO: set it up this way eventually... (similar to how routes & cities are handled)
		// PlayerData.PlanTacticGroup.onUpdate += OnUpdate;
	}

	/*void OnUpdate () {

		ObjectPool.DestroyAll<UITactic> ();
		uiTactics = new List<UITactic> ();

		foreach (PlanTacticItem tactic in PlayerData.PlanTacticGroup.Items) {
			if (tactic.Unlocked)
				CreateUITactic (tactic);
		}
	}*/

	public List<UITactic> Init (PlanTacticGroup tactics, TacticPriorityGroup priorities) {
		
		ObjectPool.DestroyAll<UITactic> ();
		uiTactics = new List<UITactic> ();
		List<UITactic> priorityUITactics = new List<UITactic> ();
		
		foreach (PlanTacticItem tactic in tactics.Items) {
			if (tactic.Unlocked)
				CreateUITactic (tactic);
		}

		foreach (PlanTacticItem tactic in priorities.Items) {
			priorityUITactics.Add (CreateUITactic (tactic));
		}
		return priorityUITactics;
	}

	public PlanTacticGroup GetTactics () {
		PlanTacticGroup group = new PlanTacticGroup ();
		foreach (Transform child in content.transform) {
			UITactic tactic = child.GetScript<UITactic> ();
			if (tactic != null && tactic.Tactic != null) {
				tactic.Tactic.Priority = -1;
				group.Add (tactic.Tactic);
			}
		}
		return group;
	}

	public void ResetTactics () {
		if (uiTactics == null) return;
		foreach (UITactic uiTactic in uiTactics) {
			uiTactic.OnClickRemove ();
		}
	}

	UITactic CreateUITactic (PlanTacticItem tactic) {
		UITactic uiTactic = ObjectPool.Instantiate<UITactic> ();
		uiTactic.Init (this, content, tactic);
		uiTactics.Add (uiTactic);
		return uiTactic;
	}
}

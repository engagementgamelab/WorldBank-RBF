using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TacticsColumn : Column {

	List<UITactic> uiTactics = new List<UITactic> ();

	void Awake () {
		PlayerData.PlanTacticGroup.onUpdate += OnUpdate;
		PlayerData.TacticPriorityGroup.onUpdate += OnUpdate;
	}

	void OnUpdate () {

		ObjectPool.Destroy<UITactic> (uiTactics.ConvertAll (x => x.Transform));
		uiTactics.Clear ();

		foreach (PlanTacticItem tactic in PlayerData.PlanTacticGroup.Items) {
			if (tactic.Unlocked && tactic.Priority == -1)
				CreateUITactic (tactic);
		}
	}

	UITactic CreateUITactic (PlanTacticItem tactic) {
		UITactic uiTactic = ObjectPool.Instantiate<UITactic> ();
		uiTactic.Init (this, content, tactic);
		uiTactics.Add (uiTactic);
		return uiTactic;
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TacticsColumn : Column {

	public ScrollRect scrollView;

	List<UITactic> uiTactics = new List<UITactic> ();
	bool initialized = false;

	void OnEnable () {
		// PlayerData.TacticGroup.onUpdate += OnUpdate;
		// PlayerData.TacticPriorityGroup.onUpdate += OnUpdate;

		OnUpdate();
	}

	void OnUpdate () {

		ObjectPool.Destroy<UITactic> (uiTactics.ConvertAll (x => x.Transform));
		uiTactics.Clear ();

		foreach (TacticItem tactic in PlayerData.TacticGroup.Items) {
			if (tactic.Unlocked && tactic.Priority == -1)
				CreateUITactic (tactic);
		}
	}

	UITactic CreateUITactic (TacticItem tactic) {
		
		UITactic uiTactic = ObjectPool.Instantiate<UITactic> ();
		uiTactic.ParentScrollRect = scrollView;

		uiTactic.Init (this, content, tactic);
		uiTactics.Add (uiTactic);
		
		return uiTactic;

	}
}

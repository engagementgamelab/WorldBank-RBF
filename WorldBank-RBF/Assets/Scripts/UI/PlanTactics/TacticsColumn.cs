using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Deprecated 9/16

public class TacticsColumn : Column {

	public ScrollRect scrollView;

	List<UITactic> uiTactics = new List<UITactic> ();
	bool initialized = false;

	LayoutElement contentLayout;
	float ContentHeight {
		get {
			if (contentLayout == null) {
				contentLayout = content.GetComponent<LayoutElement> ();
			}
			return contentLayout.minHeight;
		}
		set {
			if (contentLayout == null) {
				contentLayout = content.GetComponent<LayoutElement> ();
			}
			contentLayout.minHeight = value;
		}
	}

	void OnEnable () {
		OnUpdate();
		PlayerData.TacticPriorityGroup.onUpdate += UpdateContentHeight;
		UpdateContentHeight ();
	}

	void OnDisable () {
		PlayerData.TacticPriorityGroup.onUpdate -= UpdateContentHeight;
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
		//uiTactic.portrait.NPCSymbol = tactic.Npc;

		uiTactic.Init (this, content, tactic);
		uiTactics.Add (uiTactic);
		
		return uiTactic;

	}

	void UpdateContentHeight () {
		ContentHeight = content.childCount < 2 ? 545 : 0;
	}
}

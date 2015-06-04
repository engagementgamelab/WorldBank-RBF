using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrioritiesColumn : Column {

	int slotCount = 6;

	void Awake () {
		CreateTacticSlots ();
	}

	public void CreateTacticSlots () {
		for (int i = 0; i < slotCount; i ++) {
			UITacticSlot slot = ObjectPool.Instantiate<UITacticSlot> ();
			string title = "";
			if (i < 2) { title = "Top priority"; }
			if (i >= 2 && i < 5) { title = "Medium priority"; }
			if (i == 5)	{ title = "Lower priority"; }
			slot.Init (this, content, title);
		}
	}

	public void SetPriorities (List<UITactic> priorities) {
		// super hacky
		foreach (UITactic tactic in priorities) {
			int priority = tactic.Tactic.Priority;
			tactic.OnClick ();
			content.GetChild (priority).GetScript<UITacticSlot> ().OnClick ();
		}
	}

	public TacticPriorityGroup GetPriorities () {
		TacticPriorityGroup group = new TacticPriorityGroup ();
		foreach (Transform child in content.transform) {
			UITactic tactic = child.GetScript<UITactic> ();
			if (tactic != null) {
				tactic.Tactic.Priority = child.GetSiblingIndex ();
				group.Add (tactic.Tactic);
			}
		}
		return group;
	}
}

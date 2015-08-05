using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrioritiesColumn : Column {

	int slotCount = 6;
	List<UITacticSlot> uiSlots = new List<UITacticSlot> ();
	List<UITactic> uiTactics = new List<UITactic> ();
	bool initialized = false;

	void Awake () {
		CreateTacticSlots ();
	}

	void OnEnable () {
		if (!initialized) {
			PlayerData.TacticPriorityGroup.onUpdate += OnUpdate;
			initialized = true;
		}
	}

	void OnEnable() {

		OnUpdate();
		
	}

	public void OnUpdate () {
		
		ObjectPool.Destroy<UITactic> (uiTactics.ConvertAll (x => x.Transform));
		uiTactics.Clear ();

		ActivateSlots ();

		foreach (TacticItem tactic in PlayerData.TacticPriorityGroup.Items) {
			int priority = tactic.Priority;
			UITactic t = CreateUITactic (tactic);
			DeactivateSlot (uiSlots[priority]);
			t.Transform.SetSiblingIndex (priority);
		}		
	}

	public void CreateTacticSlots () {
		for (int i = 0; i < slotCount; i ++) {
			UITacticSlot slot = ObjectPool.Instantiate<UITacticSlot> ();
			string title = "";
			if (i < 2) { title = "Top priority"; }
			if (i >= 2 && i < 5) { title = "Medium priority"; }
			if (i == 5)	{ title = "Lower priority"; }
			slot.Init (this, content, title);
			uiSlots.Add (slot);
		}
	}

	UITactic CreateUITactic (TacticItem tactic) {
		UITactic uiTactic = ObjectPool.Instantiate<UITactic> ();
		uiTactic.Init (this, content, tactic);
		uiTactics.Add (uiTactic);
		return uiTactic;
	}

	void ActivateSlots () {
		foreach (UITacticSlot slot in uiSlots) {
			slot.gameObject.SetActive (true);
			slot.Transform.SetSiblingIndex (slot.SiblingIndex);
		}
	}

	void DeactivateSlot (UITacticSlot slot) {
		slot.gameObject.SetActive (false);
		slot.Transform.SetAsLastSibling ();
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PrioritiesColumn : Column {

	public ScrollRect tacticsScrollView;

	int slotCount = 6;
	List<UITacticSlot> uiSlots = new List<UITacticSlot> ();
	List<UITactic> uiTactics = new List<UITactic> ();

	void Awake () {
		// CreateTacticSlots ();
	}

	void OnEnable() {

		OnUpdate();
		
	}

	public void OnUpdate () {
		
/*		ObjectPool.Destroy<UITactic> (uiTactics.ConvertAll (x => x.Transform));
		uiTactics.Clear ();

		ActivateSlots ();

		foreach (TacticItem tactic in PlayerData.TacticPriorityGroup.Items) {
			int priority = tactic.Priority;
			UITactic t = CreateUITactic (tactic);
			DeactivateSlot (uiSlots[priority]);
			t.Transform.SetSiblingIndex (priority);
		}	*/	

		ObjectPool.DestroyChildren<UITacticSlot>(transform);
		ObjectPool.DestroyChildren<UITacticHeader>(transform);

		CreateTacticSlots();
	}

	public void CreateTacticSlots () {
		for (int i = 0; i < slotCount; i ++) {
			
			string title = "";
			if (i == 0) { title = "Top priority"; }
			else if (i == 2) { title = "Medium priority"; }
			else if (i == 5)	{ title = "Lower priority"; }

			if(title != "") {

				UITacticHeader header = ObjectPool.Instantiate<UITacticHeader> ();
				header.Text = title;
				header.Transform.SetParent(content.transform);
				header.Transform.SetLocalScale (1);

			}

		
			UITacticSlot slot = ObjectPool.Instantiate<UITacticSlot> ();
			ObjectPool.DestroyChildren<UITactic>(slot.transform);

			slot.Init (this, content, title);
			uiSlots.Add (slot);

			if(PlayerData.TacticPriorityGroup.Items.Count <= i)
				continue;

			if(PlayerData.TacticPriorityGroup.Items[i] != null)
			{
				TacticItem tactic = PlayerData.TacticPriorityGroup.Items[i] as TacticItem;

				UITactic uiTactic = ObjectPool.Instantiate<UITactic> ();
				uiTactic.ParentScrollRect = tacticsScrollView;

				uiTactic.Init (this, content, tactic);
				uiTactic.DisableLayout();
				uiTactic.transform.SetParent(slot.transform);

			}

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
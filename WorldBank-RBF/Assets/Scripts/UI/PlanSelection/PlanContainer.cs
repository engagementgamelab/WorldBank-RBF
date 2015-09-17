using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlanContainer : MonoBehaviour {

	public bool dropEnabled = true;
	public List<TacticSlot> slots;

	void Awake () {
		foreach (TacticSlot s in slots) {
			s.DropEnabled = dropEnabled;
		}
	}

	public void SetTactics (List<string> tactics) {

		for (int i = 0; i < slots.Count; i ++) {
			slots[i].text.text = tactics[i];
		}
	}
}

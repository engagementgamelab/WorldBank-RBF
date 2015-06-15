using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DayCounter : MB {

	Text text = null;
	Text Text {
		get {
			if (text == null) {
				text = Transform.GetChild (1).GetComponent<Text> ();
			}
			return text;
		}
	}

	public int Count {
		get { return days.Count; }
	}

	public bool HasDays {
		get { return !initialized || !days.Empty; }
	}

	Inventory inventory = new Inventory ();
	DayGroup days = new DayGroup ();
	bool initialized = false;

	void Awake () {
		inventory.Add (days);
		days.onUpdateCount += OnUpdateCount;
		days.Add (15);
		initialized = true;
	}

	public bool RemoveDays (int count) {
		if (days.Count >= count) {
			days.Remove (count);
			return true;
		}
		return false;
	}

	void OnUpdateCount () {
		Text.text = "Days: " + days.Count;
	}
}

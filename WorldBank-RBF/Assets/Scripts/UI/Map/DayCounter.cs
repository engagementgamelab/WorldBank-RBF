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

	int count = 0;
	public int Count {
		get { return count; }
		set {
			count = value;
			Text.text = "Days: " + count;
		}
	}

	void Awake () {
		Count = 15;
	}

	public bool RemoveDays (int count) {
		if (Count >= count) {
			Count -= count;
			return true;
		}
		return false;
	}
}

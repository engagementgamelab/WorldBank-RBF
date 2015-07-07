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

	void Awake () {
		PlayerData.DayGroup.onUpdate += OnUpdateCount;
		OnUpdateCount ();
	}

	void OnUpdateCount () {
		Text.text = "Days: " + PlayerData.DayGroup.Count;
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DayCounter : MB {

	public Text text;
	public List<Text> outlines;

	void Awake () {
		PlayerData.DayGroup.onUpdate += OnUpdateCount;
		OnUpdateCount ();
	}

	void OnUpdateCount () {
		string newText = "Days: " + PlayerData.DayGroup.Count;
		text.text = newText;
		foreach (Text outline in outlines) {
			outline.text = newText;
		}
	}
}

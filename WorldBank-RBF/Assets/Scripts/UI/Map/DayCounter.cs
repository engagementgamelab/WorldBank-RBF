using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DayCounter : MB {

	public Text text;

	void Awake () {
		PlayerData.DayGroup.onUpdate += OnUpdateCount;
		OnUpdateCount ();
	}

	void OnUpdateCount () {
		text.text = "Days: " + PlayerData.DayGroup.Count;
	}
}

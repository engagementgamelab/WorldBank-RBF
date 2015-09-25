using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DayCounter : Counter {

	protected override float Offset { // hack alert
		get { return -25f; }
	}

	protected override void SetUpdateCallback () {
		PlayerData.DayGroup.onUpdate += OnUpdate;
	}

	protected override void OnUpdate () {
		Count = PlayerData.DayGroup.Count;
	}
}

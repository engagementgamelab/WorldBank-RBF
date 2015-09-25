using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractionsCounter : Counter {

	protected override void SetUpdateCallback () {
		PlayerData.InteractionGroup.onUpdate += OnUpdate;
	}

	protected override void OnUpdate () {
		Count = PlayerData.InteractionGroup.Count;
	}
}

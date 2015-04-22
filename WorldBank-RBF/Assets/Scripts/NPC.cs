using UnityEngine;
using System.Collections;

public class NPC : MB, IClickable {

	public InputLayer[] IgnoreLayers { get { return null; } }

	public void OnClick (ClickSettings clickSettings) {
		Debug.Log ("heard");
	}
}

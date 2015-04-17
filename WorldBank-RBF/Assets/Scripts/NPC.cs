using UnityEngine;
using System.Collections;

public class NPC : MB, IClickable {

	public void OnClick (ClickSettings clickSettings) {
		Debug.Log ("heard");
	}
}

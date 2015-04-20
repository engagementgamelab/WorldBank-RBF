using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class NpcOptions : ScriptableObject {

	public void OnEnable () {
		hideFlags = HideFlags.HideAndDontSave;
	}
	
	public void OnGUI () {

	}	
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITacticHeader : MB {
	
	public string Text {
		set {
			GetComponent<Text> ().text = value;
		}
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITacticHeader : MonoBehaviour {
	
	public string Text {
		set {
			GetComponent<Text> ().text = value;
		}
	}

}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITactic : MB {

	Text text;
	Text Text {
		get {
			if (text == null) {
				text = Transform.GetChild (0).GetComponent<Text> ();
			}
			return text;
		}
	}

	void Awake () {
		SetContent ("incentivise improvement in patient/provider relations");
	}

	public void SetContent (string content) {
		Text.text = content;
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NPCDialogButton : MonoBehaviour {
	
	Button button;
	public Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> () as Button;
			}
			return button;
		}
	}

	public string Text {

		get {

			return GetComponent<Transform>().FindChild("Text").GetComponent<Text>().text;
		}

		set {

			GetComponent<Transform>().FindChild("Text").GetComponent<Text>().text = value;

		}

	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NPCDialogButton : MonoBehaviour {
	
	Button button;

	/// <summary>
    /// Get a reference to the button
    /// </summary>
	public Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> () as Button;
			}
			return button;
		}
	}

	/// <summary>
    /// Get or set the text on the button
    /// </summary>
	public string Text {

		get {

			return GetComponent<Transform>().FindChild("Text").GetComponent<Text>().text;
		}
		set {

			GetComponent<Transform>().FindChild("Text").GetComponent<Text>().text = value;

		}

	}
}

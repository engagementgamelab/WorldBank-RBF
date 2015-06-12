using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CityInfoBoxButton : MonoBehaviour {

	Text text = null;
	Text Text {
		get {
			if (text == null) {
				text = transform.GetChild (0).GetComponent<Text> ();
			}
			return text;
		}
	}

	public string Label {
		set { Text.text = value; }
	}

	Button button = null;
	public Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class EmailField : MB {

	public static string email = "";

	InputField inputField = null;
	InputField InputField {
		get {
			if (inputField == null) {
				inputField = GetComponent<InputField> ();
			}
			return inputField;
		}
	}

	Text text = null;
	Text Text {
		get {
			if (text == null) {
				foreach (Transform child in Transform) {
					if (child.name == "Text")
						text = child.GetComponent<Text> ();
				}
			}
			return text;
		}
	}

	void OnEnable () {
		InputField.text = email;
		Text.text = email;
	}

	public void OnUpdateEmail () {
		email = InputField.text;
	}

	public static bool IsValidEmail (string email) {
		return Regex.Matches (email, @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$").Count == 1;
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractionsCounter : MB {

	Text text = null;
	Text Text {
		get {
			if (text == null) {
				text = Transform.GetChild (2).GetComponent<Text> ();
			}
			return text;
		}
	}

	public int Count {
		set { Text.text = value.ToString (); }
	}
}

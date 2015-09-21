using UnityEngine;
using UnityEngine.UI;

public class UIText : MonoBehaviour {

	public string key;

	Text text = null;
	Text Text {
		get {
			if (text == null) {
				text = GetComponent<Text> ();
			}
			return text;
		}
	}

	void Awake () {
		Text.text = DataManager.GetUIText (key);
	}
}

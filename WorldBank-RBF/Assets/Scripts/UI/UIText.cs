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

	void OnEnable () {
		string t = DataManager.GetUIText (key);
		if (t != DataManager.DataNotLoaded)
			Text.text = t;
	}
}

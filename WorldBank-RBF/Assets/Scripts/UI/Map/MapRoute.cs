using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapRoute : MB {

	Text text = null;
	Text Text {
		get {
			if (text == null) {
				text = Transform.GetChild (1).GetComponent<Text> ();
			}
			return text;
		}
	}

	Transform line = null;
	Transform Line {
		get {
			if (line == null) {
				line = Transform.GetChild (0);
			}
			return line;
		}
	}

	[SerializeField] bool unlocked = false;
	public bool Unlocked {
		get { return unlocked; }
		set {
			unlocked = value;
			Line.gameObject.SetActive (unlocked);
			Text.gameObject.SetActive (unlocked);
		}
	}

	public int cost = 1;

	void Awake () {
		Text.text = cost.ToString ();
	}
}

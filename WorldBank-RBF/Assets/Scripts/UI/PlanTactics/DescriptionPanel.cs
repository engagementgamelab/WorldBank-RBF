using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DescriptionPanel : MB {

	Text text = null;
	Text Text {
		get {
			if (text == null) {
				text = Transform.GetChild (0).GetComponent<Text> ();
			}
			return text;
		}
	}

	void Awake () {
		Events.instance.AddListener<SelectTacticEvent> (OnSelectTacticEvent);
	}

	void OnSelectTacticEvent (SelectTacticEvent e) {
		TacticItem tactic = e.tactic;
		Text.text = tactic.Description;
	}
}

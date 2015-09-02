using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class NpcDialogBox : MB {

	Transform background = null;
	Transform Background {
		get {
			if (background == null) {
				background = Transform.GetChild (0);
			}
			return background;
		}
	}

	Transform panel = null;
	Transform Panel {
		get {
			if (panel == null) {
				panel = Transform.GetChild (1);
			}
			return panel;
		}
	}

	public Text header;
	public Text body;
	public Transform contentContainer;
	public List<Button> buttons;
	public List<FadeText> fadeTexts;

	void Awake () {
		Close ();
	}

	public void Open (string headerContent, string bodyContent, Dictionary<string, UnityAction> choices, bool left) {
		header.text = headerContent;
		body.text = bodyContent;
		contentContainer.SetSiblingIndex (left ? 0 : 1);
		Background.SetLocalEulerAnglesZ (left ? 0 : 180);
		SetButtons (choices);
		SetActive (true);
		foreach (FadeText ft in fadeTexts) {
			if (ft.Parent.gameObject.activeSelf) {
				ft.FadeIn (0.33f);
			}
		}
	}

	public void Close () {
		SetActive (false);
	}

	void SetActive (bool active) {
		Background.gameObject.SetActive (active);
		Panel.gameObject.SetActive (active);
	}

	void SetButtons (Dictionary<string, UnityAction> choices) {
		ClearButtons ();
		int index = 0;
		foreach (var choice in choices) {
			if (choice.Key == "Back") continue;
			AddButton (buttons[index], choice.Key, choice.Value);
			index ++;
		}
		UnityAction backAction;
		if (choices.TryGetValue ("Back", out backAction)) {
			AddButton (buttons[index], "Back", backAction);
		}
	}

	void ClearButtons () {
		foreach (Button b in buttons) {
			b.onClick.RemoveAllListeners ();
			b.gameObject.SetActive (false);
		}
	}

	void AddButton (Button button, string content, UnityAction action) {
		button.gameObject.SetActive (true);
		button.transform.GetChild (0).GetComponent<Text> ().text = content;
		button.onClick.AddListener (action);
	}
}

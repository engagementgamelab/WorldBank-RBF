using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnlockNotification : MB {

	public RectTransform panel;
	public Text header;
	public Text body;

	string headerText;
	string HeaderText {
		get { return header.text; }
		set { header.text = value; }
	}

	Color headerColor;
	Color HeaderColor {
		get { return header.color; }
		set { header.color = value; }
	}

	string bodyText;
	string BodyText {
		get { return body.text; }
		set { body.text = value; }
	}

	Dictionary<string, string> headers = new Dictionary<string, string> () {
		{ "dialogue", "New dialogue unlocked!" },
		{ "route", "New route unlocked!" },
		{ "tactic", "New tactic unlocked!" }
	};

	Dictionary<string, Color> headerColors = new Dictionary<string, Color> () {
		{ "dialogue", Color.cyan },
		{ "route", Color.yellow },
		{ "tactic", Color.white }
	};

	float startPosition;
	float endPosition;
	bool sliding = false;

	void Start () {

		endPosition = panel.localPosition.x;
		startPosition = endPosition + panel.sizeDelta.x + 10;
		panel.SetLocalPositionX (startPosition);

		PlayerData.DialogueGroup.onUnlock += OnAddDialogue;
		PlayerData.RouteGroup.onUnlock += OnAddRoute;
		PlayerData.TacticGroup.onUnlock += OnAddTactic;
	}

	void OnAddDialogue (DialogueItem dialogue) {
		OnAdd ("dialogue", dialogue.Context);
	}

	void OnAddRoute (RouteItem route) {
		OnAdd ("route", route.Context);
	}

	void OnAddTactic (TacticItem tactic) {
		OnAdd ("tactic", tactic.Context);
	}

	void OnAdd (string type, string context) {
		StartCoroutine (CoWaitForDialogueComplete (type, context));
	}

	IEnumerator CoWaitForDialogueComplete (string type, string context) {
		while (!NPCFocusBehavior.Instance.Unfocused)
			yield return null;
		SlideIn (type, context);
	}

	void SlideIn (string type, string context) {
		HeaderText = headers[type];
		HeaderColor = headerColors[type];
		BodyText = context;
		StartCoroutine (CoSlide (startPosition, endPosition));
		Invoke ("SlideOut", 3.5f);
	}

	void SlideOut () {
		if (panel.localPosition.x <= endPosition)
			StartCoroutine (CoSlide (endPosition, startPosition));
	}

	IEnumerator CoSlide (float from, float to) {
		
		if (sliding) yield break;
		sliding = true;

		float time = 0.5f;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			panel.SetLocalPositionX (Mathf.Lerp (from, to, progress));
			yield return null;
		}

		panel.SetLocalPositionX (to);
		sliding = false;
	}

	void GetContext (string symbol) {
		// PlayerData.CharacterGroup.
	}

	public void OnClick () {
		SlideOut ();
	}
}

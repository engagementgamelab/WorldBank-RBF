using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnlockNotification : MB {

	public RectTransform panel;
	public Text header;
	public Text body;

	string HeaderText {
		get { return header.text; }
		set { header.text = value; }
	}

	Color headerColor;
	Color HeaderColor {
		get { return header.color; }
		set { header.color = value; }
	}

	string BodyText {
		get { return body.text; }
		set { body.text = value; }
	}

	struct Settings {
		
		public readonly string Header;
		public readonly Color Color;
		public readonly string[] Sfx;

		public Settings (string header, Color color, string[] sfx) {
			Header = header;
			Color = color;
			Sfx = sfx;
		}
	}

	Dictionary<string, Settings> settings = new Dictionary<string, Settings> {
		{ 
			"dialogue", 
			new Settings (
				"New dialogue unlocked!", 
				Color.cyan, 
				new [] { "newtravel", "map" }
			)
		},
		{ 
			"route", 
			new Settings (
				"New route unlocked!", 
				Color.yellow, 
				new [] { "newtravel", "map" }
			)
		},
		{ 
			"tactic", 
			new Settings (
				"New tactic unlocked!", 
				Color.white, 
				new [] { "newtravel", "map" }
			)
		}
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
		Settings s = settings[type];
		HeaderText = s.Header;//headers[type];
		HeaderColor = s.Color;//headerColors[type];
		AudioManager.Sfx.Play (s.Sfx[0], s.Sfx[1]);
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

	public void OnClick () {
		SlideOut ();
	}
}

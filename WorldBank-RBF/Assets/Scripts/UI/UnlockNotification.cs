using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnlockNotification : MB {

	public RectTransform panel;
	public Text header;
	public Text body;
	public CanvasToggle plan;
	public CanvasToggle map;

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
				"copy_unlock_notifications_dialog",
				new Color (0.11f, 0.35f, 0.463f), 
				new [] { "newtravel", "map" }
			)
		},
		{ 
			"route", 
			new Settings (
				"copy_unlock_notifications_route",
				new Color (0.874f, 0.773f, 0.506f), 
				new [] { "newtravel", "map" }
			)
		},
		{ 
			"tactic", 
			new Settings (
				"copy_unlock_notifications_tactic",
				Color.white, 
				new [] { "newtactic", "plan" }
			)
		},
		{ 
			"indicators", 
			new Settings (
				"copy_unlock_notifications_indicators", 
				new Color (0.874f, 0.773f, 0.506f), 
				new [] { "graphupdated", "phase2" }
			)
		}
	};

	float startPosition;
	float endPosition;
	bool sliding = false;
	string currentNotification;

	void Start () {

		endPosition = panel.localPosition.x;
		startPosition = endPosition + panel.sizeDelta.x + 10;
		panel.SetLocalPositionX (startPosition);

		PlayerData.DialogueGroup.onUnlock += OnAddDialogue;
		PlayerData.RouteGroup.onUnlock += OnAddRoute;
		PlayerData.TacticGroup.onUnlock += OnAddTactic;

		NotebookManager.Instance.onUpdate += OnIndicatorsUpdated;
		NPCFocusBehavior.Instance.onSetFocus += OnSetFocus;
	}

	void OnAddDialogue (DialogueItem dialogue) {
		OnAdd ("dialogue", dialogue.Context[dialogue.Context.Count-1]);
	}

	void OnAddRoute (RouteItem route) {
		OnAdd ("route", route.Context[route.Context.Count-1]);
	}

	void OnAddTactic (TacticItem tactic) {
		OnAdd ("tactic", tactic.Context[tactic.Context.Count-1]);
	}

	void OnIndicatorsUpdated () {
		OnAdd ("indicators", "Click here to open.");
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
		currentNotification = type;
		Settings s = settings[type];
		HeaderText = DataManager.GetUIText (s.Header);
		HeaderColor = s.Color;
		AudioManager.Sfx.Play (s.Sfx[0], s.Sfx[1]);
		BodyText = context;
		StartCoroutine (CoSlide (startPosition, endPosition));

		// Tutorial (player unlocks first tactic; do not slide out until confirm)
		List<TacticItem> tactics = PlayerData.TacticGroup.Tactics;
		if(tactics.Where(i => i.Unlocked).Count() == 1)
			DialogManager.instance.CreateTutorialScreen("phase_1_unlocked_something", "phase_1_plan_creation_screen", () => { SlideOut(); });
		else
			Invoke ("SlideOut", 10f);
	}

	void SlideOut () {
		CancelInvoke ();
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
		switch (currentNotification) {
			case "dialogue": SlideOut (); break;
			case "route": map.OnClick (); break;
			case "tactic": plan.OnClick (); break;
			case "indicators": NotebookManager.Instance.indicators.Open(); break;
		}
		SlideOut ();
	}

	void OnSetFocus (FocusLevel focus) {
		if (focus == FocusLevel.Preview || focus == FocusLevel.Dialog)
			SlideOut ();
	}
}

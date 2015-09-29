using UnityEngine;
using System.Collections.Generic;

public class UnlockNotifications : MonoBehaviour {

	RectTransform rectTransform = null;
	RectTransform RectTransform {
		get {
			if (rectTransform == null) {
				rectTransform = GetComponent<RectTransform> ();
			}
			return rectTransform;
		}
	}

	float XPosition {
		get { return RectTransform.anchoredPosition.x; }
		set { RectTransform.SetAnchorPositionX (value); }
	}

	public CanvasToggle map;
	public CanvasToggle plan;
	public List<UnlockNotification> notifications;

	const float max = 400;
	const float min = 800;
	const float delay = 0.33f;

	readonly List<string[]> queue = new List<string[]> ();
	bool showedTutorial = false;

	void Start () {

		PlayerData.DialogueGroup.onUnlock += OnAddDialogue;
		PlayerData.RouteGroup.onUnlock += OnAddRoute;
		PlayerData.TacticGroup.onUnlock += OnAddTactic;
		NotebookManager.Instance.onUpdate += OnIndicatorsUpdated;

		NPCFocusBehavior.Instance.onSetFocus += OnSetFocus;

		for (int i = 0; i < notifications.Count; i ++) {
			notifications[i].InDelay = delay * (i+1);
			notifications[i].OutDelay = delay * (Mathf.Abs (notifications.Count-i));
			notifications[i].gameObject.SetActive (false);
		}
	}

	public void Click (string type) {
		switch (type) {
			case "route": map.Open (); break;
			case "tactic": plan.Open (); break;
			case "indicators": NotebookManager.Instance.indicators.Open(); break;
		}
		foreach (UnlockNotification n in notifications) {
			if (n.gameObject.activeSelf && n.CurrentNotification == type)
				n.OnClose (true);
		}
	}

	void OnAddDialogue (ModelItem dialogue) {
		OnAdd ("dialogue", dialogue.Context[dialogue.Context.Count-1]);
	}

	void OnAddRoute (ModelItem route) {
		OnAdd ("route", route.Context[route.Context.Count-1]);
	}

	void OnAddTactic (ModelItem tactic) {
		OnAdd ("tactic", tactic.Context[tactic.Context.Count-1]);
	}

	void OnIndicatorsUpdated () {
		OnAdd ("indicators", "Click here to open.");
	}

	void OnSetFocus (FocusLevel focus) {
		if (focus == FocusLevel.Default && queue.Count > 0) {
			ActivateNotifications ();
		} else {
			DeactivateNotifications ();
			queue.Clear ();
		}
	}

	void OnAdd (string type, string context) {
		if (queue.Count < 3)
			queue.Add (new [] { type, context });
	}

	void ActivateNotifications () {
		if (DataManager.tutorialEnabled && !showedTutorial) {
			DialogManager.instance.CreateTutorialScreen("phase_1_unlocked_something", "phase_1_plan_creation_screen");
			showedTutorial = true;
		}
		for (int i = 0; i < queue.Count; i ++) {
			notifications[i].gameObject.SetActive (true);
			notifications[i].SetContent (queue[i][0], queue[i][1]);
		}
	}

	void DeactivateNotifications () {
		foreach (UnlockNotification n in notifications) {
			if (n.gameObject.activeSelf)
				n.OnClose (true);
		}
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnlockNotification : MB {

	public float InDelay { get; set; }
	public float OutDelay { get; set; }

	Text header = null;
	Text Header {
		get {
			if (header == null) {
				header = Transform.GetChild (1).GetComponent<Text> ();
			}
			return header;
		}
	}

	Text body = null;
	Text Body {
		get {
			if (body == null) {
				body = Transform.GetChild (2).GetComponent<Text> ();
			}
			return body;
		}
	}

	RectTransform rectTransform = null;
	RectTransform RectTransform {
		get {
			if (rectTransform == null) {
				rectTransform = GetComponent<RectTransform> ();
			}
			return rectTransform;
		}
	}

	LayoutElement layout = null;
	LayoutElement Layout {
		get {
			if (layout == null) {
				layout = GetComponent<LayoutElement> ();
			}
			return layout;
		}
	}

	Button button = null;
	Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
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
				new Color (0.58f, 0.69f, 0.792f), 
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

	float XPosition {
		get { return RectTransform.pivot.x; }
		set { RectTransform.SetPivotX (value); }
	}

	float Height {
		get { return Layout.preferredHeight; }
		set { Layout.preferredHeight = value; }
	}

	public string CurrentNotification { get; private set; }

	public UnlockNotifications notifications;

	const float scaleTime = 0.25f;
	const float slideTime = 0.33f;
	const float max = 0f;
	const float min = 1020f;
	string[] sfx;

	public void SetContent (string type, string context) {
		CurrentNotification = type;
		Settings s = settings[type];
		Header.text = DataManager.GetUIText (s.Header);
		Header.color = s.Color;
		Body.text = context;
		sfx = s.Sfx;
		XPosition = min;
		Height = -1;
		StartCoroutine (CoSlide (min, max, InDelay,
			() => { Button.interactable = true; }
		));
		Invoke ("DelayClose", 10f);
	}

	public void OnClick () {
		notifications.Click (CurrentNotification);
		OnClose ();
	}

	public void OnClose (bool useDelay=false) {
		CancelInvoke ("DelayClose");
		Button.interactable = false;
		StartCoroutine (CoClose (useDelay));
	}

	void DelayClose () {
		OnClose (true);
	}

	IEnumerator CoClose (bool useDelay) {
		yield return StartCoroutine (CoSlide (max, min, useDelay ? OutDelay : 0f));
		StartCoroutine (CoScale (
			() => gameObject.SetActive (false)
		));
	}

	IEnumerator CoSlide (float from, float to, float delay=0f, System.Action onEnd=null) {
		
		float eTime = 0f;

		if (delay > 0f) {

			while (eTime < delay) {
				eTime += Time.deltaTime;
				yield return null;
			}

			eTime = 0f;

		}

		if (delay < 0.35f && Mathf.Approximately (to, max))
			AudioManager.Sfx.Play (sfx[0], sfx[1]);
	
		while (eTime < slideTime) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / slideTime);
			XPosition = Mathf.Lerp (from, to, progress);
			yield return null;
		}

		if (onEnd != null)
			onEnd ();
	}

	IEnumerator CoScale (System.Action onEnd) {
		
		float eTime = 0f;
		float startHeight = RectTransform.sizeDelta.y;
	
		while (eTime < scaleTime) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / scaleTime);
			Height = Mathf.Lerp (startHeight, 0f, progress);
			yield return null;
		}

		Height = 0f;
		onEnd ();
	}
}

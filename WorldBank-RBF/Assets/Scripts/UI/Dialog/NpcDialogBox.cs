using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
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

	bool IsActive {
		get { return Background.gameObject.activeSelf; }
	}

	public Text header;
	public Text body;
	public Transform contentContainer;
	public List<NpcActionButton> buttons;
	public List<FadeText> fadeTexts;
	public CanvasGroup boxGroup;
	public CanvasGroup contentGroup;
	public Color backColor = Color.white;

	const float fadeTime = 0.1f;
	bool fading = false;

	void Start () {
		SetActive (false);
		contentGroup.alpha = 0f;
	}

	public void Open (string headerContent, string bodyContent, Dictionary<string, UnityAction> choices, bool left) {
		
		bool wasActive = IsActive;
		SetActive (true);

		if (wasActive) {
			FadeOutContent (() => SetContent (headerContent, bodyContent, choices, left ? 0 : 1, left ? 0 : 180));
		} else {
			SetContent (headerContent, bodyContent, choices, left ? 0 : 1, left ? 0 : 180);
			FadeIn (false);
		}
	}

	void SetContent (string headerContent, string bodyContent, Dictionary<string, UnityAction> choices, int siblingIndex, float bgRotation) {
		SetButtons (choices);
		header.text = headerContent;
		body.text = bodyContent;
		contentContainer.SetSiblingIndex (siblingIndex);
		Background.SetLocalEulerAnglesZ (bgRotation);
	}

	public void Close () {
		FadeOut ();
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
		foreach (NpcActionButton b in buttons) {
			b.Button.onClick.RemoveAllListeners ();
			b.gameObject.SetActive (false);
		}
	}

	void AddButton (NpcActionButton button, string content, UnityAction action) {
		bool backButton = content == "Back";
		button.gameObject.SetActive (true);
		button.Text.Text.text = content;
		button.Icon.gameObject.SetActive (!backButton && content != "Learn More");
		button.Color = backButton ? backColor : button.DefaultColor;
		button.Button.onClick.AddListener (action);
		button.FadeIn (0.33f);
	}

	void FadeIn (bool wasActive) {
		if (!wasActive)
			StartCoroutine (CoFade (boxGroup, 0f, 1f, fadeTime));
		StartCoroutine (CoFade (contentGroup, 0f, 1f, fadeTime));
	}

	void FadeOut () {
		StartCoroutine (CoFade (contentGroup, 1f, 0f, fadeTime));
		StartCoroutine (CoFade (boxGroup, 1f, 0f, fadeTime, () => SetActive (false)));
	}

	void FadeOutContent (System.Action midFade) {
		StartCoroutine (CoFade (contentGroup, 1f, 0f, fadeTime, () => {
			midFade ();
			FadeIn (true);
		}));
	}

	IEnumerator CoFade (CanvasGroup group, float from, float to, float time, System.Action onEnd=null) {
		
		while (fading) yield return null;
		fading = true;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			group.alpha = Mathf.Lerp (from, to, progress);
			yield return null;
		}

		if (onEnd != null) onEnd ();
		fading = false;
	}
}

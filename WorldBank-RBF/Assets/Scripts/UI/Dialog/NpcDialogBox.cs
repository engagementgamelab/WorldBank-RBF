using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NpcDialogBox : MB {

	struct Content {

		public readonly string Header;
		public readonly string Body;
		public readonly Dictionary<string, UnityAction> Choices;
		public readonly bool Left;

		public bool Empty {
			get { return Header == "" && Body == "" && Choices == null; }
		}

		public Content (string headerContent, string bodyContent, Dictionary<string, UnityAction> choices, bool left=false) {
			Header = headerContent;
			Body = bodyContent;
			Choices = choices;
			Left = left;
		}
	}

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
	public List<Text> buttonTexts;
	public List<Image> buttonImages;
	public Scrollbar scrollbar;
	public CanvasGroup boxGroup;
	public CanvasGroup contentGroup;

	Color defaultColor = new Color (0.874f, 0.773f, 0.506f);
	Color backColor = Color.white;
	Color unlockColor = new Color (0.58f, 0.69f, 0.792f);

	bool tutorialInteractions = false;
	bool tutorialBack = false;

	readonly Dictionary<string, float> fadeTimes = new Dictionary<string, float> {
		{ "inbox", 0.1f },
		{ "outbox", 0.2f },
		{ "incontent", 0.1f },
		{ "outcontent", 0.2f },
		{ "inbutton", 0.5f },
		{ "outbutton", 0.1f }
	};

	Content currentContent;
	Content emptyContent = new Content ("", "", null);

	void Start () {
		SetActive (false);
	}

	public void Open (string headerContent, string bodyContent, Dictionary<string, UnityAction> choices, bool left) {
		if (IsActive) {
			Content newContent = new Content (headerContent, bodyContent, choices, left);
			if (currentContent.Empty) {
				FadeInContent (newContent);
			} else {
				SwapContent (newContent);
			}
		} else {
			FadeInFromClose (new Content (headerContent, bodyContent, choices, left));
		}
	}

	public void Clear () {
		FadeOutContent ();
	}

	public void Close () {
		FadeOutFromOpen ();
	}

	void ApplyContent (Content content) {
		SetButtons (content.Choices);
		header.text = content.Header;
		body.text = content.Body;
		currentContent = content;
		ResetScroll ();
	}

	void ClearContent () {
		ClearButtons ();
		header.text = "";
		body.text = "";
		currentContent = emptyContent;
		ResetScroll ();
	}

	void FadeInFromClose (Content content) {
		SetActive (true);
		ApplyContent (content);
		SetButtonsAlpha (0f);
		SetButtonsInteractable (false);
		contentGroup.alpha = 0f;
		StartCoroutine (CoFadeInFromClose (() => SetButtonsInteractable (true)));
	}

	void FadeOutFromOpen () {
		SetButtonsInteractable (false);
		StartCoroutine (CoFadeOutFromOpen (() => {
			ClearContent ();
			SetActive (false);
		}));
	}

	void FadeInContent (Content content) {
		ApplyContent (content);
		SetButtonsInteractable (false);
		StartCoroutine (CoFadeInContent (() => SetButtonsInteractable (true)));
	}

	void FadeOutContent () {
		SetButtonsInteractable (false);
		StartCoroutine (CoFadeOutContent (ClearContent));
	}

	void SwapContent (Content newContent) {
		StartCoroutine (CoSwapContent (
			() => {
				ClearContent ();
				ApplyContent (newContent);
				SetButtonsInteractable (false);
			},
			() => SetButtonsInteractable (true)
		));
	}

	IEnumerator CoFadeInFromClose (System.Action onEnd) {
		yield return StartCoroutine (CoFade (boxGroup, 0f, 1f, fadeTimes["inbox"]));
		yield return StartCoroutine (CoFade (contentGroup, 0f, 1f, fadeTimes["incontent"]));
		yield return StartCoroutine (CoFadeInButtons (fadeTimes["inbutton"]));
		onEnd ();
	}

	IEnumerator CoFadeOutFromOpen (System.Action onEnd) {
		yield return StartCoroutine (CoFadeOutButtons (fadeTimes["outbutton"]));
		yield return StartCoroutine (CoFade (contentGroup, 1f, 0f, fadeTimes["outcontent"]));
		yield return StartCoroutine (CoFade (boxGroup, 1f, 0f, fadeTimes["outbox"]));
		onEnd ();
	}

	IEnumerator CoFadeInContent (System.Action onEnd) {
		yield return StartCoroutine (CoFade (contentGroup, 0f, 1f, fadeTimes["incontent"]));
		yield return StartCoroutine (CoFadeInButtons (fadeTimes["inbutton"]));
		if (onEnd != null) onEnd ();
	}

	IEnumerator CoFadeOutContent (System.Action onEnd) {
		yield return StartCoroutine (CoFadeOutButtons (fadeTimes["outbutton"]));
		yield return StartCoroutine (CoFade (contentGroup, 1f, 0f, fadeTimes["outcontent"]));
		if (onEnd != null) onEnd ();
	}

	IEnumerator CoSwapContent (System.Action midFade, System.Action onEnd) {
		yield return StartCoroutine (CoFadeOutContent (null));
		midFade ();
		yield return StartCoroutine (CoFadeInContent (onEnd));
	}

	void SetButtonsInteractable (bool interactable) {
		foreach (NpcActionButton b in buttons) {
			if (b.gameObject.activeSelf) {
				if (DataManager.tutorialEnabled && b.Text.Text.text == "Back" && !tutorialInteractions) {
					b.Button.interactable = false;
				} else {
					b.Button.interactable = interactable;
					Debug.Log (b.Button.interactable);
				}
			}
		}
	}

	void SetButtonsAlpha (float alpha) {
		foreach (NpcActionButton b in buttons)
			b.Alpha = alpha;
	}

	IEnumerator CoFadeInButtons (float time) {
		foreach (NpcActionButton b in buttons) {
			if (b.gameObject.activeSelf) {
				yield return StartCoroutine (b.CoFade (0f, 1f, time));
			}
		}		
	}

	IEnumerator CoFadeOutButtons (float time) {
		for (int i = buttons.Count-1; i > -1; i --) {
			NpcActionButton b = buttons[i];
			if (b.gameObject.activeSelf) {
				yield return StartCoroutine (b.CoFade (1f, 0f, time));
			}
		}		
	}
	
	void SetActive (bool active) {
		Background.gameObject.SetActive (active);
		Panel.gameObject.SetActive (active);
		if (!active)
			ClearContent ();
	}

	void SetButtons (Dictionary<string, UnityAction> choices) {
		ClearButtons ();
		int index = 0;
		foreach (var choice in choices) {
			if (choice.Key == "Back") continue;
			AddButton (buttons[index], choice.Key, choice.Value, index);
			index ++;
		}
		UnityAction backAction;
		if (choices.TryGetValue ("Back", out backAction)) {
			AddButton (buttons[index], "Back", backAction, index);
		}
	}

	void ClearButtons () {
		foreach (NpcActionButton b in buttons) {
			b.Button.onClick.RemoveAllListeners ();
			b.gameObject.SetActive (false);
		}
	}

	void AddButton (NpcActionButton button, string content, UnityAction action, int index) {
		
		bool backButton = content == "Back";
		button.gameObject.SetActive (true);
		button.Text.Text.text = content.Replace ("~", "");
		button.Icon.gameObject.SetActive (!backButton && content != "Learn More");
		buttonTexts[index].color = backButton ? backColor : defaultColor;
		buttonImages[index].color = backButton ? backColor : defaultColor;
		
		button.Button.onClick.AddListener (action);
		button.Button.onClick.AddListener (() => { SetButtonsInteractable (false); });

		if (DataManager.tutorialEnabled) {
			if (!tutorialInteractions) {
				if (backButton) {
					button.Button.interactable = false;
					return;
				} else if (content != "Learn More") {
					button.Button.onClick.AddListener (() => {
						DialogManager.instance.CreateTutorialScreen("phase_1_interactions_counter");
						tutorialInteractions = true;
					});
				}
			} else if (!tutorialBack) {
				if (backButton) {
					button.Button.onClick.AddListener (() => {
						DialogManager.instance.CreateTutorialScreen("phase_1_move_around");
						tutorialBack = true;
					});
				}
			}
		}

		if (content.Contains ("~")) {
			buttonTexts[index].color = unlockColor;
			buttonImages[index].color = unlockColor;
		}

		button.Button.interactable = true;
	}

	void ResetScroll () {
		scrollbar.value = 1f;
	}

	IEnumerator CoFade (CanvasGroup group, float from, float to, float time, System.Action onEnd=null) {
		
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			group.alpha = Mathf.Lerp (from, to, progress);
			yield return null;
		}

		if (onEnd != null) onEnd ();
	}
}

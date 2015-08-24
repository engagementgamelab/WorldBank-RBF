using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatScreen : GenericDialogBox {

	public Transform messagesContainer;
	public Scrollbar messagesScrollbar;
	public LayoutElement rightPanel;
	public Animator advisorsPanel;
    public bool rightPanelActive = true;

	public List<ScenarioOptionButton> btnListOptions;
	public List<GameObject> spacers;

	protected bool panelOpen = false;

	protected virtual void OnEnable () {
		rightPanel.gameObject.SetActive (rightPanelActive);
		if (rightPanelActive && !panelOpen) {
			advisorsPanel.Play ("Opened");
			panelOpen = true;
		}
	}

	// TODO: This seems to break if there are more than two buttons
	// Also -- this should be made more generic instead of having 3 versions of the same method
	public virtual void AddOptions(List<string> btnContent, List<UnityAction> btnAction) {

		if (btnContent.Count != btnAction.Count)
			throw new System.Exception ("Button content count must match the button action count");

		RemoveOptions ();
		int btnIndex = 0;

		foreach (string content in btnContent) {
			
			ScenarioOptionButton btnChoice = btnListOptions[btnIndex];
			btnChoice.gameObject.SetActive (true);
			UnityAction action = btnAction[btnIndex];
			btnIndex ++;

			btnChoice.Text = content;
			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (action);
		}

		SetSpacerActiveState (btnContent.Count);
	}

	public virtual void AddOptions(List<string> btnContent, List<string> optionIds) {

		if (btnContent.Count != optionIds.Count)
			throw new System.Exception ("Button content count must match the button action count");

		RemoveOptions ();
		int btnIndex = 0;

		foreach (string content in btnContent) {

			ScenarioOptionButton btnChoice = btnListOptions[btnIndex];
			btnChoice.gameObject.SetActive (true);
			string op = optionIds[btnIndex];
			btnIndex ++;

			btnChoice.Text = content;
			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => OptionSelected (op));
		}

		SetSpacerActiveState (btnContent.Count);
	}

	public virtual void AddOptions(List<string> currentCardOptions) {

		RemoveOptions ();
		int btnIndex = 0;

		foreach(string option in currentCardOptions) {

			ScenarioOptionButton btnChoice = btnListOptions[btnIndex];
			btnChoice.gameObject.SetActive (true);
			btnIndex ++;

			btnChoice.Text = DataManager.GetUnlockableBySymbol(option).title;
			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => OptionSelected(option));
		}

		SetSpacerActiveState (currentCardOptions.Count);
	}

	public void AddYearEndOptions (Dictionary<string, string>[] options) {

		if (options.Length > 3)
			throw new System.Exception ("Only 3 year-end options can be displayed on the screen at a time.");

		RemoveOptions ();
		int btnIndex = 0;

		foreach (Dictionary<string, string> option in options) {
			
			string optionTxt = option["text"];
			string optionVal = option["load"];

			ScenarioOptionButton btnChoice = btnListOptions[btnIndex];
			btnChoice.gameObject.SetActive (true);
			btnIndex ++;

			btnChoice.Text = optionTxt;
			btnChoice.Button.onClick.RemoveAllListeners ();
			btnChoice.Button.onClick.AddListener (() => YearEndOptionSelected (optionTxt, optionVal));
		}

		ScenarioOptionButton btnNextYear = btnListOptions[btnIndex];
		btnNextYear.gameObject.SetActive (true);
		btnNextYear.Text = "Go to next year";
		btnNextYear.Button.onClick.RemoveAllListeners ();
		btnNextYear.Button.onClick.AddListener(() => Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT_YEAR)));

		SetSpacerActiveState (options.Length+1);
	}

	// Scenario option was selected
	protected virtual void OptionSelected(string strOptionSymbol) {

		// Broadcast to open next card
		Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT, strOptionSymbol));
	}

	void YearEndOptionSelected (string optionTxt, string optionVal) {

		// Update selected decisions
		DataManager.ScenarioDecisions(optionTxt);

		// Broadcast to affect current scenario path with the config value
		Events.instance.Raise(new ScenarioEvent(ScenarioEvent.DECISION_SELECTED, optionVal));
		Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT_YEAR));
	}

	protected void RemoveOptions () {
		foreach (ScenarioOptionButton btn in btnListOptions) {
			btn.gameObject.SetActive (false);
		}
	}

	protected void AddResponseSpeech(string strDialogue, Models.Character npc) {
		AdvisorMessage response = ObjectPool.Instantiate<AdvisorMessage>("Scenario");
		response.NPCName = npc.display_name;
		response.Content = strDialogue;
		response.NPCSymbol = npc.symbol;
		response.transform.SetParent(messagesContainer);
		response.transform.localScale = Vector3.one;
		if (gameObject.activeSelf)
			StartCoroutine (CoScrollToEnd ());
	}

	protected void AddSystemMessage (string content) {
		SystemMessage message = ObjectPool.Instantiate<SystemMessage>("Scenario");
		message.Content = content;
		message.transform.SetParent(messagesContainer);
		message.transform.localScale = Vector3.one;
		if (gameObject.activeSelf)
			StartCoroutine (CoScrollToEnd ());
	}

	void SetSpacerActiveState (int buttonCount) {
		spacers[0].SetActive (buttonCount <= 2);
		spacers[1].SetActive (buttonCount <= 3);
	}

	IEnumerator CoScrollToEnd () {
		
		// WHY 2 frames unity? why??
		yield return new WaitForFixedUpdate ();
		yield return new WaitForFixedUpdate ();

		float startValue = messagesScrollbar.value;
		float time = 0.5f;
		float eTime = 0f;

		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			messagesScrollbar.value = Mathf.Lerp (startValue, 0, progress);
			yield return null;
		}
	}
}

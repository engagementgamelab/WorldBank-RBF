using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatScreen : GenericDialogBox {

	public Transform messagesContainer;
	public Scrollbar messagesScrollbar;

	public List<ScenarioOptionButton> btnListOptions;
	public List<GameObject> spacers;

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

		if (currentCardOptions.Count > 2)
			spacers[0].SetActive (false);
		if (currentCardOptions.Count > 3)
			spacers[1].SetActive (false);
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

		if (options.Length+1 > 2)
			spacers[0].SetActive (false);
		if (options.Length+1 > 3)
			spacers[1].SetActive (false);
	}

	// Scenario option was selected
	void OptionSelected(string strOptionSymbol) {

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
		foreach (GameObject spacer in spacers) {
			spacer.SetActive (true);
		}
	}
}

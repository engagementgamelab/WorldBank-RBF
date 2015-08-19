using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatScreen : GenericDialogBox {

	public List<ScenarioOptionButton> btnListOptions;

	public virtual void AddOptions(Models.ScenarioCard _data, List<string> currentCardOptions) {

		RemoveOptions ();

		int btnIndex = 0;

		foreach(string option in currentCardOptions) {

			ScenarioOptionButton btnChoice = btnListOptions[btnIndex];
			btnChoice.gameObject.SetActive (true);
			btnIndex ++;

			btnChoice.Text = DataManager.GetUnlockableBySymbol(option).title;

			btnChoice.Button.onClick.RemoveAllListeners();
			
			if(option == "Back")
				btnChoice.Button.onClick.AddListener (() => DialogManager.instance.CreateScenarioDialog(_data));
			else
				btnChoice.Button.onClick.AddListener (() => OptionSelected(option));

		}
	}

	// Scenario option was selected
	void OptionSelected(string strOptionSymbol) {

		// Broadcast to open next card
		Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT, strOptionSymbol));

		// Close ();
	}

	protected void RemoveOptions () {
		foreach (ScenarioOptionButton btn in btnListOptions) {
			btn.gameObject.SetActive (false);
		}
	}
}

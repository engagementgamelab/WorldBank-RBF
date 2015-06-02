/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 ScenarioCardDialog.cs
 Scenario dialog UI logic/rendering.

 Created by Johnny Richardson on 5/11/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScenarioCardDialog : GenericDialogBox {

	public Models.ScenarioCard data;

	public List<GenericButton> btnListOptions = new List<GenericButton>();
	private List<GenericButton> btnListAdvisors = new List<GenericButton>();

	public void AddAdvisors(List<string> advisors) {

		RemoveButtons();

		// Create buttons for all advisors
		foreach(string characterSymbol in advisors) {

			// Show an advisor option only if they have dialogue (not for feedback only)
			if(!data.characters[characterSymbol].hasDialogue)
				continue;

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();

			Models.Character charRef = DataManager.GetDataForCharacter(characterSymbol);
			
			btnChoice.Text = charRef.display_name;

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => DialogManager.instance.CreateScenarioDialog(data, charRef.symbol));

			btnChoice.gameObject.SetActive(true);
			btnListAdvisors.Add(btnChoice);
		}
		
		AddButtons(btnListAdvisors);

	}

	public virtual void AddOptions(List<string> options) {
	
		RemoveButtons(true);

		foreach(string option in options) {

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
			
			btnChoice.Text = "Option: " + option;

			btnChoice.Button.onClick.RemoveAllListeners();
			
			if(option == "Back")
				btnChoice.Button.onClick.AddListener (() => DialogManager.instance.CreateScenarioDialog(data));
			else
				btnChoice.Button.onClick.AddListener (() => Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT)));
				// ScenarioManager.GetNextCard()

			btnListOptions.Add(btnChoice);
		}

		AddButtons(btnListOptions, true);
	}
    
}
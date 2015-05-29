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

	public Models.ScenarioCard card;

	private List<GenericButton> btnListAdvisors = new List<GenericButton>();
	private List<GenericButton> btnListOptions = new List<GenericButton>();

	public void AddAdvisors(List<string> advisors) {

		// Create buttons for all advisors
		foreach(string characterSymbol in advisors) {

			// Show an advisor option only if they have dialogue (not for feedback only)
			if(!card.characters[characterSymbol].hasDialogue)
				continue;

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();

			Models.Character charRef = DataManager.GetDataForCharacter(characterSymbol);
			
			btnChoice.Text = charRef.display_name;

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => DialogManager.instance.CreateScenarioDialog(card, charRef.symbol));

			btnChoice.gameObject.SetActive(true);
			btnListAdvisors.Add(btnChoice);
		}
		
		AddButtons(btnListAdvisors);

	}

	public void AddOptions(List<string> options) {
	
		foreach(string option in options) {

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
			
			btnChoice.Text = "Option: " + option;

			btnChoice.Button.onClick.RemoveAllListeners();
			
			if(option == "Back")
				btnChoice.Button.onClick.AddListener (() => DialogManager.instance.CreateScenarioDialog(card));
			else
				btnChoice.Button.onClick.AddListener (() => ScenarioManager.GetNextCard());

			btnListOptions.Add(btnChoice);
		}

		AddButtons(btnListOptions, true);
	}
    
}
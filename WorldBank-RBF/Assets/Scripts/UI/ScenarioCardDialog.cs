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

	public Transform conferencePanel;
	public Transform conferenceButtonGroup;
	public Animator conferenceAnimator;

	private List<GenericButton> btnListAdvisors = new List<GenericButton>();
	

	public void AddAdvisors(List<string> advisors) {

		// RemoveButtons<GenericButton>();

		// Create buttons for all advisors
		foreach(string characterSymbol in advisors) {

			// Show an advisor option only if they have dialogue (not for feedback only)
			if(!data.characters[characterSymbol].hasDialogue)
				continue;

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();

			Models.Character charRef = DataManager.GetDataForCharacter(characterSymbol);
			
			btnChoice.Text = charRef.display_name;

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => AdvisorSelected(charRef.symbol));

			btnChoice.gameObject.SetActive(true);
			btnListAdvisors.Add(btnChoice);
		}
		
		AddButtons<GenericButton>(btnListAdvisors, false, conferenceButtonGroup);

	}

	public virtual void AddOptions(List<string> options) {

		List<OptionButton> btnListOptions = new List<OptionButton>();
	
		// RemoveButtons<OptionButton>();

		foreach(string option in options) {

			OptionButton btnChoice = ObjectPool.Instantiate<OptionButton>();

			// Debug.Log("Option: " + btnChoice);

			btnChoice.Text = DataManager.GetUnlockableBySymbol(option).title;

			btnChoice.Button.onClick.RemoveAllListeners();
			
			if(option == "Back")
				btnChoice.Button.onClick.AddListener (() => DialogManager.instance.CreateScenarioDialog(data));
			else
				btnChoice.Button.onClick.AddListener (() => Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT)));
				// ScenarioManager.GetNextCard()

			btnListOptions.Add(btnChoice);
		}

		AddButtons<OptionButton>(btnListOptions);
	}

	private void AdvisorSelected(string strAdvisorSymbol) {

		Models.Advisor advisor = data.characters[strAdvisorSymbol];
		
		Content = advisor.dialogue;

		if(advisor.narrowsNpcs)
		{
			foreach(string npc_symbol in advisor.narrows)
				ScenarioManager.currentAdvisorOptions.Remove(npc_symbol);

		}

		if(advisor.unlocks != null)
		{
			foreach(string option in advisor.unlocks)
				ScenarioManager.currentCardOptions.Add(option);
		}

		ScenarioManager.currentAdvisorOptions.Remove(strAdvisorSymbol);

		// Create buttons for all advisors
		AddAdvisors(ScenarioManager.currentAdvisorOptions);

		// Create buttons for all options if not speaking to advisor
		AddOptions(ScenarioManager.currentCardOptions);

		conferenceAnimator.Play("ConferenceHide");
		conferencePanel.GetComponent<CanvasGroup>().interactable = false;
		conferencePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;

	}
    
}
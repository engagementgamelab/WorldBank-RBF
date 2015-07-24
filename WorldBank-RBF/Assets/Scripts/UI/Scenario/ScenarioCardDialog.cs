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
using System.Linq;

public class ScenarioCardDialog : GenericDialogBox {

	Models.ScenarioCard _data;

    /// <summary>
    /// Set the data for this card
    /// </summary>
    public Models.ScenarioCard Data {
        set {
        	_data = value;

        	// Cleanup
        	responseTextPanel.transform.DetachChildren();

			// Generate advisors and starting options
			currentAdvisorOptions = value.characters.Select(x => x.Key).ToList();
			currentCardOptions = new List<string>(value.starting_options);

			allCardOptions = currentCardOptions.Concat(new List<string>(value.final_options)).ToList();
			allCardAffects = new List<string>(value.starting_options_affects).Concat(new List<string>(value.final_options_affects)).ToList();

			// Create buttons for all advisors
			AddAdvisors();

			// Create buttons for all options if not speaking to advisor
			AddOptions();

			AddResponseSpeech(value.initiating_dialogue, DataManager.GetDataForCharacter(value.initiating_npc));

        }
    }

	public Transform conferencePanel;
	public Transform responseTextPanel;
	public Transform choicesGroup;

	List<string> currentAdvisorOptions;
	List<string> currentCardOptions;

	List<string> allCardOptions;
	List<string> allCardAffects;

	List<NPCConferenceButton> btnListAdvisors = new List<NPCConferenceButton>();

	bool rightSideResponse = true;

	public void AddAdvisors() {

		// RemoveButtons<NPCConferenceButton>();

		// Create buttons for all advisors
		foreach(string characterSymbol in currentAdvisorOptions) {

			// Show an advisor option only if they have dialogue (not for feedback only)
			if(!_data.characters[characterSymbol].hasDialogue)
				continue;

			NPCConferenceButton btnChoice = ObjectPool.Instantiate<NPCConferenceButton>();

			Models.Character charRef = DataManager.GetDataForCharacter(characterSymbol);
			
			btnChoice.NPCName = charRef.display_name;

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => AdvisorSelected(charRef.symbol));

			btnChoice.gameObject.SetActive(true);
			btnListAdvisors.Add(btnChoice);
			
		}
		
		AddButtons<NPCConferenceButton>(btnListAdvisors, false, conferencePanel);

	}

	public virtual void AddOptions() {

		List<ScenarioChoiceButton> btnListOptions = new List<ScenarioChoiceButton>();
	
		foreach(string option in currentCardOptions) {

			ScenarioChoiceButton btnChoice = ObjectPool.Instantiate<ScenarioChoiceButton>();

			btnChoice.Text = DataManager.GetUnlockableBySymbol(option).title;

			btnChoice.Button.onClick.RemoveAllListeners();
			
			if(option == "Back")
				btnChoice.Button.onClick.AddListener (() => DialogManager.instance.CreateScenarioDialog(_data));
			else
				btnChoice.Button.onClick.AddListener (() => OptionSelected(option));

			btnListOptions.Add(btnChoice);
		}

		AddButtons<ScenarioChoiceButton>(btnListOptions, false, choicesGroup);
	}

	void AdvisorSelected(string strAdvisorSymbol) {

		Models.Advisor advisor = _data.characters[strAdvisorSymbol];
		if(advisor.narrowsNpcs)
		{
			foreach(string npc_symbol in advisor.narrows)
				currentAdvisorOptions.Remove(npc_symbol);

		}

		if(advisor.unlocks != null)
		{
			foreach(string option in advisor.unlocks)
				currentCardOptions.Add(option);
		}

		if(advisor.dialogue != null)
			AddResponseSpeech(advisor.dialogue, DataManager.GetDataForCharacter(strAdvisorSymbol));

		currentAdvisorOptions.Remove(strAdvisorSymbol);

		// Create buttons for all advisors
		AddAdvisors();

		// Create buttons for all options if not speaking to advisor
		AddOptions();

	}

	void AddResponseSpeech(string strDialogue, Models.Character npc) {

		NPCResponse responseSpeech = ObjectPool.Instantiate<NPCResponse>();

		// Show response portraits/arrows only after initial response
		if(responseTextPanel.transform.childCount > 0) {
			if(rightSideResponse)
				responseSpeech.RightSide = true;
			else
				responseSpeech.LeftSide = true;
		}

		responseSpeech.Content = strDialogue;
		responseSpeech.NPCSymbol = npc.symbol;

		responseSpeech.transform.SetParent(responseTextPanel.transform);

		rightSideResponse = !rightSideResponse;

	}

	// Scenario option was selected
	void OptionSelected(string strOptionSymbol) {

		// Broadcast to open next card
		Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT, strOptionSymbol));

		Close();

	}
    
}
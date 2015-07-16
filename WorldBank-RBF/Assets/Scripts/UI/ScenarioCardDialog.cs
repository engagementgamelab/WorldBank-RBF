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

	private Models.ScenarioCard _data;

    /// <summary>
    /// Set the data for this card
    /// </summary>
    /// <returns>The player's ID.</returns>
    public Models.ScenarioCard Data {
        set {
        	_data = value;

			// Generate advisors and starting options
			currentAdvisorOptions = value.characters.Select(x => x.Key).ToList();
			currentCardOptions = new List<string>(value.starting_options);

			allCardOptions = currentCardOptions.Concat(new List<string>(value.final_options)).ToList();
			allCardAffects = new List<string>(value.starting_options_affects).Concat(new List<string>(value.final_options_affects)).ToList();

			// Create buttons for all advisors
			AddAdvisors();

			// Create buttons for all options if not speaking to advisor
			AddOptions();

        }
    }

	public Transform conferencePanel;
	public Transform conferenceButtonGroup;
	public Animator conferenceAnimator;

	private List<string> currentAdvisorOptions;
	private List<string> currentCardOptions;

	private List<string> allCardOptions;
	private List<string> allCardAffects;

	private List<GenericButton> btnListAdvisors = new List<GenericButton>();
	

	public void AddAdvisors() {

		// RemoveButtons<GenericButton>();

		// Create buttons for all advisors
		foreach(string characterSymbol in currentAdvisorOptions) {

			// Show an advisor option only if they have dialogue (not for feedback only)
			if(!_data.characters[characterSymbol].hasDialogue)
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

	public virtual void AddOptions() {

		List<OptionButton> btnListOptions = new List<OptionButton>();
	
		// RemoveButtons<OptionButton>();

		foreach(string option in currentCardOptions) {

			OptionButton btnChoice = ObjectPool.Instantiate<OptionButton>();

			btnChoice.Text = DataManager.GetUnlockableBySymbol(option).title;

			btnChoice.Button.onClick.RemoveAllListeners();
			
			if(option == "Back")
				btnChoice.Button.onClick.AddListener (() => DialogManager.instance.CreateScenarioDialog(_data));
			else
				btnChoice.Button.onClick.AddListener (() => OptionSelected(option));
				
				// ScenarioManager.GetNextCard()

			btnListOptions.Add(btnChoice);
		}

		AddButtons<OptionButton>(btnListOptions);
	}

	private void AdvisorSelected(string strAdvisorSymbol) {

		Models.Advisor advisor = _data.characters[strAdvisorSymbol];
		
		Content = advisor.dialogue;

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

		currentAdvisorOptions.Remove(strAdvisorSymbol);

		// Create buttons for all advisors
		AddAdvisors();

		// Create buttons for all options if not speaking to advisor
		AddOptions();

		conferenceAnimator.Play("ConferenceHide");
		conferencePanel.GetComponent<CanvasGroup>().interactable = false;
		conferencePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;

	}

	// Scenario option was selected
	private void OptionSelected(string strOptionSymbol) {

		string strAffectSymbol = allCardAffects[allCardOptions.IndexOf(strOptionSymbol)];

		Dictionary<string, int> dictAffect = DataManager.GetIndicatorBySymbol(strAffectSymbol);

		// Update indicators with affect
		NotebookManager.Instance.UpdateIndicators(dictAffect["indicator_1"], dictAffect["indicator_2"], dictAffect["indicator_3"]);

		// Broadcast to open next card
		Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT, strOptionSymbol));

	}
    
}
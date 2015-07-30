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

        	Initialize();

        }
    }

	public Transform upcomingCardsPanel;
	public Transform conferencePanel;
	public Transform responseTextPanel;
	public Transform choicesGroup;

	public Image mainPortraitImage;

	public Text debugText;

	List<string> currentAdvisorOptions;
	List<string> currentCardOptions;

	List<string> allCardOptions;
	List<string> allCardAffects;

	List<NPCConferenceButton> btnListAdvisors = new List<NPCConferenceButton>();

	bool rightSideResponse = true;
	bool showNewQueuedCard;

	void Update() {

		if(showNewQueuedCard) 
		{
			DisplayOtherCards();
			showNewQueuedCard = false;
		}

	}

	public virtual void Initialize() {

    	// Cleanup
    	ObjectPool.DestroyChildren<NPCResponse>(responseTextPanel.transform);

		// Generate advisors and starting options
		currentAdvisorOptions = _data.characters.Select(x => x.Key).ToList();
		currentCardOptions = new List<string>(_data.starting_options);

		allCardOptions = currentCardOptions.Concat(new List<string>(_data.final_options)).ToList();
		allCardAffects = new List<string>(_data.starting_options_affects).Concat(new List<string>(_data.final_options_affects)).ToList();

		// Create buttons for all advisors
		AddAdvisors();

		// Create buttons for all options if not speaking to advisor
		AddOptions();

		AddResponseSpeech(_data.initiating_dialogue, DataManager.GetDataForCharacter(_data.initiating_npc));

		DisplayOtherCards();

		// Listen for ScenarioEvent
		Events.instance.AddListener<ScenarioEvent>(OnScenarioEvent);

		// Load in main portrait
		mainPortraitImage.sprite = Resources.Load<Sprite>("Portraits/PhaseTwo/" + _data.initiating_npc);

		debugText.text = _data.symbol;

	}

	void DisplayOtherCards() {

		ObjectPool.DestroyChildren<PortraitTextBox>(upcomingCardsPanel);

		foreach(Models.ScenarioCard card in ScenarioQueue.Problems)
		{
   			PortraitTextBox btnChoice = ObjectPool.Instantiate<PortraitTextBox>();
   
   			Models.Character charRef = DataManager.GetDataForCharacter(card.initiating_npc);

			btnChoice.Text = card.name;

			btnChoice.transform.SetParent(upcomingCardsPanel.transform);

		}

	}

	public void AddAdvisors() {

		ObjectPool.DestroyChildren<NPCConferenceButton>(conferencePanel);

		// Create buttons for all advisors
		foreach(string characterSymbol in currentAdvisorOptions) {

			// Show an advisor option only if they have dialogue (not for feedback only)
			if(!_data.characters[characterSymbol].hasDialogue)
				continue;

			string npcDialogue = _data.characters[characterSymbol].dialogue;

			NPCConferenceButton btnChoice = ObjectPool.Instantiate<NPCConferenceButton>();

			Models.Character charRef = DataManager.GetDataForCharacter(characterSymbol);
			
			btnChoice.NPCName = charRef.display_name;
			btnChoice.Text = npcDialogue.Substring(0, Mathf.Clamp(80, 0, npcDialogue.Length)) + "...";

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

	public void ToggleConferencePanel() {

		conferencePanel.gameObject.SetActive(!conferencePanel.gameObject.activeSelf);

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

		// Hide panel
		ToggleConferencePanel();

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

    /// <summary>
    // Callback for ScenarioEvent, filtering for type of event
    /// </summary>
    void OnScenarioEvent(ScenarioEvent e) {

    	showNewQueuedCard = true;

    }
    
}
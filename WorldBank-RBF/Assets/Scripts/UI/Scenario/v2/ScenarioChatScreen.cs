using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScenarioChatScreen : ChatScreen {

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

    public Transform advisorsContainer;
	public Transform noMessagesPanel;
    public Text contactsTitleText;
    public Text debugText;

    List<string> previousAdvisorOptions;
    List<string> currentAdvisorOptions;
	List<string> currentCardOptions;

	List<string> allCardOptions;
	List<string> allCardAffects;

	List<AdvisorButton> btnListAdvisors = new List<AdvisorButton>();

	// Player may not use more than 3 advisors per card
	int advisorsUseLimit = 3;
	int advisorsUsed = 0;

 	public override void OnEnable() {

 		base.OnEnable();

		disabledPanel.gameObject.SetActive(false);

 	}

    void Initialize () {

    	// Get initial character info
		Models.Character charRef = DataManager.GetDataForCharacter(_data.initiating_npc);

		// Generate advisors
		previousAdvisorOptions = (previousAdvisorOptions == null)
			? new List<string> ()
				: currentAdvisorOptions.ToList ();
		
		currentAdvisorOptions = _data.characters.Select(x => x.Key).ToList();
		
		AddAdvisors();
		
		// Generate starting options
		currentCardOptions = new List<string>(_data.starting_options);
		allCardOptions = currentCardOptions.Concat(new List<string>(_data.final_options)).ToList();
		allCardAffects = new List<string>(_data.starting_options_affects).Concat(new List<string>(_data.final_options_affects)).ToList();

		// Create buttons for all options if not speaking to advisor
		AddOptions(currentCardOptions, null, true);

		AddResponseSpeech(_data.initiating_dialogue, charRef, true);

		Events.instance.AddListener<ScenarioEvent> (OnScenarioEvent);

		// Reset of advisors used and make advisors container interactable
		advisorsUsed = 0;
		advisorsContainer.GetComponent<CanvasGroup>().interactable = true;
		advisorsContainer.GetComponent<CanvasGroup>().alpha = 1;

		contactsTitleText.text = "Contacts (" + advisorsUsed + "/3)";

    }

    public void EndYear (Models.ScenarioConfig scenarioConfig, List<string> selectedOptions, int currentYear) {

    	bool indicatorsNegative = !DataManager.IsIndicatorDeltaGood(
														    		IndicatorsCanvas.AppliedAffects[0], 
														    		IndicatorsCanvas.AppliedAffects[IndicatorsCanvas.AppliedAffects.Count-1]
														    	  );

    	string strActionsSummary = "<i>Your actions for this year:</i>\n";
    	string[] yearEndPrompts = (currentYear == 1) ? scenarioConfig.prompt_year_1 : scenarioConfig.prompt_year_2;
    	string yearEndMessage;

    	// If player has not made any changes, choose first prompt
    	if(selectedOptions.Count == 0)
    		yearEndMessage = yearEndPrompts[0];

    	// If player has made changes, choose prompt based on if indicators are positive
    	else
    		yearEndMessage = yearEndPrompts[Convert.ToInt32(indicatorsNegative)+1];

    	// Open indicators action for system button
		ChatAction openIndicators = new ChatAction();
		openIndicators.action = (() => Events.instance.Raise(new ScenarioEvent(ScenarioEvent.OPEN_INDICATORS)));
    	
    	if (panelOpen) {
	    	advisorsPanel.Play ("Closed");
    		panelOpen = false;
    	}

    	AddSystemMessage (yearEndMessage);

    	if(selectedOptions.Count > 0) {
	    	foreach (string opt in selectedOptions)
	    		strActionsSummary += opt + "\n";
	    }
	    else
		    strActionsSummary = "<i><b>You did not take any actions this year!</b></i>";
    	
    	AddSystemMessage (strActionsSummary);

    	AddSystemButtons (new List<string>() {"View your indicators"}, new List<ChatAction>() { openIndicators });

    	string currentChoicesConcat = string.Join(" ", DataManager.ScenarioDecisions().ToArray());
		Dictionary<string, string>[] choiceData = scenarioConfig.choices.Where(choice => !currentChoicesConcat.Contains(choice["text"])).ToArray();

		AddYearEndOptions (choiceData);
    }

    public void AddAdvisors() {

    	List<string> removeAdvisors = previousAdvisorOptions
    		.Except (currentAdvisorOptions).ToList ();

    	List<string> newAdvisors = currentAdvisorOptions
			.Except (previousAdvisorOptions).ToList ();

    	foreach (string characterSymbol in removeAdvisors) {
    		
    		Models.Character charRef = DataManager.GetDataForCharacter(characterSymbol);
    		
    		AdvisorButton btnChoice = btnListAdvisors.FirstOrDefault (x => x.NPCName == charRef.display_name);

    		btnListAdvisors.Remove (btnChoice);
    		btnChoice.Hide ();
    	}

    	foreach (string characterSymbol in newAdvisors) {
    		
    		// Show an advisor option only if they have dialogue (not for feedback only)
			if(!_data.characters[characterSymbol].hasDialogue) {
				currentAdvisorOptions.Remove (characterSymbol);
				continue;
			}

			string npcDialogue = _data.characters[characterSymbol].dialogue;

			AdvisorButton btnChoice = ObjectPool.Instantiate<AdvisorButton>("Scenario");
			btnChoice.Show ();

			Models.Character charRef = DataManager.GetDataForCharacter(characterSymbol);
			
			btnChoice.NPCName = charRef.display_name;
			btnChoice.NPCSymbol = charRef.symbol;

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => AdvisorSelected(charRef.symbol));

			btnChoice.gameObject.SetActive(true);
			btnListAdvisors.Add(btnChoice);

			AddButton<AdvisorButton> (btnChoice, advisorsContainer);
    	}
	}

	void AdvisorSelected(string strAdvisorSymbol) {

		previousAdvisorOptions = currentAdvisorOptions.ToList ();

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

		if(advisor.dialogue != null) {
			AddResponseSpeech(advisor.dialogue, DataManager.GetDataForCharacter(strAdvisorSymbol));

			// SFX
			AudioManager.Sfx.Play ("recievemessage", "Phase2");
		}

		currentAdvisorOptions.Remove(strAdvisorSymbol);

		// Create buttons for all advisors
		AddAdvisors();

		// Create buttons for all options if not speaking to advisor
		AddOptions(currentCardOptions);

		advisorsUsed++;

		contactsTitleText.text = "Contacts (" + advisorsUsed + "/3)";

		// Disable advisor container if player used their limit for this card
		if(advisorsUsed == advisorsUseLimit) {
			advisorsContainer.GetComponent<CanvasGroup>().interactable = false;
			advisorsContainer.GetComponent<CanvasGroup>().alpha = .4f;
		}

		// SFX
		AudioManager.Sfx.Play ("addtodiscussion", "Phase2");
	}

	void Clear () {
    	ObjectPool.DestroyChildren<AdvisorMessage>(messagesContainer);
    	ObjectPool.DestroyChildren<SystemMessage>(messagesContainer);
    	RemoveOptions ();
	}

	void OnScenarioEvent (ScenarioEvent e) {

		if(e.eventType == "feedback") {

			KeyValuePair<string, Models.Advisor> npc = _data.characters.Where(d => d.Value.hasFeedback && d.Value.feedback.ContainsKey(e.eventSymbol)).
								 ToDictionary(d => d.Key, d => d.Value).FirstOrDefault();

			if(!npc.Equals(null)) {

				ChatAction nextCardAction = new ChatAction();

				UnityAction nextCard = (() => Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT, e.eventSymbol)));
				nextCardAction.action = nextCard;

				RemoveOptions();
				AddOptions (
					new List<string> { "Next Problem" },
					new List<ChatAction> { nextCardAction }
				);

				AddResponseSpeech(npc.Value.feedback[e.eventSymbol].ToString(), 
								  DataManager.GetDataForCharacter(npc.Key));
			}
			else {
				// Broadcast to open next card
				Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT, e.eventSymbol));
			}

		}
		else if (e.eventType == "next_year" && !panelOpen) {

			advisorsPanel.Play ("Opened");
			panelOpen = true;
		
		}

	}
}

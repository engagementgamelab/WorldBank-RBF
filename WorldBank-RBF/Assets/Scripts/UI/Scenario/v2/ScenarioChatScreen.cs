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

		if (gameObject.activeSelf)
			AddResponseSpeech(_data.initiating_dialogue, charRef, true);

		Events.instance.AddListener<ScenarioEvent> (OnScenarioEvent);

		// Reset of advisors used and make advisors container interactable
		advisorsUsed = 0;
		advisorsContainer.GetComponent<CanvasGroup>().interactable = true;
		advisorsContainer.GetComponent<CanvasGroup>().alpha = 1;

		contactsTitleText.text = "Contacts (" + advisorsUsed + "/3)";

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
			btnChoice.Button.onClick.AddListener (() => StartCoroutine("AdvisorSelected", charRef.symbol));

			btnChoice.gameObject.SetActive(true);
			btnListAdvisors.Add(btnChoice);

			AddButton<AdvisorButton> (btnChoice, advisorsContainer);
    	}
	}

	public void Clear () {

    	ObjectPool.DestroyChildren<ScenarioChatMessage>(messagesContainer, "Scenario");
    	ObjectPool.DestroyChildren<SystemMessage>(messagesContainer, "Scenario");
    	ObjectPool.DestroyChildren<IndicatorsMessage>(messagesContainer, "Scenario");

    	RemoveOptions ();

    	// Disable advisors
		advisorsContainer.GetComponent<CanvasGroup>().interactable = false;
		advisorsContainer.GetComponent<CanvasGroup>().alpha = .4f;

	}

	public void NoMessages() {
		
		Clear();

		AddSystemMessage("No messages.");

	}

	IEnumerator AdvisorSelected(string strAdvisorSymbol) {
	    		
		AddSystemMessage("...");

		yield return new WaitForSeconds(1);

		ObjectPool.Destroy<SystemMessage>(messagesContainer.transform.GetChild(messagesContainer.transform.childCount-1));

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

		if(advisor.dialogue != null && gameObject.activeSelf) {
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

		// Tutorial
		DialogManager.instance.CreateTutorialScreen("phase_2_conference_action");
	}

	void AddIndicatorsMessage(Dictionary<string, int> dictAffect) {

		IndicatorsMessage message = ObjectPool.Instantiate<IndicatorsMessage>("Scenario");
		message.Display(dictAffect);
		
		message.transform.SetParent(messagesContainer);
		message.transform.localScale = Vector3.one;

	}

	IEnumerator ShowFeedback(string eventSymbol)
	{

	    DialogManager.instance.RemoveTutorialScreen();
	    
		yield return new WaitForSeconds(1f);
			
		Clear();

		AddSystemMessage("Waiting for feedback...");

		yield return new WaitForSeconds(3f);
			
		Clear();

		KeyValuePair<string, Models.Advisor> npc = _data.characters.Where(d => d.Value.hasFeedback && d.Value.feedback.ContainsKey(eventSymbol)).
							 ToDictionary(d => d.Key, d => d.Value).FirstOrDefault();

		if(!npc.Equals(null)) {

			ChatAction nextCardAction = new ChatAction();

			UnityAction nextCard = (() => Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT, eventSymbol)));
			nextCardAction.action = nextCard;

			RemoveOptions();
			AddOptions (
				new List<string> { "Confirm Feedback" },
				new List<ChatAction> { nextCardAction }
			);

			Debug.Log("feedback: " + npc.Key + " for: " + _data.symbol);

			AddResponseSpeech(npc.Value.feedback[eventSymbol].ToString(), 
							  DataManager.GetDataForCharacter(npc.Key), false, true);

			Dictionary<string, int> dictAffect = DataManager.GetIndicatorBySymbol(eventSymbol);

			AddIndicatorsMessage(dictAffect);

			IndicatorsCanvas.SelectedOptions.Add(DataManager.GetUnlockableBySymbol(eventSymbol).title, dictAffect.Values.ToArray());

			// SFX
			AudioManager.Sfx.Play ("planconfirm", "UI");

			// Tutorial
			DialogManager.instance.CreateTutorialScreen("phase_2_supervisor");
		}
		else {
			// Broadcast to open next card
			Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT, eventSymbol));
		}

	}

	void OnScenarioEvent (ScenarioEvent e) {

		if(e.eventType == "feedback") {

			StartCoroutine("ShowFeedback", e.eventSymbol);

		}
		else if (e.eventType == "next_year" && !panelOpen) {

			advisorsPanel.Play ("Opened");
			panelOpen = true;
		
		}

	}
}

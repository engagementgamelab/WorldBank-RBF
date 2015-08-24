using UnityEngine;
using UnityEngine.UI;
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
    public GameObject advisorsButton;
    public Text debugText;

    List<string> previousAdvisorOptions;
    List<string> currentAdvisorOptions;
	List<string> currentCardOptions;

	List<string> allCardOptions;
	List<string> allCardAffects;

	List<AdvisorButton> btnListAdvisors = new List<AdvisorButton>();

    void Initialize () {

    	// Get initial character info
		Models.Character charRef = DataManager.GetDataForCharacter(_data.initiating_npc);

    	// Clear ();

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
		AddOptions(currentCardOptions);

		AddResponseSpeech(_data.initiating_dialogue, charRef);

		// advisorsButton.SetActive (true);
		Events.instance.AddListener<ScenarioEvent> (OnScenarioEvent);

		debugText.text = _data.symbol;
    }

    public void EndYear (Models.ScenarioConfig scenarioConfig, List<string> selectedOptions, int currentYear) {
    	
    	// Clear ();
    	// advisorsButton.SetActive (false);
    	if (panelOpen) {
	    	advisorsPanel.Play ("Closed");
    		panelOpen = false;
    	}

    	string yearEndMessage = (currentYear == 1) ? scenarioConfig.prompt_year_1 : scenarioConfig.prompt_year_2;
    	AddSystemMessage ("~ This will be a message from the flatulating ministers B) ~\n" + yearEndMessage);

    	string summary = "";
    	foreach (string opt in selectedOptions)
    		summary += opt + "\n";
    	AddSystemMessage (summary);

    	AddSystemMessage ("~ This will be a button B) ~\nView yr freakin indicators dumbo yuh bibby!!!");

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

		if(advisor.dialogue != null)
			AddResponseSpeech(advisor.dialogue, DataManager.GetDataForCharacter(strAdvisorSymbol));

		currentAdvisorOptions.Remove(strAdvisorSymbol);

		// Create buttons for all advisors
		AddAdvisors();

		// Create buttons for all options if not speaking to advisor
		AddOptions(currentCardOptions);
	}

	void Clear () {
    	ObjectPool.DestroyChildren<AdvisorMessage>(messagesContainer);
    	ObjectPool.DestroyChildren<SystemMessage>(messagesContainer);
    	RemoveOptions ();
	}

	void OnScenarioEvent (ScenarioEvent e) {
		if (e.eventType == "next_year" && !panelOpen) {
			advisorsPanel.Play ("Opened");
			panelOpen = true;
		}
	}
}

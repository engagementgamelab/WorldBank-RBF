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

    public Transform messagesContainer;
    public Transform advisorsContainer;
    public Text debugText;

    List<string> currentAdvisorOptions;
	List<string> currentCardOptions;

	List<string> allCardOptions;
	List<string> allCardAffects;

	List<AdvisorButton> btnListAdvisors = new List<AdvisorButton>();

    void Initialize () {

    	// Get initial character info
		Models.Character charRef = DataManager.GetDataForCharacter(_data.initiating_npc);

    	// Cleanup
    	ObjectPool.DestroyChildren<AdvisorMessage>(messagesContainer);
    	RemoveOptions ();

		// Generate advisors and starting options
		currentAdvisorOptions = _data.characters.Select(x => x.Key).ToList();
		currentCardOptions = new List<string>(_data.starting_options);

		allCardOptions = currentCardOptions.Concat(new List<string>(_data.final_options)).ToList();
		allCardAffects = new List<string>(_data.starting_options_affects).Concat(new List<string>(_data.final_options_affects)).ToList();

		// Create buttons for all advisors
		AddAdvisors();

		// Create buttons for all options if not speaking to advisor
		AddOptions(_data, currentCardOptions);

		AddResponseSpeech(_data.initiating_dialogue, charRef);

		debugText.text = _data.symbol;
    }

    public void AddAdvisors() {

		ObjectPool.DestroyChildren<AdvisorButton>(advisorsContainer);

		// Create buttons for all advisors
		foreach(string characterSymbol in currentAdvisorOptions) {

			// Show an advisor option only if they have dialogue (not for feedback only)
			if(!_data.characters[characterSymbol].hasDialogue)
				continue;

			string npcDialogue = _data.characters[characterSymbol].dialogue;

			AdvisorButton btnChoice = ObjectPool.Instantiate<AdvisorButton>("Scenario");

			Models.Character charRef = DataManager.GetDataForCharacter(characterSymbol);
			
			btnChoice.NPCName = charRef.display_name;
			btnChoice.NPCSymbol = charRef.symbol;

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => AdvisorSelected(charRef.symbol));

			btnChoice.gameObject.SetActive(true);
			btnListAdvisors.Add(btnChoice);
			
		}
		
		AddButtons<AdvisorButton>(btnListAdvisors, false, advisorsContainer);

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
		AddOptions(_data, currentCardOptions);
	}

	void AddResponseSpeech(string strDialogue, Models.Character npc) {
		AdvisorMessage response = ObjectPool.Instantiate<AdvisorMessage>("Scenario");
		response.NPCName = npc.display_name;
		response.Content = strDialogue;
		response.NPCSymbol = npc.symbol;
		response.transform.SetParent(messagesContainer);
	}
}

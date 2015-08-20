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
    public Animator advisorsPanel;
    public GameObject advisorsButton;
    public Text debugText;

    List<string> currentAdvisorOptions;
	List<string> currentCardOptions;

	List<string> allCardOptions;
	List<string> allCardAffects;

	List<AdvisorButton> btnListAdvisors = new List<AdvisorButton>();

    void Initialize () {

    	// Get initial character info
		Models.Character charRef = DataManager.GetDataForCharacter(_data.initiating_npc);

    	Clear ();

		// Generate advisors and starting options
		currentAdvisorOptions = _data.characters.Select(x => x.Key).ToList();
		currentCardOptions = new List<string>(_data.starting_options);

		allCardOptions = currentCardOptions.Concat(new List<string>(_data.final_options)).ToList();
		allCardAffects = new List<string>(_data.starting_options_affects).Concat(new List<string>(_data.final_options_affects)).ToList();

		// Create buttons for all advisors
		AddAdvisors();

		// Create buttons for all options if not speaking to advisor
		AddOptions(currentCardOptions);

		AddResponseSpeech(_data.initiating_dialogue, charRef);

		advisorsButton.SetActive (true);

		debugText.text = _data.symbol;
    }

    public void EndYear (Models.ScenarioConfig scenarioConfig, List<string> selectedOptions, int currentYear) {
    	
    	Clear ();
    	advisorsButton.SetActive (false);
    	advisorsPanel.Play ("Closed");

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
		AddOptions(currentCardOptions);
	}

	void AddResponseSpeech(string strDialogue, Models.Character npc) {
		AdvisorMessage response = ObjectPool.Instantiate<AdvisorMessage>("Scenario");
		response.NPCName = npc.display_name;
		response.Content = strDialogue;
		response.NPCSymbol = npc.symbol;
		response.transform.SetParent(messagesContainer);
		response.transform.localScale = Vector3.one;
		if (gameObject.activeSelf)
			StartCoroutine (CoScrollToEnd ());
	}

	void AddSystemMessage (string content) {
		SystemMessage message = ObjectPool.Instantiate<SystemMessage>("Scenario");
		message.Content = content;
		message.transform.SetParent(messagesContainer);
		message.transform.localScale = Vector3.one;
		if (gameObject.activeSelf)
			StartCoroutine (CoScrollToEnd ());
	}

	IEnumerator CoScrollToEnd () {
		
		// WHY 2 frames unity? why??
		yield return new WaitForFixedUpdate ();
		yield return new WaitForFixedUpdate ();

		float startValue = messagesScrollbar.value;
		float time = 0.5f;
		float eTime = 0f;

		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			messagesScrollbar.value = Mathf.Lerp (startValue, 0, progress);
			yield return null;
		}
	}

	void Clear () {
    	ObjectPool.DestroyChildren<AdvisorMessage>(messagesContainer);
    	ObjectPool.DestroyChildren<SystemMessage>(messagesContainer);
    	RemoveOptions ();
	}
}

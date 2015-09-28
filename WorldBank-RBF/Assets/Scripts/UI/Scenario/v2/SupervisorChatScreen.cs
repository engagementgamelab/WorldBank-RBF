using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class SupervisorChatScreen : ChatScreen {
	
	public Text debugPanelTacticsText;

	TacticCardDialog currentTacticCard;

	static Timers.TimerInstance tacticCardCooldown;
	static Timers.TimerInstance investigateCooldown;

	List<Models.TacticCard> tacticsAvailable;
	List<Models.TacticCard> queuedTactics;

	string tacticState;
	Models.TacticCard investigatingTactic;

	int cardIndex = 0;
	float cooldownTotal = 0;
	float cooldownElapsed = 0;

	bool investigateFurther;
	bool tacticsQueued;

	SystemMessage investigateMsg;

	enum SupervisorState {
		PresentingProblem,
		Investigating,
		PresentingOptions,
		WaitingForProblem
	}

	SupervisorState state = SupervisorState.WaitingForProblem;

	void OnEnable() {

 		rightPanel.gameObject.SetActive(false);

		// Tutorial
		DialogManager.instance.CreateTutorialScreen("phase_2_supervisor_opened");

		if(tacticsQueued) {
			state = SupervisorState.WaitingForProblem;
			ShowTactics();
			tacticsQueued = false;
		}

	}

	void OnDisable() {

 		rightPanel.gameObject.SetActive(true);

	}

	/// <summary>
    /// Get/set
    /// </summary>
    public List<Models.TacticCard> Available {
        set {
            tacticsAvailable = value;
            queuedTactics = value;

            if(!gameObject.activeSelf)
            	tacticsQueued = true;
            else
	            ShowTactics();
        }
    }

    Models.Character supervisor;
    Models.Character Supervisor {
    	get {
    		if (supervisor == null) {
	    		supervisor = DataManager.GetDataForCharacter ("rahb_capitol_city");
    		}
    		return supervisor;
    	}
    }

 	public void Clear() {

 		ObjectPool.DestroyChildren<ScenarioChatMessage>(messagesContainer, "Scenario");

 	}

 	void OpenTacticCard () {

 		if(queuedTactics.Count == 0) {
 			RemoveOptions();
 			AddSystemMessage("No more messages for this year.");

 			return;
 		}

		Models.TacticCard card = null;
		investigateFurther = false;

		card = queuedTactics[cardIndex];

		ChatAction investigate = new ChatAction();
		ChatAction skip = new ChatAction();

		investigate.action = (() => Investigate (card));
		skip.action = SkipCard;

		AddResponseSpeech (card.initiating_dialogue, false, true);
		AddOptions (
			new List<string> () { "Investigate", "View other problems" }, 
			new List<ChatAction> () { investigate, skip },
			true
		);

		debugPanelTacticsText.text = "Tactic Symbol: " + card.symbol;
	}

	void AddCard () {
		
		// Early-out if all tactics have been added
		if (tacticsAvailable.Count == 0) return;
		
		ShowTactics ();
	}

	void ShowTactics () {

		// If supervisor is ready for new problems, open a new card
		if (state == SupervisorState.WaitingForProblem && queuedTactics.Count > 0) {
			cardIndex = 0;
			OpenTacticCard ();
			state = SupervisorState.PresentingProblem;
		}
	}

	void Investigate (Models.TacticCard card) {
		
		float[] cooldownTime = investigateFurther ? card.investigate_further_cooldown : card.investigate_cooldown;

		RemoveOptions ();

		// Remove this tactic from the queue & set it as the tactic currently under investigation
		queuedTactics.Remove (card);
		investigatingTactic = card;
		state = SupervisorState.Investigating;

		investigateMsg = AddSystemMessage ("Investigating");

		investigateCooldown = Timers.Instance.StartTimer(gameObject, cooldownTime);
		investigateCooldown.Symbol = "tactic_results";
		investigateCooldown.onTick += OnCooldownTick;
		investigateCooldown.onEnd += EndInvestigation;

		// Set cooldown total
		cooldownTotal = investigateCooldown.Duration;

	}

	void SkipCard () {

		// Open next card in queue, looping to beginning if at the end of the queue
		cardIndex ++;
		if (cardIndex > queuedTactics.Count-1)
			cardIndex = 0;

		if (queuedTactics.Count > 0)
			OpenTacticCard ();
	}

	void EndInvestigation () {
		
		state = SupervisorState.PresentingOptions;

		List<ChatAction> investigateActions = new List<ChatAction>();

		List<string> optionSymbols = investigatingTactic.new_options.ToList<string>();

		if(investigateFurther)
			optionSymbols.AddRange(investigatingTactic.further_options);

		List<string> optionTitles = optionSymbols.ToList().ConvertAll (x => DataManager.GetUnlockableBySymbol (x).title);

		ChatAction investigate = new ChatAction();
		ChatAction skip = new ChatAction();

		investigate.action = (() => Investigate (investigatingTactic));
		skip.action = SkipCard;

		AddResponseSpeech (investigateFurther ? investigatingTactic.investigate_further_dialogue : investigatingTactic.investigate_dialogue);

		foreach(string option in optionSymbols) {
			string key = option;
			ChatAction resultAction = new ChatAction();

			UnityAction feedback = (() => AddResponseSpeech (investigatingTactic.feedback_dialogue[key], true));
			resultAction.action = feedback;
			investigateActions.Add(resultAction);
		}

		Debug.Log("INVESTIGATE FURTHER: " + (investigatingTactic.further_options != null && !investigateFurther));
		
		if(investigatingTactic.further_options != null && !investigateFurther) {
			optionTitles.Insert(0, "Investigate Further");
			optionTitles.Add("View other problems");

			investigateActions.Insert(0, investigate);
			investigateActions.Add(skip);
		}

		AddOptions (optionTitles, investigateActions, true);

		investigateFurther = true;

		if(tabAnimator.GetComponent<Button>().interactable)
			tabAnimator.Play("ScenarioTabAlert");
	}

	protected override void OptionSelected (string option) {

		ChatAction showTactics = new ChatAction();
		showTactics.action = ShowTactics;

		AddResponseSpeech (investigatingTactic.feedback_dialogue[option], false, false, option);
		state = SupervisorState.WaitingForProblem;
		AddOptions (
			new List<string> () { "OK" },
			new List<ChatAction> () { showTactics }
		);

	}

    void AddResponseSpeech (string message, bool endOfCard=false, bool initial=false, string optionUsed=null) {

    	if(optionUsed != null) {
			Dictionary<string, int> dictAffect = DataManager.GetIndicatorBySymbol(optionUsed);
			IndicatorsCanvas.SelectedOptions.Add(DataManager.GetUnlockableBySymbol(optionUsed).title, dictAffect.Values.ToArray());
		}

    	AddResponseSpeech (message, Supervisor, initial, false);
    	
    	if(endOfCard)
	    	SkipCard();
		
		// SFX
	    if(gameObject.activeSelf)
			AudioManager.Sfx.Play ("assistantresponse", "Phase2");
    }

	/// <summary>
    // Callback for TimerTick, filtering for type of event
    /// </summary>
    void OnCooldownTick(GameEvents.TimerTick e) {

    	if(e.Symbol == "tactic_results")
    	{
    		// Debug.Log(cooldownTotal + " - " + e.SecondsElapsed);
	    	cooldownElapsed = cooldownTotal - e.SecondsElapsed;

    		System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(cooldownElapsed);
    		string currentSecond = timeSpan.Seconds.ToString();

	    	investigateMsg.Content = "Investigating: " + currentSecond + "s";

	    	if(timeSpan.Seconds == 0)
	    		RemoveSystemMessage(investigateMsg);

    	}
    }
}

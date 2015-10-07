using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SupervisorChatScreen : ChatScreen {
	
	public Button scenarioChatTab;
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
		Feedback,
		WaitingForProblem
	}

	SupervisorState state = SupervisorState.WaitingForProblem;
	
	/// <summary>
    /// Get/set
    /// </summary>
    public List<Models.TacticCard> Available {
        set {
            tacticsAvailable = value;
            queuedTactics = value;

            state = SupervisorState.WaitingForProblem;

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

    public void StopInvestigation() {

    	if(investigateCooldown != null && investigateCooldown.Active)
	    	investigateCooldown.Stop();
    	
    }

	void ShowTactics () {

		// If supervisor is ready for new problems, open a new card
		if (state == SupervisorState.WaitingForProblem) {
			if(queuedTactics.Count > 0) {
				cardIndex = 0;
				OpenTacticCard ();
				state = SupervisorState.PresentingProblem;
			}
			else {
				Clear();

		 		if(queuedTactics.Count == 0) {
		 			RemoveOptions();
		 			AddSystemMessage("No more messages for this year.");

		 			return;
		 		}
			}
		}
	}

 	void OpenTacticCard () {
			
		Clear();

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
		
		// SFX
	    if(gameObject.activeSelf)
			AudioManager.Sfx.Play ("fem1greeting", "NPCs");

		debugPanelTacticsText.text = "Tactic Symbol: " + card.symbol;
	}

	void AddCard () {
		
		// Early-out if all tactics have been added
		if (tacticsAvailable.Count == 0) return;
		
		ShowTactics ();
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
		
		// SFX
	    if(gameObject.activeSelf)
			AudioManager.Sfx.Play ("fem1response", "NPCs");

		foreach(string option in optionSymbols) {
			string key = option;
			ChatAction resultAction = new ChatAction();

			UnityAction feedback = (() => OptionSelected(key));
			resultAction.action = feedback;
			investigateActions.Add(resultAction);
		}
		
		if(investigatingTactic.further_options != null && !investigateFurther) {
			optionTitles.Insert(0, "Investigate Further");

			investigateActions.Insert(0, investigate);
		}
		
		optionTitles.Add("View other problems");
		investigateActions.Add(skip);

		AddOptions (optionTitles, investigateActions, true);

		investigateFurther = true;

		if(tabAnimator.GetComponent<Button>().interactable)
			tabAnimator.Play("ScenarioTabAlert");
	}

	protected override void OptionSelected (string option) {

		state = SupervisorState.Feedback;

		StartCoroutine(ShowFeedback(option));

	}

	IEnumerator ShowFeedback(string option, bool nodelay=false) {
			
		Clear();

		// Disable scenario chat
		scenarioChatTab.interactable = false;
		scenarioChatTab.animator.Play("SupervisorTabOff");

		yield return new WaitForSeconds(1);

		AddSystemMessage(DataManager.GetUIText("copy_waiting_for_feedback"));

		yield return new WaitForSeconds(nodelay ? 1 : 3);

		ChatAction showTactics = new ChatAction();
		UnityAction showAction = (() => { state = SupervisorState.WaitingForProblem; ShowTactics(); });
		showTactics.action = showAction;
	
		Clear();

		AddResponseSpeech (investigatingTactic.feedback_dialogue[option], false, false, option);
		
		// SFX
	    if(gameObject.activeSelf)
			AudioManager.Sfx.Play ("fem1response", "NPCs");

		AddOptions (
			new List<string> () { "Confirm feedback" },
			new List<ChatAction> () { showTactics },
			true
		);

		// Enable scenario chat
		scenarioChatTab.interactable = true;
		scenarioChatTab.animator.Play("SupervisorTabOn");
	
	}

    void AddResponseSpeech (string message, bool endOfCard=false, bool initial=false, string optionUsed=null) {

    	if(optionUsed != null) {
			
			Dictionary<string, int> dictAffect = DataManager.GetIndicatorBySymbol(optionUsed);
			
			// Hack that we use to see if these indicators are from the supervisor
			int[] affectVals = dictAffect.Values.ToArray();
			int[] affects = new int[4];
			for (int i = 0; i < affectVals.Length; i ++) {
				affects[i] = affectVals[i];
			}

			IndicatorsCanvas.SelectedOption = new KeyValuePair<string, int[]>(DataManager.GetUnlockableBySymbol(optionUsed).title, affects);
			
			Events.instance.Raise(new ScenarioEvent(ScenarioEvent.AFFECT_USED, optionUsed));

	    	AddResponseSpeech (message, Supervisor, initial, true, dictAffect);
		}
		else
	    	AddResponseSpeech (message, Supervisor, initial, false);
    	
    	if(endOfCard)
	    	SkipCard();
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

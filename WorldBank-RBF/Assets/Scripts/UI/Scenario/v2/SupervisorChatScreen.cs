using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class SupervisorChatScreen : ChatScreen {

	TacticCardDialog currentTacticCard;

	static Timers.TimerInstance tacticCardCooldown;
	static Timers.TimerInstance investigateCooldown;

	List<string> tacticsAvailable;
	List<string> queuedTactics;

	string tacticState;
	Models.TacticCard investigatingTactic;

	int cardIndex = 0;
	float cooldownTotal = 0;
	float cooldownElapsed = 0;

	bool investigateFurther;

	SystemMessage investigateMsg;

	enum SupervisorState {
		PresentingProblem,
		Investigating,
		PresentingOptions,
		WaitingForProblem
	}

	SupervisorState state = SupervisorState.WaitingForProblem;

	/// <summary>
    /// Get/set
    /// </summary>
    public List<string> Available {
        set {
            tacticsAvailable = value;
            queuedTactics = value;

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

	void Awake () {

		// Listen for problem card cooldown tick
		Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);

 	}

 	public override void OnEnable() {

 		base.OnEnable();

		// Disable screen if out of tactics for this year
		disabledPanel.gameObject.SetActive(queuedTactics.Count == 0);

		// Tutorial
		DialogManager.instance.CreateTutorialScreen("phase_2_supervisor_opened");

 	}

 	void OpenTacticCard () {

 		string tacticName = queuedTactics[cardIndex];
		Models.TacticCard card = null;
		investigateFurther = false;

		try {

			card = DataManager.GetTacticCardByName(tacticName);
	
		}
		catch(System.Exception e) {
			
			Debug.LogWarning("Unable to locate a tactic card for '" + tacticName + "'. Removing from available tactics.", this);
			queuedTactics.Remove (tacticName);
			SkipCard ();
			return;

		}

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
	}

	void AddCard () {
		
		// Early-out if all tactics have been added
		if (tacticsAvailable.Count == 0) return;
			
		// Add a random card from the available tactics
		string tactic = tacticsAvailable[Random.Range (0, tacticsAvailable.Count-1)];
		// tacticsAvailable.Remove (tactic);
		// queuedTactics.Add (tactic);

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
		queuedTactics.Remove (card.tactic_name);
		investigatingTactic = card;
		state = SupervisorState.Investigating;

		investigateMsg = AddSystemMessage ("Investigating");

		investigateCooldown = Timers.StartTimer(gameObject, cooldownTime);
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
		// Out of tactics for this year
		else
			disabledPanel.gameObject.SetActive(true);
	}

	void EndInvestigation () {
		
		state = SupervisorState.PresentingOptions;

		List<ChatAction> investigateActions = new List<ChatAction>();

		string[] optionSymbols = investigateFurther ? investigatingTactic.further_options : investigatingTactic.new_options;
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
		
		if(investigatingTactic.further_options != null && !investigateFurther) {
			optionTitles.Insert(0, "Investigate Further");
			optionTitles.Add("View other problems");

			investigateActions.Insert(0, investigate);
			investigateActions.Add(skip);
		}

		AddOptions (optionTitles, investigateActions, true);

		// If we can't investigate more, remove button and show close
		/*if(investigatingTactic.further_options == null) {
			buttonInvestigate.gameObject.SetActive(false);
			buttonObserve.Text = "Close";
		}*/

		investigateFurther = true;

		if(tabAnimator.GetComponent<Button>().interactable)
			tabAnimator.Play("ScenarioTabAlert");
	}

	protected override void OptionSelected (string option) {

		ChatAction showTactics = new ChatAction();
		showTactics.action = ShowTactics;

		AddResponseSpeech (investigatingTactic.feedback_dialogue[option]);
		state = SupervisorState.WaitingForProblem;
		AddOptions (
			new List<string> () { "OK" },
			new List<ChatAction> () { showTactics }
		);

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

    void AddResponseSpeech (string message, bool endOfCard=false, bool initial=false) {
    	AddResponseSpeech (message, Supervisor, initial);

    	if(endOfCard)
	    	SkipCard();
		
		// SFX
	    if(gameObject.activeSelf)
			AudioManager.Sfx.Play ("assistantresponse", "Phase2");
    }
}

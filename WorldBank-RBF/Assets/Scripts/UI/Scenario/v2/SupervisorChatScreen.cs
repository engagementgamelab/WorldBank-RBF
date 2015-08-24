using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;

public class SupervisorChatScreen : ChatScreen {

	TacticCardDialog currentTacticCard;

	static Timers.TimerInstance tacticCardCooldown;
	static Timers.TimerInstance investigateCooldown;

	List<string> tacticsAvailable;
	List<string> queuedTactics;
	float[] tacticCardIntervals = new float[3] {3, 4, 4};

	string tacticState;
	Models.TacticCard investigatingTactic;

	int cardIndex = 0;
	float cooldownTotal = 0;
	float cooldownElapsed = 0;

	bool investigateFurther;

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
            queuedTactics = new List<string> ();
            Initialize();
            AddCard ();
            AddCard ();
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
		RemoveOptions ();
 		Events.instance.AddListener<TacticsEvent>(OnTacticsEvent);

		// Listen for problem card cooldown tick
		Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);
 	}

 	void Initialize() {

 		tacticCardIntervals = DataManager.PhaseTwoConfig.tactic_card_intervals;

		tacticCardCooldown = Timers.StartTimer(this.gameObject, tacticCardIntervals);
		tacticCardCooldown.Symbol = "tactic_open";
		tacticCardCooldown.onEnd += AddCard;

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

		AddResponseSpeech (card.initiating_dialogue);
		AddOptions (
			new List<string> () { "Investigate", "View other problems" }, 
			new List<ChatAction> () { investigate, skip }
		);
	}

	void AddCard () {
		
		// Early-out if all tactics have been added
		if (tacticsAvailable.Count == 0) return;
			
		// Add a random card from the available tactics
		string tactic = tacticsAvailable[Random.Range (0, tacticsAvailable.Count-1)];
		tacticsAvailable.Remove (tactic);
		queuedTactics.Add (tactic);

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

		AddSystemMessage ("Rahb's doing a WILD investigation and will return like in 5 seconds! :D");

		investigateCooldown = Timers.StartTimer(gameObject, cooldownTime);
		investigateCooldown.Symbol = "tactic_results";
		investigateCooldown.onTick += OnCooldownTick;
		investigateCooldown.onEnd += EndInvestigation;

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
		string[] options = investigateFurther ? investigatingTactic.further_options : investigatingTactic.new_options;
		
		List<string> content = options.ToList () 
			.ConvertAll (x => DataManager.GetUnlockableBySymbol (x).title);

		// if(investigatingTactic.further_options != null && !investigateFurther) {
		// 	content.Add("Investigate Further");
		// 	options.Add(() => Investigate (tacticName, card));
		// }

		ChatAction investigate = new ChatAction();
		ChatAction skip = new ChatAction();

		investigate.action = (() => Investigate (investigatingTactic));
		skip.action = SkipCard;

		AddResponseSpeech (investigateFurther ? investigatingTactic.investigate_further_dialogue : investigatingTactic.investigate_dialogue);
		AddOptions (
			new List<string> () { "Investigate", "View other problems" }, 
			new List<ChatAction> () { investigate, skip }
		);

		// If we can't investigate more, remove button and show close
		/*if(investigatingTactic.further_options == null) {
			buttonInvestigate.gameObject.SetActive(false);
			buttonObserve.Text = "Close";
		}*/

		investigateFurther = true;
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
    // Callback for TacticsEvent, filtering for type of event
    /// </summary>
    void OnTacticsEvent(TacticsEvent e) {

    	Debug.Log("OnTacticsEvent: " + e.eventType);

    	switch(e.eventType) {

    		/*case "tactic_open":
    			tacticState = "open";
				AddCard ();
				break;*/

	   		/*case "tactic_results":
    			tacticState = "options";
    			endInvestigate = true;
    			break;

	   		case "tactic_closed":
	   			RemoveCard(e.eventSymbol);
    			break;

    		case "investigate":
    			Investigating(e.cooldown);
    			break;

    		case "investigate_further":
    			Investigating(e.cooldown);
    			break;
*/
    	}

    }

	/// <summary>
    // Callback for TimerTick, filtering for type of event
    /// </summary>
    void OnCooldownTick(GameEvents.TimerTick e) {

    	if(e.Symbol == "tactic_results")
    	{
    		// Debug.Log(cooldownTotal + " - " + e.SecondsElapsed);
	    	cooldownElapsed = cooldownTotal - e.SecondsElapsed;

    	}
    }

    void AddResponseSpeech (string message) {
    	AddResponseSpeech (message, Supervisor);
    }
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;

public class SupervisorChatScreen : ChatScreen {

	TacticCardDialog currentTacticCard;

	List<string> tacticsAvailable;
	List<string> queuedTactics;
	int[] tacticCardIntervals = new int[3] {3, 4, 4};

	string tacticState;
	int cardIndex = 0;
	Models.TacticCard investigatingTactic;

	float cooldownTotal = 0;
	float cooldownElapsed = 0;

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
 		// TODO: Replace w/ new timer
		InvokeRepeating ("AddCard", 0, tacticCardIntervals[0]);
		/*tacticCardIntervals = DataManager.PhaseTwoConfig.tactic_card_intervals;

		if(tacticCardCooldown == null)
			tacticCardCooldown = new TimerUtils.Cooldown();

		tacticCardCooldown.Init(tacticCardIntervals, new TacticsEvent(TacticsEvent.TACTIC_OPEN), "tactic_open");*/

	}

 	void OpenTacticCard () {

 		string tacticName = queuedTactics[cardIndex];
		Models.TacticCard card = null;

		try {

			card = DataManager.GetTacticCardByName(tacticName);
	
		}
		catch(System.Exception e) {
			
			Debug.LogWarning("Unable to locate a tactic card for '" + tacticName + "'. Removing from available tactics.", this);
			queuedTactics.Remove (tacticName);
			SkipCard ();
			return;

		}

		AddResponseSpeech (card.initiating_dialogue);
		AddOptions (
			new List<string> () { "Investigate", "View other problems" }, 
			new List<UnityAction> () { () => Investigate (tacticName, card), SkipCard }
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

	void Investigate (string tacticName, Models.TacticCard card) {
		
		RemoveOptions ();

		// Remove this tactic from the queue & set it as the tactic currently under investigation
		queuedTactics.Remove (tacticName);
		investigatingTactic = card;
		state = SupervisorState.Investigating;

		AddSystemMessage ("Rahb's doing a WILD investigation and will return like in 5 seconds! :D");

		Invoke ("EndInvestigation", 5f);
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
		string[] options = investigatingTactic.new_options;
		List<string> content = options.ToList ()
			.ConvertAll (x => DataManager.GetUnlockableBySymbol (x).title);

		AddResponseSpeech (investigatingTactic.investigate_dialogue);
		AddOptions (content, options.ToList ());
	}

	protected override void OptionSelected (string option) {
		AddResponseSpeech (investigatingTactic.feedback_dialogue[option]);
		state = SupervisorState.WaitingForProblem;
		AddOptions (
			new List<string> () { "OK" },
			new List<UnityAction> () { ShowTactics }
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

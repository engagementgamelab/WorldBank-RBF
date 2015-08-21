using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;

public class SupervisorChatScreen : ChatScreen {

	TacticCardDialog currentTacticCard;

	static TimerUtils.Cooldown tacticCardCooldown;
	static TimerUtils.Cooldown investigateCooldown;

	List<string> tacticsAvailable;
	List<string> queuedTactics;
	int[] tacticCardIntervals = new int[3] {3, 4, 4};

	// TODO: don't use these
	bool openTacticCard;
	bool isInvestigating;
	bool endInvestigate;

	string tacticState;
	int cardIndex = 0;

	int cooldownTotal = 0;
	int cooldownElapsed = 0;

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

	void Awake () {
		RemoveOptions ();
 	}

 	void Start () {

 		Events.instance.AddListener<TacticsEvent>(OnTacticsEvent);

		// Listen for problem card cooldown tick
		Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);
 	}

 	void Initialize() {
		
		tacticCardIntervals = DataManager.PhaseTwoConfig.tactic_card_intervals;

		if(tacticCardCooldown == null)
			tacticCardCooldown = new TimerUtils.Cooldown();

		tacticCardCooldown.Init(tacticCardIntervals, new TacticsEvent(TacticsEvent.TACTIC_OPEN), "tactic_open");

	}

 	void OpenTacticCard (string tacticName) {

		Models.TacticCard card = null;

		try {

			card = DataManager.GetTacticCardByName(tacticName);
	
		}
		catch(System.Exception e) {
			
			Debug.LogWarning("Unable to locate a tactic card for '" + tacticName + "'. Timer restarting.", this);
			
			tacticCardCooldown.Init(tacticCardIntervals, new TacticsEvent(TacticsEvent.TACTIC_OPEN), "tactic_open");

			return;

		}

		Models.Character supervisor = DataManager.GetDataForCharacter ("rahb_capitol_city");
		AddResponseSpeech (card.symbol, supervisor);
	}

	void AddCard () {
		int index = new System.Random().Next(0, tacticsAvailable.Count);//Random.Range (0, tacticsAvailable.Count-1);
		Debug.Log (index);
		string tactic = tacticsAvailable[index];
		tacticsAvailable.Remove (tactic);
		queuedTactics.Add (tactic);
		OpenTacticCard (tactic);
	}

 	void RemoveCard(string strButtonName) {

		int cardIndex = 0;

		/*foreach(TacticChoiceButton child in buttonsPanel.transform.GetComponentsInChildren<TacticChoiceButton>()) {
			if(child.Text == strButtonName)
				break;

			cardIndex++;
		}

		ObjectPool.Destroy<TacticChoiceButton>(buttonsPanel.transform.GetChild(cardIndex).transform);

		if(ScenarioQueue.Tactics.Length == 0) {
			Toggle();
			return;
		}

		OpenTacticCard(0, true);
		HideCardButton(0);*/
	}

	public void Investigating(int[] cooldownTime) {


		// Show cooldown text
		// tooltipDoneImg.gameObject.SetActive(false);
		// tooltipAlertImg.gameObject.SetActive(false);
		// tooltipClockImg.gameObject.SetActive(true);
		// tooltipTxt.gameObject.SetActive(true);

		isInvestigating = true;

		cooldownTotal = cooldownTime[0];

    	if(investigateCooldown == null)
			investigateCooldown = new TimerUtils.Cooldown();
		
	 	investigateCooldown.Init(cooldownTime, new TacticsEvent(TacticsEvent.TACTIC_RESULTS), "tactic_results");
	 	tacticCardCooldown.Pause();

	 	/*cooldownText.gameObject.SetActive(true);	
	 	overlayPanel.gameObject.SetActive(true);
	 	buttonsPanel.gameObject.SetActive(false);*/

	 	// currentTacticCard.Disable();

	}

 	/// <summary>
    // Callback for TacticsEvent, filtering for type of event
    /// </summary>
    void OnTacticsEvent(TacticsEvent e) {

    	Debug.Log("OnTacticsEvent: " + e.eventType);

    	switch(e.eventType) {

    		case "tactic_open":
    			tacticState = "open";
				AddCard ();
				break;

	   		case "tactic_results":
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
}

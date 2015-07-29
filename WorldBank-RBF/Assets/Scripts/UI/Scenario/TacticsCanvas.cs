/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 TacticsCanvas.cs
 Phase two tactics canvas management.

 Created by Johnny Richardson on 7/28/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;

public class TacticsCanvas : MonoBehaviour {

	public Text cooldownText;

	public RectTransform buttonsPanel;
	public RectTransform overlayPanel;
	public RectTransform tacticCardsTooltip;

	public Image tooltipAlertImg;
	public Image tooltipClockImg;
	public Image tooltipDoneImg;

	static TimerUtils.Cooldown tacticCardCooldown;
	static TimerUtils.Cooldown investigateCooldown;

	// ScenarioCardDialog currentScenarioCard;
	TacticCardDialog currentTacticCard;

	static List<string> tacticsAvailable;

	static int[] tacticCardIntervals = new int[3] {4, 3, 3};
	
	bool openTacticCard;

	string tacticState;

	int cooldownTotal = 0;
	int cooldownElapsed = 0;
    
    /// <summary>
    /// Get/set
    /// </summary>
    public static List<string> Available {
        set {
            tacticsAvailable = value;

            Initialize();
        }
    }

	void Start () {

		Events.instance.AddListener<ScenarioEvent>(OnScenarioEvent);

		// Listen for problem card cooldown tick
		Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);

	}

	void Update () {
		
		// If a tactic card has been enqueued, open it
		if(openTacticCard)
		{
			openTacticCard = false;
			OpenTacticCard();
		}

		cooldownText.text = cooldownElapsed + " seconds";
	}

	static void Initialize() {

		if(tacticCardCooldown == null)
			tacticCardCooldown = new TimerUtils.Cooldown();

		tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN));

	}

    /// <summary>
    /// Tries to open either a tactic card dialog, and if tactic card not found, restarts tactic card timer.
    /// </summary>
	void OpenTacticCard() {

		if(tacticState == "open")
		{
			Models.TacticCard card = null;		
			int tacticIndex = new System.Random().Next(0, tacticsAvailable.Count);

			try {
		
				card = DataManager.GetTacticCardByName(tacticsAvailable[tacticIndex]);
		
			}
			catch(System.Exception e) {
				
				Debug.LogWarning("Unable to locate a tactic card for '" + tacticsAvailable[tacticIndex] + "'. Timer restarting.", this);
				
				tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN), "tactic_open");

				return;

			}

			ScenarioQueue.AddTacticCard(card);

			AddCardButton(card);

			tacticsAvailable.Remove(tacticsAvailable[tacticIndex]);
			
			if(tacticsAvailable.Count == 0)
				tacticCardCooldown.Stop();
			else
				tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN), "tactic_open");

			if(currentTacticCard != null)
				return;

		 	TacticCardDialog dialog = DialogManager.instance.CreateTacticDialog(card);
		 	dialog.transform.SetParent(transform);


		 	currentTacticCard = dialog;

			// coverFlowHelper.AddTransform(dialog.transform);

			tacticCardsTooltip.gameObject.SetActive(true);


		}
		else 
		{
			// Show tactic card
			currentTacticCard.Enable();

			currentTacticCard.GetResultOptions();

			tacticCardCooldown.Resume();

		 	cooldownText.gameObject.SetActive(false);	
		 	overlayPanel.gameObject.SetActive(false);
		 	buttonsPanel.gameObject.SetActive(true);
		}

		// Pause tactic card cooldown
		// tacticCardCooldown.Pause();
	
	}

	void AddCardButton(Models.TacticCard card) {

		GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
		
		btnChoice.Text = card.name;

		btnChoice.Button.onClick.RemoveAllListeners();
		// btnChoice.Button.onClick.AddListener (() => GetFeedback(option));

		btnChoice.transform.SetParent(buttonsPanel.transform);

	}

	bool QueueTacticCard() {

		openTacticCard = true;

		return true;
	}

	public static void Stop() {
	
		// Pause tactic card cooldown
		tacticCardCooldown.Stop();

	}

	public void Investigating(int[] cooldownTime) {


		// Show cooldown text
		// tooltipDoneImg.gameObject.SetActive(false);
		// tooltipAlertImg.gameObject.SetActive(false);
		// tooltipClockImg.gameObject.SetActive(true);
		// tooltipTxt.gameObject.SetActive(true);

		cooldownTotal = cooldownTime[0];

    	if(investigateCooldown == null)
			investigateCooldown = new TimerUtils.Cooldown();
		
	 	investigateCooldown.Init(cooldownTime, new ScenarioEvent(ScenarioEvent.TACTIC_RESULTS), "tactic_results");
	 	tacticCardCooldown.Pause();

	 	cooldownText.gameObject.SetActive(true);	
	 	overlayPanel.gameObject.SetActive(true);
	 	buttonsPanel.gameObject.SetActive(false);

	 	currentTacticCard.Disable();

	}

    /// <summary>
    // Callback for ScenarioEvent, filtering for type of event
    /// </summary>
    void OnScenarioEvent(ScenarioEvent e) {

    	Debug.Log("OnScenarioEvent: " + e.eventType);

    	switch(e.eventType) {

    		case "tactic_open":
    			tacticState = "open";
				QueueTacticCard();
				break;

	   		case "tactic_results":
    			tacticState = "options";
				QueueTacticCard();
    			break;

	   		case "tactic_closed":
	   			tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN));
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
	    	cooldownElapsed = cooldownTotal - e.SecondsElapsed;

    }
}

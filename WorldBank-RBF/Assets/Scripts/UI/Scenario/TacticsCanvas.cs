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
	static List<string> allTactics;

	static int[] tacticCardIntervals = new int[3] {4, 3, 3};
	
	CanvasGroup canvasGroup;

	bool openTacticCard;
	bool endInvestigate;

	string tacticState;

	int cooldownTotal = 0;
	int cooldownElapsed = 0;
    
    /// <summary>
    /// Get/set
    /// </summary>
    public static List<string> Available {
        set {
            tacticsAvailable = value;
            allTactics = value;

            Initialize();
        }
    }

	void Start () {

		Events.instance.AddListener<ScenarioEvent>(OnScenarioEvent);

		// Listen for problem card cooldown tick
		Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);

		gameObject.SetActive(true);

		canvasGroup = gameObject.GetComponent<CanvasGroup>();
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0;

	}

	void Update () {
		
		// If a tactic card has been enqueued, open it
		if(openTacticCard)
		{
			openTacticCard = false;
			GetNextCard();
		}
		else if(endInvestigate)
		{
			endInvestigate = false;
			EndInvestigate();
		}

		cooldownText.text = cooldownElapsed + " seconds";
	}

	static void Initialize() {

		if(tacticCardCooldown == null)
			tacticCardCooldown = new TimerUtils.Cooldown();

		tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN));

	}

	void GetNextCard() {
	
		int tacticIndex = new System.Random().Next(0, tacticsAvailable.Count);

		OpenTacticCard(tacticIndex);

	}

    /// <summary>
    /// Tries to open either a tactic card dialog, and if tactic card not found, restarts tactic card timer.
    /// </summary>
    /// <param name="cardIndex">The index of the card.</param>
	void OpenTacticCard(int cardIndex, bool replace=false) {

		TacticCardDialog dialog = null;

		if(tacticState == "open")
		{

			if(!replace) {
				
				Models.TacticCard card = null;
				string tacticName = null;

				try {

			 		tacticName = tacticsAvailable[cardIndex];
					card = DataManager.GetTacticCardByName(tacticName);
			
				}
				catch(System.Exception e) {
					
					Debug.LogWarning("Unable to locate a tactic card for '" + tacticName + "'. Timer restarting.", this);
					
					tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN), "tactic_open");

					return;

				}

				AddCardButton(card, ScenarioQueue.Tactics.Length-1);

				dialog = DialogManager.instance.CreateTacticDialog(card);
			 	dialog.transform.SetParent(transform);
			 	dialog.gameObject.SetActive(false);

			 	dialog.Index = ScenarioQueue.Tactics.Length;

			 	if(ScenarioQueue.Tactics.Length == 0)
			 		HideCardButton(0);

				ScenarioQueue.AddTacticCard(dialog);

				tacticsAvailable.Remove(tacticsAvailable[cardIndex]);

			}
			else
			{
				dialog = ScenarioQueue.Tactics[cardIndex];

				if(currentTacticCard != null)
					currentTacticCard.gameObject.SetActive(false);
			}

		}
		else 
		{

			dialog = ScenarioQueue.Tactics[cardIndex];

			if(currentTacticCard != null)
				currentTacticCard.gameObject.SetActive(false);

		}

		
		if(tacticsAvailable.Count == 0)
			tacticCardCooldown.Stop();
		else
			tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN), "tactic_open");

		if(currentTacticCard != null && !replace)
			return;

	 	currentTacticCard = dialog;

	 	dialog.gameObject.SetActive(true);
		tacticCardsTooltip.gameObject.SetActive(true);

		// Pause tactic card cooldown
		// tacticCardCooldown.Pause();
	
	}

	void AddCardButton(Models.TacticCard card, int cardIndex) {

		GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
		
		btnChoice.transform.localScale = Vector3.one;
		btnChoice.Text = card.name;

		btnChoice.transform.SetParent(buttonsPanel.transform);

		btnChoice.Button.onClick.RemoveAllListeners();
		btnChoice.Button.onClick.AddListener (() => ReplaceCard(btnChoice.transform.GetSiblingIndex(), btnChoice.gameObject));

	}

	void HideCardButton(int buttonIndex) {

		buttonsPanel.transform.GetChild(buttonIndex).gameObject.SetActive(false);

	}

	void RemoveCard(int cardIndex) {

		ObjectPool.Destroy<GenericButton>(buttonsPanel.transform.GetChild(cardIndex).transform);

		ScenarioQueue.RemoveTacticCard(cardIndex);

		OpenTacticCard(0, true);
		HideCardButton(0);
	}

	bool QueueTacticCard() {

		openTacticCard = true;

		return true;
	}

	public static void Stop() {
	
		// Pause tactic card cooldown
		tacticCardCooldown.Stop();

	}

	public void Toggle() {

		canvasGroup = gameObject.GetComponent<CanvasGroup>();
	
		canvasGroup.interactable = !canvasGroup.interactable;
		canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;

		canvasGroup.alpha = (canvasGroup.alpha == 1) ? 0 : 1;


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

	void EndInvestigate() {

		tacticCardCooldown.Resume();

	 	cooldownText.gameObject.SetActive(false);	
	 	overlayPanel.gameObject.SetActive(false);
	 	buttonsPanel.gameObject.SetActive(true);
		
		// Show tactic card
		currentTacticCard.GetResultOptions();
		currentTacticCard.Enable();

	}

	public void ReplaceCard(int cardIndex, GameObject buttonRef) {

		OpenTacticCard(cardIndex, true);

		foreach(Transform child in buttonsPanel.transform)
			child.gameObject.SetActive(true);

		buttonRef.SetActive(false);

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
    			endInvestigate = true;
    			break;

	   		case "tactic_closed":
	   			// tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN));

	   			RemoveCard(System.Int32.Parse(e.eventSymbol));

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
    		Debug.Log(cooldownTotal + " - " + e.SecondsElapsed);
	    	cooldownElapsed = cooldownTotal - e.SecondsElapsed;
    	}

    }
}

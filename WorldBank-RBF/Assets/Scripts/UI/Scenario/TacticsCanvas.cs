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
using System.Globalization;
using JsonFx.Json;

public class TacticsCanvas : MonoBehaviour {

	public Text cooldownText;

	public RectTransform buttonsPanel;
	public RectTransform overlayPanel;
	public RectTransform cardsPanel;
	public RectTransform tacticCardsTooltip;

	public Image tooltipAlertImg;
	public Image tooltipClockImg;
	public Image tooltipDoneImg;

	Timers.TimerInstance tacticCardCooldown;
	Timers.TimerInstance investigateCooldown;

	// ScenarioCardDialog currentScenarioCard;
	TacticCardDialog currentTacticCard;

	Transform parentPanel;

	static List<string> tacticsAvailable;
	static List<string> allTactics;
	
	static float[] tacticCardIntervals = new float[3] {3, 4, 4};
	
	CanvasGroup canvasGroup;

	bool openTacticCard;
	bool isInvestigating;
	bool endInvestigate;

	string tacticState;

	float cooldownTotal = 0;
	float cooldownElapsed = 0;

	NumberFormatInfo floatFormatter;

	void Start () {

		// Culture for formatting floats to seconds
		floatFormatter = new CultureInfo("en-US", false).NumberFormat;
		floatFormatter.NumberDecimalDigits = 0;

		Events.instance.AddListener<TacticsEvent>(OnTacticsEvent);

		// Listen for problem card cooldown tick
		Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);

		parentPanel = transform.GetChild(0);

		gameObject.SetActive(true);

		canvasGroup = gameObject.GetComponent<CanvasGroup>();
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0;

	}

	void Update () {
		
		// If a tactic card has been enqueued, open it
		if(endInvestigate)
		{
			endInvestigate = false;
			EndInvestigate();
		}
		
		if(cooldownElapsed == 0)
		{
			tacticState = "results";
			endInvestigate = false;
			cooldownElapsed = 5;
		}
		else {
			string strElapsed = cooldownElapsed.ToString("N", floatFormatter);
			cooldownText.text = strElapsed + " seconds";
		}

	}

	public void Initialize(List<string> tactics) {
		
        tacticsAvailable = tactics;
        allTactics = tactics;

		tacticCardIntervals = DataManager.PhaseTwoConfig.tactic_card_intervals;

		tacticCardCooldown = Timers.StartTimer(this.gameObject, tacticCardIntervals);
		tacticCardCooldown.Symbol = "tactic_open";
		tacticCardCooldown.onEnd += GetNextCard;

	}

	public void Toggle() {

		canvasGroup = gameObject.GetComponent<CanvasGroup>();
	
		canvasGroup.interactable = !canvasGroup.interactable;
		canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;

		canvasGroup.alpha = (canvasGroup.alpha == 1) ? 0 : 1;

		parentPanel.gameObject.SetActive(!parentPanel.gameObject.activeSelf);
		tacticCardsTooltip.gameObject.SetActive(ScenarioQueue.Tactics.Length > 0);

		if(currentTacticCard != null && !isInvestigating) {
			currentTacticCard.gameObject.SetActive(true);
			currentTacticCard.ResetButtons();
		}

	}

	public void Investigating(float[] cooldownTime) {


		// Show cooldown text
		// tooltipDoneImg.gameObject.SetActive(false);
		// tooltipAlertImg.gameObject.SetActive(false);
		// tooltipClockImg.gameObject.SetActive(true);
		// tooltipTxt.gameObject.SetActive(true);

		isInvestigating = true;

		cooldownTotal = cooldownTime[0];

		investigateCooldown = Timers.StartTimer(gameObject, cooldownTime);
		investigateCooldown.Symbol = "tactic_results";
		investigateCooldown.onTick += OnCooldownTick;
		investigateCooldown.onEnd += EndInvestigate;

	 	tacticCardCooldown.Stop();

	 	cooldownText.gameObject.SetActive(true);	
	 	overlayPanel.gameObject.SetActive(true);
	 	buttonsPanel.gameObject.SetActive(false);

	 	currentTacticCard.Disable();

	}

	public void ReplaceCard(int cardIndex, GameObject buttonRef) {

		OpenTacticCard(cardIndex, true);

		foreach(Button child in buttonsPanel.transform.GetComponentsInChildren<Button>())
			child.interactable = true;

		buttonRef.GetComponent<Button>().interactable = false;

	}

	void GetNextCard() {
	
		int tacticIndex = new System.Random().Next(0, tacticsAvailable.Count);

		tacticState = "open";
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
					
					tacticCardCooldown = Timers.StartTimer(gameObject, tacticCardIntervals);
					tacticCardCooldown.Symbol = "tactic_open";
					tacticCardCooldown.onEnd += GetNextCard;

					return;

				}

				AddCardButton(card, ScenarioQueue.Tactics.Length-1);

				dialog = DialogManager.instance.CreateTacticDialog(card);
			 	dialog.transform.SetParent(cardsPanel.transform, false);
			 	dialog.gameObject.SetActive(false);

			 	dialog.Index = ScenarioQueue.Tactics.Length;

			 	if(ScenarioQueue.Tactics.Length == 0)
			 		HideCardButton(0);

				ScenarioQueue.AddTacticCard(dialog);

				tacticsAvailable.Remove(tacticsAvailable[cardIndex]);

			}
			else
			{

			 	if(ScenarioQueue.Tactics.Length > 0 && cardIndex < ScenarioQueue.Tactics.Length) {
					dialog = ScenarioQueue.Tactics[cardIndex];
					dialog.ResetButtons();

					if(currentTacticCard != null)
						currentTacticCard.gameObject.SetActive(false);
				}

			}

		}
		else 
		{

		 	if(ScenarioQueue.Tactics.Length > 0 && cardIndex <= ScenarioQueue.Tactics.Length) {
				dialog = ScenarioQueue.Tactics[cardIndex];

				if(currentTacticCard != null)
					currentTacticCard.gameObject.SetActive(false);
			}

		}

		
		if(tacticsAvailable.Count == 0)
			tacticCardCooldown.Stop();
		else {

			tacticCardCooldown = Timers.StartTimer(gameObject, tacticCardIntervals);
			tacticCardCooldown.Symbol = "tactic_open";
			tacticCardCooldown.onEnd += GetNextCard;
		
		}
		
		tacticCardsTooltip.gameObject.SetActive(true);

		if((currentTacticCard != null && !replace) || dialog == null)
			return;

	 	currentTacticCard = dialog;

	 	dialog.gameObject.SetActive(true);

		// Pause tactic card cooldown
		// tacticCardCooldown.Pause();
	
	}

	void AddCardButton(Models.TacticCard card, int cardIndex) {

		TacticChoiceButton btnChoice = ObjectPool.Instantiate<TacticChoiceButton>("Scenario");
		
		btnChoice.transform.localScale = Vector3.one;
		btnChoice.Text = card.name;
		btnChoice.GetComponent<Button>().interactable = true;

		btnChoice.transform.SetParent(buttonsPanel.transform);

		btnChoice.Button.onClick.RemoveAllListeners();
		btnChoice.Button.onClick.AddListener (() => ReplaceCard(btnChoice.transform.GetSiblingIndex(), btnChoice.gameObject));

	}

	void HideCardButton(int buttonIndex) {

		buttonsPanel.transform.GetChild(buttonIndex).gameObject.GetComponent<Button>().interactable = false;

	}

	void RemoveCard(string strButtonName) {

		int cardIndex = 0;

		foreach(TacticChoiceButton child in buttonsPanel.transform.GetComponentsInChildren<TacticChoiceButton>()) {
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
		HideCardButton(0);
	}

	bool QueueTacticCard() {

		openTacticCard = true;

		return true;
	}

	void EndInvestigate() {

		tacticState = "options";
		isInvestigating = false;

		tacticCardCooldown.Resume();

	 	cooldownText.gameObject.SetActive(false);	
	 	overlayPanel.gameObject.SetActive(false);
		currentTacticCard.Enable();

	 	buttonsPanel.gameObject.SetActive(true);
		
		// Show tactic card
		currentTacticCard.GetResultOptions();

	}

    /// <summary>
    // Callback for TacticsEvent, filtering for type of event
    /// </summary>
    void OnTacticsEvent(TacticsEvent e) {

    	Debug.Log("OnTacticsEvent: " + e.eventType);

    	switch(e.eventType) {

	   		case "tactic_results":
    			break;

	   		case "tactic_closed":
	   			// tacticCardCooldown.Init(tacticCardIntervals, new TacticsEvent(TacticsEvent.TACTIC_OPEN));

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

    	int secondsRounded = (int)System.Math.Round(e.SecondsElapsed, 0);

    	if(e.Symbol == "tactic_results")
	    	cooldownElapsed = cooldownTotal - secondsRounded;

    }
}

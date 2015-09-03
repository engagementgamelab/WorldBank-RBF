/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 ScenarioManager.cs
 Phase two scenario management.

 Created by Johnny Richardson on 5/11/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JsonFx.Json;

public class ScenarioManager : MonoBehaviour {

	public ScenarioChatScreen scenarioChat;
	public SupervisorChatScreen supervisorChat;

	public IndicatorsCanvas indicatorsCanvas;
	public Animator scenarioInfoAnimator;

	public Button scenarioChatTab;
	public Button supervisorChatTab;

	public Text scenarioCardCooldownText;
	public Text scenarioCooldownText;

	public float problemCardDurationOverride = 0;
	public float monthLengthSecondsOverride = 0;


	public bool enableCooldown;

	Timers.TimerInstance phaseCooldown;
	Timers.TimerInstance problemCardCooldown;
	Timers.TimerInstance monthCooldown;
	
	ScenarioYearEndDialog yearEndPanel;

	int[] currentAffectValues;
	int[] currentAffectBias;

	string[] monthsLabels = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sept", "Oct",	"Nov", "Dec" };

	List<string> selectedOptions = new List<string>();
	List<int[]> usedAffects = new List<int[]>();

	object tacticsAvailable;

	bool queueProblemCard;
	bool openProblemCard;
	bool openYearEnd;
	bool inYearEnd;

	int scenarioTwistIndex;
	int currentCardIndex;
	int currentQueueIndex;

	int monthsCount = 36;
	int currentMonth = 1;
	int currentYear = 1;

	float problemCardDuration;
	float cardCooldownElapsed;
	float phaseCooldownElapsed;

	float monthLengthSeconds;
	float phaseLength;

	NumberFormatInfo floatFormatter;

	// Use this for initialization
	void Start () {

		#if UNITY_EDITOR
	        NetworkManager.Instance.GetURL("/plan/all/", PlansRetrieved);
		#else
		    // Get plans
			if(PlayerManager.Instance.Authenticated)
		        NetworkManager.Instance.GetURL("/plan/all/", PlansRetrieved);
		#endif

		Events.instance.AddListener<ScenarioEvent>(OnScenarioEvent);

		// Listen for problem card cooldown tick
		Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);

		// Culture for formatting floats to seconds
		floatFormatter = new CultureInfo("en-US", false).NumberFormat;
		floatFormatter.NumberDecimalDigits = 0;

		// Turn off supervisor tab for start
		supervisorChatTab.GetComponent<CanvasGroup>().alpha = .5f;

		DialogManager.instance.CreateTutorialScreen("tooltip_2");

	}

	void Update () {

		// If a problem card has been enqueued or we're waiting for one to open, determine next card
		if(queueProblemCard)
			GetNextCard();

		// Update card cooldown label
    	scenarioCardCooldownText.text = cardCooldownElapsed + "s";

    	// Update scenario cooldown label
    	if(!inYearEnd) {
    		System.TimeSpan timeSpan = TimeSpan.FromSeconds(phaseCooldownElapsed);

    		scenarioCooldownText.text = String.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    	}

	}

	void OnApplicationQuit() {

		if(problemCardCooldown == null)
			return;
        
        problemCardCooldown.Stop();
        monthCooldown.Stop();

    }

    /// <summary>
    /// Callback that handles all display for plans after they are retrieved.
    /// </summary>
    /// <param name="response">Textual response from /plan/all/ endpoint.</param>
    void PlansRetrieved(string response) {

    	Dictionary<string, string>[] planData = JsonReader.Deserialize<Dictionary<string, string>[]>(response);

		List<GenericButton> btnList = new List<GenericButton>();

		foreach(Dictionary<string, string> choice in planData) {

			string planId = choice["id"];

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();

			btnChoice.Text = choice["name"];

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => GetScenarioForPlan(planId));

			btnList.Add(btnChoice);

		}

		DialogManager.instance.CreateChoiceDialog("Choose Plan:", btnList);

    }

    /// <summary>
    /// Increment card index and open next scenario card, show year break, or end the scenario.
    /// </summary>
	public void GetNextCard() {

		if(inYearEnd)
			return;
	
		// currentQueueIndex starts at 1, so decrement it
		int cardIndex = queueProblemCard ? currentQueueIndex : currentCardIndex;
		int nextCardIndex = currentCardIndex + 1;
		int yearLength = DataManager.ScenarioLength(scenarioTwistIndex);

		// Open supervisor panel if no more cards, and disable scenario tab
		if(cardIndex == yearLength)
		{
			scenarioChat.disabledPanel.gameObject.SetActive(true);
			supervisorChat.gameObject.SetActive(true);

			scenarioChatTab.interactable = true;
			supervisorChatTab.interactable = false;
		}
		else
			scenarioChatTab.gameObject.SetActive(true);
 
		// Should we display a year break (happens if forced by timer)?
		if(openYearEnd && !queueProblemCard) {

			// If current month is fewer than 12 (player managed to finish all problems before year end), calculate affects for all missing months
			// TODO: WILL THIS CHANGE?
			if(currentMonth < 12) {
				int mo = 0;
				while(mo < 12-currentMonth) {
					CalculateIndicators();
					mo++;
				}
			}

			// Hide all scenario problem cards
			EndYear();

			// Show end of scenario
			if(currentYear == 3) {

				// Show indicators
				NotebookManager.Instance.OpenIndicators();

				// Show scenario end panel and hide cooldown
				// scenarioEndPanel.gameObject.SetActive(true); TODO: Show system message instead
				scenarioCooldownText.gameObject.SetActive(false);

				monthCooldown.Stop();

				return;

			}

			return;
			
		}

		// Load next card
		if((yearLength-1) > cardIndex) {

			// Hide year end panel
			// yearEndPanel.gameObject.SetActive(false);

			cardIndex = queueProblemCard ? ++currentQueueIndex : ++currentCardIndex;	

			OpenScenarioCard(cardIndex, queueProblemCard);

		}

		queueProblemCard = false;

	}

    /// <summary>
    /// Displays a scenario card, given the current card index.
    /// </summary>
	void OpenScenarioCard(int cardIndex, bool queue=false) {

		Debug.Log("open scenario card with index " + cardIndex);

		// Generate scenario card for the current card index, as well as if the scenario is in a twist
		Models.ScenarioCard card = DataManager.GetScenarioCardByIndex(cardIndex, scenarioTwistIndex);

		if(queue) {
			
			ScenarioQueue.AddProblemCard(card);
			Events.instance.Raise(new ScenarioEvent(ScenarioEvent.PROBLEM_QUEUE));
		
			return;
		}

	 	// Remove card from queue
	 	ScenarioQueue.RemoveProblemCard(card);

		// Create the card dialog
		DialogManager.instance.SetCard(card);

		// SFX
		if(currentCardIndex > 0)
			AudioManager.Sfx.Play ("newproblem", "Phase2");

	}

    /// <summary>
    /// End the current year.
    /// </summary>
	void EndYear () {
		
		// Next year will start at card 0
		currentCardIndex = -1;

		// Queue always starts at 0
		currentQueueIndex = 0;
		
		scenarioChat.gameObject.SetActive(true);
		supervisorChat.gameObject.SetActive(false);

		scenarioChatTab.gameObject.SetActive(true);
		scenarioChatTab.interactable = false;
		supervisorChatTab.interactable = false;
		supervisorChatTab.GetComponent<CanvasGroup>().alpha = .5f;

		// "Year end" screen
		openYearEnd = false;
		inYearEnd = true;
		queueProblemCard = false;

		// Update timer text
		scenarioCooldownText.text = "Break - Year " + currentYear;

		Models.ScenarioConfig scenarioConf = DataManager.GetScenarioConfig();
		DialogManager.instance.EndYear (scenarioConf, selectedOptions);

	}

	void NextProblemCard(string strSymbol) {

		Dictionary<string, int> dictAffect = DataManager.GetIndicatorBySymbol(strSymbol);

		selectedOptions.Add(DataManager.GetUnlockableBySymbol(strSymbol).title);
		usedAffects.Add(dictAffect.Values.ToArray());

		// Initialize tactics cards after first problem card done
		if(currentYear == 1 && currentCardIndex == 0) {
			List<string> availableTactics = ((IEnumerable)tacticsAvailable).Cast<object>().Select(obj => obj.ToString()).ToList<string>();
			
			// Also add tactics that show only if they are not part of player's selected plan
			foreach(string tactic in DataManager.PhaseTwoConfig.tactics_not_selected.ToList<string>())
			{
				if(!availableTactics.Contains(tactic))
					availableTactics.Add(tactic);
			}

			DialogManager.instance.SetAvailableTactics (availableTactics);

			// Enable supervisor tab
			supervisorChatTab.GetComponent<CanvasGroup>().alpha = 1;
		}
		
		GetNextCard();
		currentQueueIndex++;

	}

	void NextYear() {

		// Go to next year
		DataManager.AdvanceScenarioYear();

		currentYear++;
		currentMonth = 1;
		
		inYearEnd = false;

		GetNextCard();

		NotebookManager.Instance.ToggleTabs();

		monthCooldown.Restart();

		supervisorChatTab.interactable = true;
		supervisorChatTab.GetComponent<CanvasGroup>().alpha = 1;

	}

    /// <summary>
    /// Calls API endpoint for handling scenario assignment given a plan ID.
    /// </summary>
    /// <param name="plandId">The plan ID that will trigger a scenario assignment.</param>
    void GetScenarioForPlan(string planId) {

    	// Create dict for POST
        Dictionary<string, object> saveFields = new Dictionary<string, object>();
        
        saveFields.Add("user_id", PlayerManager.Instance.ID);
        saveFields.Add("plan_id", planId);

        // Save user info
        NetworkManager.Instance.PostURL("/user/scenario/", saveFields, UserScenarioResponse);

		PlayerManager.Instance.TrackEvent("Plan ID " + planId + " Selected", "Phase Two");

    }

    /// <summary>
    /// Callback that handles assigning the player a scenario after it is set on server-side.
    /// </summary>
    /// <param name="response">Dictionary response from /user/scenario/ endpoint.</param>
    void UserScenarioResponse(Dictionary<string, object> response) {

    	// Get config values
		// monthLengthSeconds = DataManager.PhaseTwoConfig.month_length_seconds;
		
		phaseLength = DataManager.PhaseTwoConfig.phase_length_seconds;
		monthLengthSeconds = (phaseLength / 36);

		// Allow override in Unity
		#if UNITY_EDITOR
			monthLengthSeconds = (monthLengthSecondsOverride == 0) ? (phaseLength / 36) : monthLengthSecondsOverride;
		#endif

		phaseCooldownElapsed = phaseLength;

		if(enableCooldown) {

			monthCooldown = Timers.StartTimer(gameObject, new [] { monthLengthSeconds });
			monthCooldown.Symbol = "month_cooldown";
			monthCooldown.onEnd += MonthEnd;

			phaseCooldown = Timers.StartTimer(gameObject, new [] { phaseLength });
			phaseCooldown.Symbol = "phase_cooldown";
			phaseCooldown.onTick += OnCooldownTick;
			// monthCooldown.onEnd += MonthEnd;

		}

		Debug.Log("Scenario: " + response["current_scenario"].ToString());

    	// Set scene context from current scenario
    	AssignScenario(response["current_scenario"].ToString());

    	// Save tactics that are a part of this plan
    	tacticsAvailable = response["tactics"];

    	// Set initial values and calc the base affect values for the plan
    	currentAffectValues = response["default_affects"] as int[];
    	currentAffectBias = response["affects_bias"] as int[];

    	// Add defaults to used affects and calc indicators
    	usedAffects.Add(response["default_affects"] as int[]);
    	CalculateIndicators();

    	OpenScenarioCard(0);

		PlayerManager.Instance.TrackEvent("Scenario Assigned", "Phase Two");

		// SFX
		AudioManager.Sfx.Play ("login", "Phase2");

    }

    void AssignScenario(string scenarioSymbol) {

    	// Set scene context from current scenario
    	DataManager.SceneContext = scenarioSymbol;

    	problemCardDuration = (monthLengthSeconds * 12) / DataManager.ScenarioLength(scenarioTwistIndex);

    }

    /// <summary>
    /// Sets the current scenario path, whether it's a twist or a different scenario.
    /// </summary>
    /// <param name="strPathValue">The value to determine the next part of the path.</param>
    void SetScenarioPath(string strPathValue) {

    	// Path is a twist
    	if(strPathValue.Contains("twist"))
	    	scenarioTwistIndex++;
    	// Path is another scenario
    	else
    		AssignScenario(strPathValue);

    	GetNextCard();

    }

    /// <summary>
    // Calculates indicators, given the currently used affects, and then the affect bias for the current plan
    /// </summary>
    void CalculateIndicators() {

		foreach(int[] dictAffect in usedAffects) {

			currentAffectValues[0] += dictAffect[0];
			currentAffectValues[1] += dictAffect[1];
			currentAffectValues[2] += dictAffect[2];

		}

		currentAffectValues[0] += currentAffectBias[0];
		currentAffectValues[1] += currentAffectBias[1];
		currentAffectValues[2] += currentAffectBias[2];

		Debug.Log("--> Indicators: " + currentAffectValues[0] + ", " + currentAffectValues[1] + ", " + currentAffectValues[2]);

		usedAffects.Clear();

		NotebookManager.Instance.UpdateIndicators(currentAffectValues[0], currentAffectValues[1], currentAffectValues[2]);

		scenarioInfoAnimator.Play("IndicatorsUpdate", -1, 0);

    }

    /// <summary>
    // Logic for the end of a phase two month
    /// </summary>
    void MonthEnd() {

    	if(inYearEnd)
    		return;

		CalculateIndicators();	

		currentMonth++;

    	bool atYearEnd = currentMonth == 12;
		
    	if(atYearEnd) {
			Debug.Log("======== END OF YEAR " + currentYear + " ========");
			monthCooldown.Stop();

			openYearEnd = true;
			GetNextCard();
    	}
		else {
			Debug.Log("======== END OF MONTH " + currentMonth + " ========");

			cardCooldownElapsed = problemCardDuration;
			monthCooldown.Restart();
		}

    }

    /// <summary>
    // Callback for ScenarioEvent, filtering for type of event
    /// </summary>
    void OnScenarioEvent(ScenarioEvent e) {

    	Debug.Log("OnScenarioEvent: " + e.eventType);

    	switch(e.eventType) {
   			
    		case "next":

    			NextProblemCard(e.eventSymbol);

    			break;

    		case "next_year":

    			NextYear();

    			break;

	   		case "decision_selected":
	   			SetScenarioPath(e.eventSymbol);
    			break;

			case "open_indicators":
	   			indicatorsCanvas.gameObject.SetActive(true);
	   			break;

    	}

    }

    /// <summary>
    // Callback for TimerTick
    /// </summary>
    void OnCooldownTick(GameEvents.TimerTick e) {

    	/*if(e.Symbol == "problem_card")
			cardCooldownElapsed = problemCardDuration - e.SecondsElapsed;*/
    	
    	if(e.Symbol == "phase_cooldown") {
			phaseCooldownElapsed = phaseLength - e.SecondsElapsed;

			// Debug.Log("############ phaseCooldownElapsed: " + phaseCooldownElapsed);
    	}


    }
}

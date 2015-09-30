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
	public Animator scenarioAnimator;

	public Button scenarioChatTab;
	public Button supervisorChatTab;

	public LayoutElement rightPanel;

	public Text scenarioCardCooldownText;

	public float problemCardDurationOverride = 0;
	public float monthLengthSecondsOverride = 0;
	public string scenarioOverride;

	public Text debugPanelScenarioText;
	public Text debugPanelProblemText;
	public Button debugButton;

	Timers.TimerInstance problemCardCooldown;
	
	ScenarioYearEndDialog yearEndPanel;

	int[] currentAffectValues;
	int[] currentAffectGoals;

	List<int[]> usedAffects = new List<int[]>();
	List<string> availableTactics;

	Animator scenarioCardCooldownAnimator;
	Animator supervisorChatTabAnimator;

	object tacticsAvailable;

	bool enableCooldown;
	bool queueProblemCard;
	bool openProblemCard;
	bool openYearEnd;
	bool inYearEnd;
	bool cardDismissed;

	int scenarioTwistIndex;
	int currentCardIndex;
	int currentQueueIndex;

	int monthsCount = 36;
	int currentMonth = 1;
	int currentYear = 1;

	float problemCardDuration;
	float cardCooldownElapsed;

	float monthLengthSeconds;
	float phaseLength;

	NumberFormatInfo floatFormatter;

	// Use this for initialization
	void Start () {

		Events.instance.AddListener<ScenarioEvent>(OnScenarioEvent);
		Events.instance.AddListener<TutorialEvent>(OnTutorialEvent);

		// Listen for problem card cooldown tick
		// Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);

		// Culture for formatting floats to seconds
		floatFormatter = new CultureInfo("en-US", false).NumberFormat;

		scenarioCardCooldownAnimator = scenarioCardCooldownText.gameObject.GetComponent<Animator>();
		supervisorChatTabAnimator = supervisorChatTab.gameObject.GetComponent<Animator>();
		floatFormatter.NumberDecimalDigits = 0;

		// Turn off supervisor tab for start
		supervisorChatTabAnimator.Play("SupervisorTabOff");
		supervisorChatTab.GetComponent<Button>().enabled = false;

		if(!DataManager.tutorialEnabled)
			enableCooldown = true;

		GetScenarioForPlan(DataManager.currentPlanId);

		// Enable debug info
		#if UNITY_EDITOR || DEVELOPMENT_BUILD
			debugButton.gameObject.SetActive(true);
		#else
			debugButton.gameObject.SetActive(false);
			debugPanelProblemText.transform.parent.gameObject.SetActive(false);
		#endif
	}

	void Update () {

		// If a problem card has been enqueued or we're waiting for one to open, determine next card
		if(queueProblemCard) {
			if(cardCooldownElapsed.Equals(0f))
				GetNextCard();
		}
    	// Update card cooldown label
    	if(!inYearEnd) {
    		System.TimeSpan timeSpan = TimeSpan.FromSeconds(cardCooldownElapsed);

    		scenarioCardCooldownText.text = String.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    	}

	}

	void OnApplicationQuit() {

		if(problemCardCooldown == null)
			return;
        
        problemCardCooldown.Stop();

    }

    /// <summary>
    /// Increment card index and open next scenario card, show year break, or end the scenario.
    /// </summary>
	public void GetNextCard() {

		if(inYearEnd)
			return;

		if(!cardDismissed) {
			// Send player back to group chat
			DisableSupervisor();
			DisableSupervisorTab();

			scenarioChat.NoActionsTaken();

			return;
		}
	
		// currentQueueIndex starts at 1, so decrement it
		int cardIndex = currentCardIndex;
		int nextCardIndex = currentCardIndex + 1;
		int yearLength = DataManager.ScenarioLength(scenarioTwistIndex);

		// Should we display a year break (happens if forced by timer)?
		if(nextCardIndex == yearLength) {
			
			// Hide all scenario problem cards
			EndYear();

			return;
			
		}

		// Load next card
		if((yearLength-1) > cardIndex) {

			// Hide year end panel
			// yearEndPanel.gameObject.SetActive(false);

			cardIndex = ++currentCardIndex;	

			OpenScenarioCard(cardIndex);

		}

		cardDismissed = false;
		queueProblemCard = false;

	}

	public void LoadMainMenu() {

		Application.LoadLevel("Menus");
		
	}

    public void EnableSupervisor() {

    	CanvasGroup supervisorCanvas = supervisorChat.GetComponent<CanvasGroup>();

		scenarioChat.gameObject.SetActive(false);

		supervisorCanvas.alpha = 1;
		supervisorCanvas.interactable = true;
		supervisorCanvas.blocksRaycasts = true;

 		rightPanel.gameObject.SetActive(false);

		// Tutorial
		DialogManager.instance.CreateTutorialScreen("phase_2_supervisor_opened");
    
    }

    public void DisableSupervisor() {

    	CanvasGroup supervisorCanvas = supervisorChat.GetComponent<CanvasGroup>();

		// Ensure scenario tab enabled
		scenarioChatTab.animator.Play("SupervisorTabOn");
		scenarioChatTab.enabled = true;

		scenarioChat.gameObject.SetActive(true);

		supervisorCanvas.alpha = 0;
		supervisorCanvas.interactable = false;
		supervisorCanvas.blocksRaycasts = false;

 		rightPanel.gameObject.SetActive(true);

    }

    void EnableSupervisorTab() {


		// Enable supervisor tab
		supervisorChatTabAnimator.Play("SupervisorTabOn");
		supervisorChatTab.enabled = true;
		supervisorChatTab.interactable = true; 

		// Flash tab if end of first card
		if(currentYear == 1 && currentCardIndex == 1)
			supervisorChatTab.animator.Play("ScenarioTabAlert");

    }

    void DisableSupervisorTab() {

		// Disable supervisor tab
		supervisorChatTabAnimator.Play("SupervisorTabOff");
		supervisorChatTab.enabled = false;
		supervisorChatTab.interactable = false;

    }

    /// <summary>
    /// Displays a scenario card, given the current card index.
    /// </summary>
	void OpenScenarioCard(int cardIndex, bool queue=false) {

		// Clear all prior chat
		scenarioChat.Clear();

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

		// Start card cooldown
		BeginCooldown();

		debugPanelProblemText.text = "Problem Symbol: " + card.symbol;

	}

    /// <summary>
    /// End the current year.
    /// </summary>
	void EndYear () {

		CalculateIndicators();
		
		// Next year will start at card 0
		currentCardIndex = -1;

		// Queue always starts at 0
		currentQueueIndex = 0;

		// "Year end" screen
		inYearEnd = true;

		// Update timer text
		scenarioCardCooldownText.text = "Break - Year " + currentYear;

		Models.ScenarioConfig scenarioConf = DataManager.GetScenarioConfig();
		indicatorsCanvas.EndYear(scenarioConf, currentYear, scenarioTwistIndex);

		indicatorsCanvas.Open();

		// Tutorial
		DialogManager.instance.CreateTutorialScreen("phase_2_year_end");

		// Clear all supervisor dialog
		supervisorChat.gameObject.SetActive(true);
		supervisorChat.Clear();
		supervisorChat.gameObject.SetActive(false);
		
		// Ensure scenario chat is showing
		scenarioChatTab.interactable = false;
		supervisorChatTab.interactable = true;
		scenarioChat.gameObject.SetActive(true);

	}

	void NextProblemCard() {
		
		GetNextCard();
		currentQueueIndex++;

		// Tutorial
		DialogManager.instance.CreateTutorialScreen("phase_2_supervisor");

	}

	void NextYear() {

		// Go to next year
		DataManager.AdvanceScenarioYear();

		currentYear++;
		currentMonth = 1;
		
		inYearEnd = false;

		DialogManager.instance.SetAvailableTactics(availableTactics);

		GetNextCard();

		NotebookManager.Instance.ToggleTabs();

		supervisorChatTabAnimator.Play("SupervisorTabOn");
		supervisorChatTab.GetComponent<Button>().enabled = true;

		// Close indicators
		indicatorsCanvas.Close();

		debugPanelScenarioText.text = "Scenario: " + DataManager.SceneContext.Replace("scenario_", "") + ", Year: " + currentYear;

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

		Debug.Log("Scenario: " + response["current_scenario"].ToString());

    	// Set scene context from current scenario
    	AssignScenario(response["current_scenario"].ToString());

    	// Save tactics that are a part of this plan
    	tacticsAvailable = response["tactics"];

    	// Set initial/goal values and calc the base affect values for the plan
    	currentAffectValues = response["default_affects"] as int[];
    	IndicatorsCanvas.GoalAffects = response["affects_goal"] as int[];

    	// Add defaults to used affects and calc indicators
    	// usedAffects.Add(response["default_affects"] as int[]);

    	OpenScenarioCard(0);

		PlayerManager.Instance.TrackEvent("Scenario Assigned", "Phase Two");

		// SFX
		AudioManager.Sfx.Play ("login", "Phase2");
   
    	// This is the only time we won't show notification
    	CalculateIndicators();

		// Allow skipping only if player has already finished phase two before shooby doopy
		if(PlayerManager.Instance.PhaseTwoDone)
			DialogManager.instance.CreateTutorialScreen("phase_2_start", "phase_2_skip");
		else
			DialogManager.instance.CreateTutorialScreen("phase_2_start", "phase_2_first_problem");

		// Tactics setup
		availableTactics = ((IEnumerable)tacticsAvailable).Cast<object>().Select(obj => obj.ToString()).ToList<string>();
		
		// Also add tactics that show only if they are not part of player's selected plan
		foreach(string tactic in DataManager.PhaseTwoConfig.tactics_not_selected.ToList<string>())
		{
			if(!availableTactics.Contains(tactic))
				availableTactics.Add(tactic);
		}
		DialogManager.instance.SetAvailableTactics(availableTactics);

    }

    void AssignScenario(string scenarioSymbol) {

    	#if UNITY_EDITOR
    		if(!System.String.IsNullOrEmpty(scenarioOverride))
    			DataManager.SceneContext = scenarioOverride;
    		else
		    	DataManager.SceneContext = scenarioSymbol;
    	#else
	    	// Set scene context from current scenario
	    	DataManager.SceneContext = scenarioSymbol;
    	#endif

		problemCardDuration = (monthLengthSeconds * 12) / DataManager.ScenarioLength(scenarioTwistIndex);
		
		#if UNITY_EDITOR
			if(!problemCardDurationOverride.Equals(0f))
				problemCardDuration = problemCardDurationOverride;
		#endif

		cardCooldownElapsed = problemCardDuration;

		debugPanelScenarioText.text = "Scenario: " + scenarioSymbol.Replace("scenario_", "") + ", Year: 1";

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

		Debug.Log("--> Indicators: " + currentAffectValues[0] + ", " + currentAffectValues[1] + ", " + currentAffectValues[2]);

		usedAffects.Clear();

		NotebookManager.Instance.UpdateIndicators(currentAffectValues[0], currentAffectValues[1], currentAffectValues[2]);

    }

    /// <summary>
    // Logic for the end of a phase two month
    /// </summary>
    void MonthEnd() {

    	if(inYearEnd)
    		return;

		currentMonth++;

    	bool atYearEnd = currentMonth == 12;
		
    	if(atYearEnd) {
			Debug.Log("======== END OF YEAR " + currentYear + " ========");
			// monthCooldown.Stop();

			openYearEnd = true;
			GetNextCard();
    	}
		else {
			Debug.Log("======== END OF MONTH " + currentMonth + " ========");

			cardCooldownElapsed = problemCardDuration;
			// monthCooldown.Restart();
		}

    }

    void BeginCooldown() {
		
		if(!enableCooldown)
			return;

		if(problemCardCooldown == null) {
			problemCardCooldown = Timers.Instance.StartTimer(gameObject, new [] { problemCardDuration });
			problemCardCooldown.Symbol = "problem_card";
			problemCardCooldown.onTick += OnCooldownTick;
			problemCardCooldown.onEnd += GetNextCard;
		}
		else
			problemCardCooldown.Restart();

    }

    /// <summary>
    // Callback for ScenarioEvent, filtering for type of event
    /// </summary>
    void OnScenarioEvent(ScenarioEvent e) {

    	Debug.Log("OnScenarioEvent: " + e.eventType);

    	switch(e.eventType) {

    		case "feedback":

    			// Pause cooldown
    			if(problemCardCooldown != null) {
	    			problemCardCooldown.Stop();
	    			scenarioCardCooldownAnimator.Play("TimerPause");
    			}

    			cardDismissed = true;
    			break;
   			
    		case "next":
    			bool firstTutorialProblem = (DataManager.tutorialEnabled && currentCardIndex == 0);

				cardDismissed = true;

				DialogManager.instance.RemoveTutorialScreen();
				
				EnableSupervisorTab();

    			// Resume cooldown
    			if(problemCardCooldown != null) {
	    			problemCardCooldown.Resume();
	    			scenarioCardCooldownAnimator.Play("TimerRunning");
	    		}

    			if(problemCardDuration > 0 && (!firstTutorialProblem || !DataManager.tutorialEnabled)) {
					scenarioChat.NoMessages();
    				queueProblemCard = true;
    			}
    			else
	    			NextProblemCard();

	    		// Add affect for this event to used affects
	    		if(e.eventSymbol != null) {
					Dictionary<string, int> dictAffect = DataManager.GetIndicatorBySymbol(e.eventSymbol);
					usedAffects.Add(dictAffect.Values.ToArray());
				}

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
    // Callback for TutorialEvent, filtering for type of event
    /// </summary>
    void OnTutorialEvent(TutorialEvent e) {

    	switch(e.eventType) {

    		case "skip_tutorial":

	    		DataManager.tutorialEnabled = false;

	    		DialogManager.instance.RemoveTutorialScreen();
	    		
	    		enableCooldown = true;
	    		BeginCooldown();
    			break;

 			case "close":

	    		DialogManager.instance.RemoveTutorialScreen();

	    		enableCooldown = true;
	    		BeginCooldown();
 				break;

    		default:

    			DialogManager.instance.CreateTutorialScreen(e.eventType);	    		
    			break;

    	}
    }

    /// <summary>
    // Callback for TimerTick
    /// </summary>
    void OnCooldownTick(GameEvents.TimerTick e) {

    	if(e.Symbol == "problem_card")
			cardCooldownElapsed = problemCardDuration - e.SecondsElapsed;


    }
}

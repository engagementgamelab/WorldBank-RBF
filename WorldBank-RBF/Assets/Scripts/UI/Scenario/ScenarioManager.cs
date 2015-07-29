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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;

public class ScenarioManager : MonoBehaviour {

	public Text scenarioLabel;
	public Text cardLabel;
	public Text scenarioCardCooldownText;

	public ScenarioYearEndDialog yearEndPanel;
	public RectTransform scenarioEndPanel;
	public RectTransform tacticCardsParent;

	public Animator cameraAnimator;
	public CoverFlow coverFlowHelper;

	public int[] baseAffectValues;
	public int problemCardDurationOverride = 0;


	static TimerUtils.Cooldown problemCardCooldown;
	static TimerUtils.Cooldown phaseCooldown;
	
	int[] currentAffectValues;
	int[] currentAffectBias;

	List<string> selectedOptions = new List<string>();
	List<int[]> usedAffects = new List<int[]>();

	bool queueProblemCard;
	bool openProblemCard;
	bool openYearEnd;

	bool inYearEnd;
	bool indicatorUpdate;

	int scenarioTwistIndex;
	int currentCardIndex;

	int monthsCount = 36;
	int currentMonth = 1;
	int currentYear = 1;
	int problemCardDuration;
	int cardCooldownElapsed;

	// seconds (each month is 25 seconds and there are 3 years)
	public int monthLengthSeconds = 25;

	// ScenarioCardDialog currentScenarioCard;
	TacticCardDialog currentTacticCard;

	// Use this for initialization
	void Start () {

		#if UNITY_EDITOR
	        NetworkManager.Instance.GetURL("/plan/all/", PlansRetrieved);
		#else
		    // Get plans
			if(PlayerManager.Instance.Authenticated)
		        NetworkManager.Instance.GetURL("/plan/all/", PlansRetrieved);
		#endif

		// Set current affect values to base values
		currentAffectValues = baseAffectValues;

		Events.instance.AddListener<ScenarioEvent>(OnScenarioEvent);

		// Listen for problem card cooldown tick
		Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);

	}

	void Update () {

		// Update indicators with current affects
		if(indicatorUpdate) {
			indicatorUpdate = false;
			NotebookManager.Instance.UpdateIndicators(currentAffectValues[0], currentAffectValues[1], currentAffectValues[2]);
		}
		
		// If a problem card has been enqueued or we're waiting for one to open, determine next card
		else if(queueProblemCard || openProblemCard)
		{
			openProblemCard = false;
			GetNextCard();
		}

		// The current year has ended
		else if(openYearEnd) {
			openYearEnd = false;
			GetNextCard(true);
		}

		// Update card cooldown label
    	scenarioCardCooldownText.text = "Waiting for next Problem Card: " + cardCooldownElapsed + "s";

	}

	void OnApplicationQuit() {
        
        problemCardCooldown.Stop();
        phaseCooldown.Stop();

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
    /// <param name="newYear">Load new year (skip year break check). Default is false.</param>
	public void GetNextCard(bool newYear=false) {

		int nextCard = currentCardIndex+1;

		// Should we display a year break (happens at 4th and 8th card or if forced by timer)?
		if(newYear || (nextCard > 0 && nextCard <= 8) && (nextCard % 4 == 0)) {

			// Go to next year
			DataManager.AdvanceScenarioYear();

			problemCardCooldown.Stop();

			// Hide all scenario problem cards
			ObjectPool.DestroyAll<ScenarioCardDialog>();

			yearEndPanel.PreviousChoices = selectedOptions;

			// Show year end panel
			yearEndPanel.gameObject.SetActive(true);

			inYearEnd = true;
			queueProblemCard = false;

			// If current month is fewer than 12 (player managed to finish all problems before year end), calculate affects for all missing months
			// TODO: WILL THIS CHANGE?
			if(currentMonth < 12) {
				int mo = 0;
				while(mo < 12-currentMonth) {
					CalculateIndicators(true);
					mo++;
				}
			}

			currentMonth = 1;

			return;
			
		}

		// Load next card
		if(DataManager.ScenarioLength(scenarioTwistIndex)-1 > currentCardIndex) {
			
			// Hide year end panel
			yearEndPanel.gameObject.SetActive(false);

			currentCardIndex++;
			OpenDialog();

		}
		// Show end of scenario
		else {

			// ObjectPool.Destroy<ScenarioCardDialog>(currentScenarioCard.transform);

			// Show scenario end panel
			scenarioEndPanel.gameObject.SetActive(true);

		}

	}

    /// <summary>
    /// Open either a scenario or tactic card dialog.
    /// </summary>
	public void OpenDialog() {

		// Open Scenario, Scenario Decision, or Tactic Card?
		if(inYearEnd) {
			
			inYearEnd = false;
			
			// Next year will start at card 0
			currentCardIndex = -1;

			OpenScenarioDecisionCard();

			return;

		}

		OpenScenarioCard();

	}

	public void ShowTacticsCards() {

		// cameraAnimator.Play("TacticsCameraEnter");
		tacticCardsParent.gameObject.SetActive(!tacticCardsParent.gameObject.activeSelf);

	}

    /// <summary>
    /// Displays a scenario card, given the current card index.
    /// </summary>
	void OpenScenarioCard() {

		Debug.Log("open scenario card with index " + currentCardIndex);

		// Generate scenario card for the current card index, as well as if the scenario is in a twist
		Models.ScenarioCard card = DataManager.GetScenarioCardByIndex(currentCardIndex, scenarioTwistIndex);

		problemCardCooldown.Init(
			new int[] { (problemCardDurationOverride == 0) ? problemCardDuration : problemCardDurationOverride }, 
			new ScenarioEvent(ScenarioEvent.PROBLEM_OPEN), "problem_card"
		);

		if(queueProblemCard) {
			ScenarioQueue.AddProblemCard(card);
			Events.instance.Raise(new ScenarioEvent(ScenarioEvent.PROBLEM_QUEUE));
		
			queueProblemCard = false;
			return;
		}

		// Create the card dialog
	 	DialogManager.instance.CreateScenarioDialog(card);

	 	// Remove card from queue
	 	ScenarioQueue.RemoveProblemCard(card);

    	// Debug
    	// cardLabel.text = card.symbol;
    	// cardLabel.gameObject.SetActive(true);

    	// TODO: Temp UI stuff
    	if(currentTacticCard != null)
    	{
	    	currentTacticCard.transform.SetAsFirstSibling();
			currentTacticCard.gameObject.SetActive(false);
			currentTacticCard.gameObject.SetActive(true);		    	
    	}

	}

    /// <summary>
    /// Displays a secnario card, given the current card index.
    /// </summary>
	void OpenScenarioDecisionCard() {

		// Generate scenario year card for the current scenario year
		Models.ScenarioConfig scenarioConf = DataManager.GetScenarioConfig();

		// Create the card dialog
		DialogManager.instance.CreateScenarioDecisionDialog(scenarioConf);
	    	
	}

	bool QueueProblemCard() {

		return true;
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

		problemCardCooldown = new TimerUtils.Cooldown();
		phaseCooldown = new TimerUtils.Cooldown();

		phaseCooldown.Init(new int[] { monthLengthSeconds }, new ScenarioEvent(ScenarioEvent.MONTH_END));

		Debug.Log("Scenario: " + response["current_scenario"].ToString());

    	// Set scene context from current scenario
    	AssignScenario(response["current_scenario"].ToString());

    	// Set tactics that are a part of this plan
    	TacticsCanvas.Available = ((IEnumerable)response["tactics"]).Cast<object>().Select(obj => obj.ToString()).ToList<string>();

    	// Calc the base affect values for the plan
    	currentAffectBias = response["affects_bias"] as int[];
    	usedAffects.Add(response["default_affects"] as int[]);
    	CalculateIndicators();

    	OpenDialog();

		PlayerManager.Instance.TrackEvent("Scenario Assigned", "Phase Two");

    }

    void AssignScenario(string scenarioSymbol) {

    	// Set scene context from current scenario
    	DataManager.SceneContext = scenarioSymbol;

    	problemCardDuration = (monthLengthSeconds * 12) / DataManager.ScenarioLength(scenarioTwistIndex);

    	// Debug
    	scenarioLabel.text = DataManager.SceneContext.Replace("_", " ") + ": ";
    	scenarioLabel.gameObject.SetActive(true);

    }

    /// <summary>
    /// Sets the current scenario path, whether it's a twist or a different scenario.
    /// </summary>
    /// <param name="strPathValue">The value to determine the next part of the path.</param>
    void SetScenarioPath(string strPathValue) {

    	// Path is a twist
    	if(strPathValue.Contains("twist")) {
	    	scenarioTwistIndex++;

	    	// Debug
	    	scenarioLabel.text = DataManager.SceneContext.Replace("_", " ") + ", twist " + scenarioTwistIndex;
    	}
    	// Path is another scenario
    	else
    		AssignScenario(strPathValue);

    	GetNextCard();

    }

    /// <summary>
    // Calculates indicators, given the currently used affects, and then the affect bias for the current plan
    /// </summary>
    void CalculateIndicators(bool updateNow=false) {

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

		if(updateNow)
			NotebookManager.Instance.UpdateIndicators(currentAffectValues[0], currentAffectValues[1], currentAffectValues[2]);
		else
			indicatorUpdate = true;

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
			phaseCooldown.Stop();

			openYearEnd = true;
    	}
		else {
			// Debug.Log("======== END OF MONTH " + currentMonth + " ========");
			phaseCooldown.Init(new int[] { monthLengthSeconds }, new ScenarioEvent(ScenarioEvent.MONTH_END), "Month " + currentMonth);
		}

		// Debug.Log("--> Indicators: " + currentAffectValues[0] + ", " + currentAffectValues[1] + ", " + currentAffectValues[2]);
		// Debug.Log("===================================================");

    }

    /// <summary>
    // Callback for ScenarioEvent, filtering for type of event
    /// </summary>
    void OnScenarioEvent(ScenarioEvent e) {

    	Debug.Log("OnScenarioEvent: " + e.eventType);

    	switch(e.eventType) {

	   		case "investigate":
	   			CalculateIndicators();
	   			MonthEnd();
    			break;

    		case "next":
				Dictionary<string, int> dictAffect = DataManager.GetIndicatorBySymbol(e.eventSymbol);

    			selectedOptions.Add(e.eventSymbol);
    			usedAffects.Add(dictAffect.Values.ToArray());

    			openProblemCard = true;
    			break;

    		case "problem_open":

    			if(!inYearEnd)
	    			queueProblemCard = true;

    			break;

	   		case "decision_selected":
	   			SetScenarioPath(e.eventSymbol);
    			break;

	   		case "month_end":
	   			CalculateIndicators();
	   			MonthEnd();
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

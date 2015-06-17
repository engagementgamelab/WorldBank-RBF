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

	public RectTransform yearEndPanel;
	public RectTransform scenarioEndPanel;

	private static int currentCardIndex;

	private static TimerUtils.Cooldown tacticCardCooldown;
	
	private static int[] tacticCardIntervals = new int[3] {3, 3, 3};
	private List<string> tacticsAvailable;

	private List<string> selectedOptions = new List<string>();

	private bool openTacticCard;
	private string tacticState;

	private ScenarioCardDialog currentScenarioCard;
	private TacticCardDialog currentTacticCard;

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

	}

	void Update () {

		// If a tactic card has been enqueued, open it
		if(openTacticCard)
		{
			openTacticCard = false;
			OpenDialog(true);
		}

	}

    /// <summary>
    /// Callback that handles all display for plans after they are retrieved.
    /// </summary>
    /// <param name="response">Textual response from /plan/all/ endpoint.</param>
    private void PlansRetrieved(string response) {

    	Dictionary<string, string>[] planData = JsonReader.Deserialize<Dictionary<string, string>[]>(response);

		List<GenericButton> btnList = new List<GenericButton>();

		foreach(Dictionary<string, string> choice in planData) {

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();

			btnChoice.Text = choice["name"];

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => GetScenarioForPlan(choice["id"]));

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

		// Should we display a year break (happens at 4th and 8th card)?
		if(!newYear && (nextCard > 0 && nextCard <= 8) && (nextCard % 4 == 0)) {
			
			// Pause tactic card cooldown
			tacticCardCooldown.Pause();

			ObjectPool.Destroy<ScenarioCardDialog>(currentScenarioCard.transform);

			// Show year end panel
			yearEndPanel.gameObject.SetActive(true);

			return;
		}
		// Load next card
		if(DataManager.ScenarioLength()-1 > currentCardIndex) {
			
			// Hide year end panel
			yearEndPanel.gameObject.SetActive(false);

			currentCardIndex++;
			OpenDialog();

		}
		// Show end of scenario
		else {

			ObjectPool.Destroy<ScenarioCardDialog>(currentScenarioCard.transform);

			// Show scenario end panel
			scenarioEndPanel.gameObject.SetActive(true);

		}

	}

    /// <summary>
    /// Open either a scenario or tactic card dialog.
    /// </summary>
    /// <param name="isTactic">Is this card for a tactic? (Default: false)</param>
	public void OpenDialog(bool isTactic=false) {

		// Open Scenario or Tactic Card?
		if(!isTactic)
			OpenScenarioCard();
		else
			OpenTacticCard();

	}

	void OpenScenarioCard() {

			// Generate scenario card for the current card index
			Models.ScenarioCard card = DataManager.GetScenarioCardByIndex(currentCardIndex);

			// Create the card dialog
		 	currentScenarioCard = DialogManager.instance.CreateScenarioDialog(card);

	    	// Debug
	    	cardLabel.text = card.symbol;
	    	cardLabel.gameObject.SetActive(true);

	    	// Temp UI stuff
	    	if(currentTacticCard != null)
	    	{
		    	currentTacticCard.transform.SetAsFirstSibling();
				currentTacticCard.gameObject.SetActive(false);
				currentTacticCard.gameObject.SetActive(true);		    	
	    	}
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

				tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN));
				
				Debug.LogWarning("Unable to locate a tactic card for '" + tacticsAvailable[tacticIndex] + "'. Timer restarting.", this);

				return;

			}

			currentTacticCard = DialogManager.instance.CreateTacticDialog(card);

			tacticsAvailable.Remove(tacticsAvailable[tacticIndex]);

			if(tacticsAvailable.Count == 0)
				tacticCardCooldown.Stop();
		}
		else 
		{
			// Show tactic card
			currentTacticCard.Enable();

			currentTacticCard.GetResultOptions();
		}

		// Pause tactic card cooldown
		tacticCardCooldown.Pause();
	
	}

	bool QueueTacticCard() {

		openTacticCard = true;

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
        NetworkManager.Instance.PostURL("/user/scenario/", saveFields, AssignScenario);

		PlayerManager.Instance.TrackEvent("Plan ID " + planId + " Selected", "Phase Two");

    }

    /// <summary>
    /// Callback that handles assigning the player a scenario after it is set on server-side.
    /// </summary>
    /// <param name="response">Dictionary response from /user/scenario/ endpoint.</param>
    void AssignScenario(Dictionary<string, object> response) {

		tacticCardCooldown = new TimerUtils.Cooldown();
		tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN));

    	// Set scene context from current scenario
    	DataManager.SceneContext = response["current_scenario"].ToString();

    	// Set tactics that are a part of this plan
    	tacticsAvailable = ((IEnumerable)response["tactics"]).Cast<object>().Select(obj => obj.ToString()).ToList<string>();

    	// Debug
    	scenarioLabel.text = DataManager.SceneContext.Replace("_", " ") + ": ";
    	scenarioLabel.gameObject.SetActive(true);

    	OpenDialog();

		PlayerManager.Instance.TrackEvent("Scenario Assigned", "Phase Two");

    }

    /// <summary>
    // Callback for ScenarioEvent, filtering for type of event
    /// </summary>
    void OnScenarioEvent(ScenarioEvent e) {

    	switch(e.eventType) {

    		case "next":
    			selectedOptions.Add(e.eventSymbol);
    			GetNextCard();
    			break;

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

    	}

    }
}

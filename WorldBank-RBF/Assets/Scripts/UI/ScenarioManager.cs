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
	public RectTransform endPanel;

	public static List<string> currentAdvisorOptions;
	public static List<string> currentCardOptions;

	private static int currentCardIndex;

	private static TimerUtils.Cooldown tacticCardCooldown;
	
	public static List<string> tacticCardOptions  = new List<string> { "Investigate", "Observe" };
	private static int[] tacticCardIntervals = new int[3] {3, 3, 3};
	private List<string> tacticsAvailable;

	private bool openTacticCard;
	private string tacticState;

	private ScenarioCardDialog currentScenarioCard;
	private TacticCardDialog currentTacticCard;

	// Use this for initialization
	void Start () {

        // Get plans
		if(PlayerManager.Instance.Authenticated)
	        NetworkManager.Instance.GetURL(DataManager.RemoteURL + "/plan/all/", PlansRetrieved);

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
    /// Increment card index and open next scenario card, or end the scenario.
    /// </summary>
	public void GetNextCard() {

		if(DataManager.ScenarioLength()-1 > currentCardIndex) {
			currentCardIndex++;
			OpenDialog();
		}
		else {
			ObjectPool.Destroy<ScenarioCardDialog>(currentScenarioCard.transform);

			endPanel.gameObject.SetActive(true);
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

			// Generate advisor options
			currentAdvisorOptions = card.characters.Select(x => x.Key).ToList();
			currentCardOptions = new List<string>(card.starting_options);

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

	void OpenTacticCard() {

		if(tacticState == "open")
		{
			int tacticIndex = new System.Random().Next(0, tacticsAvailable.Count);

			Models.TacticCard card = DataManager.GetTacticCardByName(tacticsAvailable[tacticIndex]);
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
        NetworkManager.Instance.PostURL(DataManager.RemoteURL + "/user/scenario/", saveFields, AssignScenario);

    }

    /// <summary>
    /// Callback that handles assigning the player a scenario after it is set on server-side.
    /// </summary>
    /// <param name="response">Dictionary response from /user/scenario/ endpoint.</param>
    void AssignScenario(Dictionary<string, object> response) {

		tacticCardCooldown = new TimerUtils.Cooldown();
		tacticCardCooldown.Init(tacticCardIntervals, new ScenarioEvent(ScenarioEvent.TACTIC_OPEN));

		Debug.Log("Scenario: " + response["current_scenario"].ToString());

    	// Set scene context from current scenario
    	DataManager.SceneContext = response["current_scenario"].ToString();

    	// Set tactics that are a part of this plan
    	tacticsAvailable = ((IEnumerable)response["tactics"]).Cast<object>().Select(obj => obj.ToString()).ToList<string>();

    	// Debug
    	scenarioLabel.text = DataManager.SceneContext.Replace("_", " ") + ": ";
    	scenarioLabel.gameObject.SetActive(true);

    	OpenDialog();

    }

    /// <summary>
    // Callback for ScenarioEvent, filtering for type of event
    /// </summary>
    void OnScenarioEvent(ScenarioEvent e) {

    	switch(e.eventType) {

    		case "next":
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

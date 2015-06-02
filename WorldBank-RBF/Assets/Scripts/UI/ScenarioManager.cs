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

	public static List<string> currentAdvisorOptions;
	public static List<string> currentCardOptions;

	private static int currentCardIndex;

	private static TimerUtils.RandomCooldown tacticCardCooldown;
	private static TimerUtils.Cooldown tacticInvestigateCooldown;
	
	public static List<string> tacticCardOptions  = new List<string> { "Investigate", "Observe" };
	private static int[] tacticCardIntervals = new int[3] {3, 3, 3};

	private bool openTacticCard;
	private string tacticState;

	private TacticCardDialog currentTacticCard;

	// Use this for initialization
	void Start () {

		PlayerManager.Instance.Authenticate();

        // Get plans
        NetworkManager.Instance.GetURL(DataManager.config.serverRoot + "/plan/all/", PlansRetrieved);

		Events.instance.AddListener<ScenarioEvent>(OnScenarioEvent);

	}

	void Update () {

		if(openTacticCard)
		{
			OpenDialog(true);
			openTacticCard = false;
		}
		

	}

    /// <summary>
    /// Callback that handles all display for plans after they are retrieved.
    /// </summary>
    /// <param name="response">Textual response from /plan/all/ endpoint.</param>
    private void PlansRetrieved(string response) {

    	string[] planIDs = JsonReader.Deserialize<string[]>(response);

		List<GenericButton> btnList = new List<GenericButton>();

		foreach(string choice in planIDs) {

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();

			btnChoice.Text = choice;

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => GetScenarioForPlan(choice));

			btnList.Add(btnChoice);
		}

		DialogManager.instance.CreateChoiceDialog("Choose Plan:", btnList);

    }

	public void OpenDialog(bool isTactic=false, string dialogState="open") {

		if(!isTactic) {
		
			Models.ScenarioCard card = DataManager.GetScenarioCardByIndex(currentCardIndex);

			currentAdvisorOptions = card.characters.Select(x => x.Key).ToList();
			currentCardOptions = new List<string>(card.starting_options);

			DialogManager.instance.CreateScenarioDialog(card);

	    	// Debug
	    	cardLabel.text = card.symbol;
	    	cardLabel.gameObject.SetActive(true);
		
		}
		else {

			if(tacticState == "open")
			{
				Models.TacticCard card = DataManager.GetTacticCardByName(DataManager.tacticNames[0]);
				currentTacticCard = DialogManager.instance.CreateTacticDialog(card);
			}
			else 
			{
    			// Show tactic card
    			currentTacticCard.Enable();

				currentTacticCard.GetResultOptions();
			}
		}

		tacticCardCooldown.Pause();

	}

	public void GetNextCard() {

		currentCardIndex++;

		OpenDialog();

		tacticCardCooldown.Resume();

	}

	private bool QueueTacticCard() {

		openTacticCard = true;

		return true;
	}

    /// <summary>
    /// Calls API endpoint for handling scenario assignment given a plan ID.
    /// </summary>
    /// <param name="plandId">The plan ID that will trigger a scenario assignment.</param>
    private void GetScenarioForPlan(string planId) {

    	// Create dict for POST
        Dictionary<string, object> saveFields = new Dictionary<string, object>();
        
        saveFields.Add("user_id", PlayerManager._userId);
        saveFields.Add("plan_id", planId);

        // Save user info
        NetworkManager.Instance.PostURL(DataManager.config.serverRoot + "/user/scenario/", saveFields, AssignScenario);

    }

    /// <summary>
    /// Callback that handles assigning the player a scenario after it is set on server-side.
    /// </summary>
    /// <param name="response">Dictionary response from /user/scenario/ endpoint.</param>
    private void AssignScenario(Dictionary<string, object> response) {

    	tacticCardCooldown = new TimerUtils.RandomCooldown(tacticCardIntervals);

    	// Set scene context from current scenario
    	DataManager.currentSceneContext = response["current_scenario"].ToString();

    	// Debug
    	scenarioLabel.text = DataManager.currentSceneContext.Replace("_", " ") + ": ";
    	scenarioLabel.gameObject.SetActive(true);

    	OpenDialog();

    }

    // Callback for ScenarioEvent
    private void OnScenarioEvent(ScenarioEvent e) {

    	switch(e.eventType) {
    		case "cooldown":
    			tacticState = "open";
				QueueTacticCard();
				break;
    		case "next":
    			GetNextCard();
    			break;
    		case "investigate":
    			tacticState = "options";

    			// Hide tactic card for now
    			currentTacticCard.Disable();
    			
    			if(tacticInvestigateCooldown == null)
	    			tacticInvestigateCooldown = new TimerUtils.Cooldown();
    			
    			tacticInvestigateCooldown.Init(5, new ScenarioEvent(ScenarioEvent.TACTIC_RESULTS));
    			break;
	   		case "tactic_results":
				QueueTacticCard();
    			break;
	   		case "tactic_closed":
	   			tacticCardCooldown.Resume();
    			break;
    	}

    }
}

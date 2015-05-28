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

	public static List<string> currentAdvisorOptions;
	public static List<string> currentCardOptions;

	private static int currentCardIndex;

	private static TimerUtils.RandomCooldown tacticCardCooldown;
	private static int[] tacticCardIntervals = new int[3] {30, 45, 60};

	// Use this for initialization
	void Start () {

		PlayerManager.Instance.Authenticate();

        // Get plans
        NetworkManager.Instance.GetURL(DataManager.config.serverRoot + "/plan/all/", PlansRetrieved);

	}

    public void PlansRetrieved(string response) {

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

    public void AssignScenario(Dictionary<string, object> response) {

    	tacticCardCooldown = new TimerUtils.RandomCooldown(tacticCardIntervals, OpenDialog);

    	// Set scene context from current scenario
    	DataManager.currentSceneContext = response["current_scenario"].ToString();

    	scenarioLabel.text = DataManager.currentSceneContext.Replace("_", " ");
    	scenarioLabel.gameObject.SetActive(true);

    	OpenDialog();

    }

	public static void OpenDialog() {

		currentCardIndex = 0;

		Models.ScenarioCard scenario = DataManager.GetScenarioCardByIndex(currentCardIndex);

		currentAdvisorOptions = scenario.characters.Select(x => x.Key).ToList();
		currentCardOptions = new List<string>(scenario.starting_options);

		DialogManager.instance.CreateScenarioDialog(scenario);

		tacticCardCooldown.Pause();

	}

	public static void GetNextCard() {

		currentCardIndex++;

		OpenDialog();

		tacticCardCooldown.Resume();

	}

    private void GetScenarioForPlan(string planId) {

        Dictionary<string, object> saveFields = new Dictionary<string, object>();
        
        saveFields.Add("user_id", PlayerManager._userId);
        saveFields.Add("plan_id", planId);

        // Save user info
        NetworkManager.Instance.PostURL(DataManager.config.serverRoot + "/user/scenario/", saveFields, AssignScenario);

    }
}

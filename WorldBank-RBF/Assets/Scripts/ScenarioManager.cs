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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;

public class ScenarioManager : MonoBehaviour {

	public static List<string> currentAdvisorOptions;
	public static List<string> currentCardOptions;

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

			Debug.Log(btnChoice);

			btnChoice.Text = choice;

			btnChoice.Button.onClick.RemoveAllListeners();

			// string t = choice; // I don't understand why this is necessary, but if you just pass in 'choice' below, it will break
			btnChoice.Button.onClick.AddListener (() => GetScenarioForPlan(choice));
			// btnChoice.Button.onClick.AddListener(() => OpenSpeechDialog(currNpc, choiceName, npc, false));

			btnList.Add(btnChoice);
		}

		// BackButtonDelegate del = delegate { OpenSpeechDialog(currNpc, "Initial", npc, true); };

		DialogManager.instance.CreateChoiceDialog("Choose Plan:", btnList);

    }

    private void GetScenarioForPlan(string planId) {

        Dictionary<string, object> saveFields = new Dictionary<string, object>();
        
        saveFields.Add("user_id", PlayerManager._userId);
        saveFields.Add("plan_id", planId);

        // Save user info
        NetworkManager.Instance.PostURL(DataManager.config.serverRoot + "/user/scenario/", saveFields, AssignScenario);

    }

    public void AssignScenario(Dictionary<string, object> response) {

    	// Set scene context from current scenario
    	DataManager.currentSceneContext = response["current_scenario"].ToString();

    }

	public void OpenDialog() {

		Models.Scenario scenario = DataManager.GetScenarioBySymbol("card_template");

		currentAdvisorOptions = scenario.characters.Select(x => x.Key).ToList();
		currentCardOptions = new List<string>(scenario.starting_options);

		DialogManager.instance.CreateScenarioDialog(scenario);

	}
}

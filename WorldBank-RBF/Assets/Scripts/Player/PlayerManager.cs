using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Linq;
using JsonFx.Json;
 
// TODO: This needs lots of cleanup
public class PlayerManager : MonoBehaviour {
    
    protected PlayerManager() {}
    static PlayerManager _instance = null;

    Models.PlanRecord plan;
        
    public static PlayerManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType (typeof (PlayerManager)) as PlayerManager;

                if (_instance == null) {
                    GameObject obj = new GameObject ();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent<PlayerManager> ();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Is player authenticated?
    /// </summary>
    /// <returns>Yes or no.</returns>
    public bool Authenticated {
        get {
            return _isAuthenticated;
        }
    }

    /// <summary>
    /// Get player's ID.
    /// </summary>
    /// <returns>The player's ID.</returns>
    public string ID {
        get {
            return _playerId;
        }
    }

    /// <summary>
    /// Get player's plan submission status.
    /// </summary>
    public bool PlanSubmitted {
        get {
            return _submittedPlan || PlayerPrefs.HasKey("current plan");
        }
    }

    /// <summary>
    /// Get player's phase two completion status.
    /// </summary>
    public bool PhaseTwoDone {
        get {
            return _phaseTwoDone || PlayerPrefs.HasKey("phase 2 done");
        }
    }

    bool _isAuthenticated;
    bool _phaseTwoDone;
    bool _submittedPlan;

    string _playerId;

    Models.Plan _userCurrentPlan;

    public void Authenticate(string email, string pass="") {

        NetworkManager.Instance.PostURL("/user/auth/", 
                                        new Dictionary<string, object>() {{ "email", email }},
                                        AuthCallback);
        
    }

    public void Register (string email, string firstName, string lastName) {

        Dictionary<string, object> registerFields = new Dictionary<string, object>();

        registerFields.Add("email", email);
        registerFields.Add("first_name", firstName);
        registerFields.Add("last_name", lastName);

        NetworkManager.Instance.PostURL("/user/create/", registerFields, AuthCallback);

    }

    public void AuthCallback(Dictionary<string, object> response) {

        // Local version
        if(response.ContainsKey("local")) 
        {
            _playerId = "local";
            Events.instance.Raise(new PlayerLoginEvent(true));
            return;
        }

        if(response.ContainsKey("error"))  
        {          
            Events.instance.Raise(new PlayerLoginEvent(false, response["error"].ToString()));
            return;
        }

        System.Text.StringBuilder output = new System.Text.StringBuilder();
        
        JsonWriter writer = new JsonWriter (output);
        
        writer.Write(response["user"]);

        // Set user info
        Models.User user = JsonReader.Deserialize<Models.User>(output.ToString());

        _playerId = user._id;
        GA.API.Design.NewEvent ("player_id: " + _playerId);

        _submittedPlan = user.submitted_plan;
        _phaseTwoDone = user.phase_two_done;
        
        _isAuthenticated = Convert.ToBoolean(response["auth"]);

        Events.instance.Raise(new PlayerLoginEvent(true));

        //Analytics.SetUserId(_playerId);

        TrackEvent("User Login", "API");
        
        return;
        
    }

    public void SaveData(Dictionary<string, object> saveFields, Action<Dictionary<string, object>> response=null) {

        // Insert user ID
        saveFields.Add("user_id", _playerId);

        #region Local Data Fallback
            
            // Saving plan
            if(!saveFields.ContainsKey("save_phase_2")) {

                // Save form as raw byte array as local fallback
                System.Text.StringBuilder output = new System.Text.StringBuilder();
                JsonWriter writer = new JsonWriter (output);

                Models.Plan p = saveFields["plan"] as Models.Plan;
        		Models.PlanRecord planData = new Models.PlanRecord();
        		
                planData.name = p.name;
        		planData.tactics = p.tactics;

                planData.affects_goal = new int[] { 30, 35, 40 };

                writer.Write( GradePlan(planData) );

                PlayerPrefs.SetString("current plan", output.ToString());

            }
            // Saving phase 2 state
            else
                PlayerPrefs.SetInt("phase 2 done", 1);

            PlayerPrefs.Save();

        #endregion

        // Save user info
        NetworkManager.Instance.PostURL("/user/save/", saveFields, response);

    }

    public void TrackEvent(string strEventName, string strEventCategory) {

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            return;
        #else

            var parseFields = new Dictionary<string, string>() {{ "user", _playerId }};
            Dictionary<string, object> postFields = new Dictionary<string, object>() {{ "eventName", strEventName }, { "eventCategory", strEventCategory }, { "userId", _playerId }};

            // Send to Parse SDK
            // ParseAnalytics.TrackEventAsync(strEventName, parseFields);

           //Analytics.CustomEvent(strEventName, new Dictionary<string, object>() {{ "eventCategory", strEventCategory }, { "userId", _playerId }});

            // Send analytic event
            NetworkManager.Instance.PostURL("/analytics/event/", postFields);

            Debug.Log("Track Event: '" + strEventName + "'");

        #endif

    }

    Models.PlanRecord GradePlan(Models.PlanRecord planInput) {

        int[] planKeysConfig = { 3, 3, 3, 2, 2, 1 };

        string[][] assignmentMatrix = { new string[] {"scenario_2", "scenario_4"}, new string[] {"scenario_1", "scenario_3"} };

        // Obtain scenario filtering flags 
        Dictionary<string, string> scenarioFilters = new Dictionary<string, string>() {
           { "autonomy", "unlockable_grant_providers_autonomy" },
           { "pbc", "unlockable_contract_outside_organization_to_administer_plan" }
        };

        int planScore = 14;
        int optionIndex = 0;

        while(optionIndex < planInput.tactics.Length) {

            // Get the priority of this tactic
            int tacticPriority = DataManager.GetUnlockableBySymbol(planInput.tactics[optionIndex]).priority;

            // If no priority, default to 0
            if(tacticPriority == null)
              tacticPriority = 0;

            // Calculate reduction for total score
            int scoreReduction = Mathf.Abs(tacticPriority - planKeysConfig[optionIndex]);

            planScore -= scoreReduction;

            optionIndex++;

        }

		planInput.score = planScore;

        // Get the grading info for the plan score
        // Score is within range of grading block?
        Models.Grade gradeInfo = DataManager.GetGradeForPlan(planInput); 

        // Determine which filtering flags the plan meets
        bool hasPbc = Array.Exists(planInput.tactics, el => el.Equals(scenarioFilters["pbc"]));
        bool hasAutonomy = Array.Exists(planInput.tactics, el => el.Equals(scenarioFilters["autonomy"]));

        Models.PlanRecord planOutput = new Models.PlanRecord();

        // Output plan
        // User's local plan is always id 0
        planOutput._id = "0";
        planOutput.name = planInput.name;
        planOutput.tactics = planInput.tactics;
        planOutput.affects_goal = planInput.affects_goal;
        planOutput.score = planScore;
        planOutput.default_affects = gradeInfo.default_affects;
        planOutput.pbc = hasPbc;
        planOutput.autonomy = hasAutonomy;
        planOutput.current_scenario = assignmentMatrix[hasPbc ? 1 : 0][hasAutonomy ? 1 : 0];

        return planOutput;

    }


}
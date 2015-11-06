using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using JsonFx.Json;
using Parse;
 
// TODO: This needs lots of cleanup
public class PlayerManager : MonoBehaviour {
    
    protected PlayerManager() {}
    private static PlayerManager _instance = null;
        
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
            return _submittedPlan;
        }
    }

    /// <summary>
    /// Get player's phase two completion status.
    /// </summary>
    public bool PhaseTwoDone {
        get {
            return _phaseTwoDone;
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

        Debug.Log("*** AUTH");

        if(response.ContainsKey("error"))  
        {          
            Events.instance.Raise(new PlayerLoginEvent(false, response["error"].ToString()));
            return;
        }

        // NetworkManager._userCookie = response["user_cookie"].ToString();

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

}
using UnityEngine;
using UnityEngine.Analytics;
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

    private bool _isAuthenticated;
    private string _playerId;

    private Models.Plan _userCurrentPlan;

    public void Authenticate(string email, string pass) {

        Dictionary<string, object> authFields = new Dictionary<string, object>();

        authFields.Add("email", email);
        authFields.Add("password", pass);

        /*ParseUser.LogInAsync(user.username, pass).ContinueWith(t =>
        {
            if (t.IsFaulted || t.IsCanceled)
            {
                // The login failed. Check the error to see why.
            }
            else
            {
                // Login was successful.
            }
        });*/

        NetworkManager.Instance.PostURL("/user/auth/", authFields, AuthCallback);
        
    }

    public void Register(string email, string username, string location, string pass, string passConfirm) {

        if(pass != passConfirm)
        {          
            Events.instance.Raise(new PlayerLoginEvent(false, "Password and Password Confirmation do not match!"));
            return;
        }

        Dictionary<string, object> registerFields = new Dictionary<string, object>();

        registerFields.Add("email", email);
        registerFields.Add("username", username);
        registerFields.Add("location", location);
        registerFields.Add("password", pass);

        var user = new ParseUser()
        {
            Username = username,
            Password = pass,
            Email = email
        };

        Task signUpTask = user.SignUpAsync();

        NetworkManager.Instance.PostURL("/user/create/", registerFields, AuthCallback);
        
    }

    public void AuthCallback(Dictionary<string, object> response) {

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
        _isAuthenticated = Convert.ToBoolean(response["auth"]);

        Events.instance.Raise(new PlayerLoginEvent(true));

        Analytics.SetUserId(_playerId);

        TrackEvent("User Login", "API");
        
        return;
        
    }

    public void SaveData(Dictionary<string, object> saveFields, Action<Dictionary<string, object>> response=null) {

        // Insert user ID
        saveFields.Add("user_id", _playerId);

        // Save user info
        NetworkManager.Instance.PostURL("/user/save/", saveFields, response, true);
    }

    public void TrackEvent(string strEventName, string strEventCategory) {

        #if UNITY_EDITOR
            return;
        #endif

        var parseFields = new Dictionary<string, string>() {{ "user", _playerId }};
        Dictionary<string, object> postFields = new Dictionary<string, object>() {{ "eventName", strEventName }, { "eventCategory", strEventCategory }, { "userId", _playerId }};

        // Send to Parse SDK
        // ParseAnalytics.TrackEventAsync(strEventName, parseFields);

        Analytics.CustomEvent(strEventName, new Dictionary<string, object>() {{ "eventCategory", strEventCategory }, { "userId", _playerId }});

        // Send analytic event
        NetworkManager.Instance.PostURL("/analytics/event/", postFields, null, true);

        Debug.Log("Track Event: '" + strEventName + "'");

    }

}
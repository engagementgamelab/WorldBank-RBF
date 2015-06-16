using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using JsonFx.Json;
 
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

        NetworkManager.Instance.PostURL(DataManager.config.serverRoot + "/user/auth/", authFields, AuthCallback);
        
    }

    public void Register(string email, string username, string location, string pass, string passConfirm) {

        if(pass != passConfirm)
        {          
            Events.instance.Raise(new PlayerFormEvent("Password and Password Confirmation do not match!"));
            return;
        }

        Dictionary<string, object> registerFields = new Dictionary<string, object>();

        registerFields.Add("email", email);
        registerFields.Add("username", username);
        registerFields.Add("location", location);
        registerFields.Add("password", pass);

        NetworkManager.Instance.PostURL(DataManager.config.serverRoot + "/user/create/", registerFields, AuthCallback);
        
    }

    public void AuthCallback(Dictionary<string, object> response) {

        if(response.ContainsKey("error"))  
        {          
            Events.instance.Raise(new PlayerFormEvent(response["error"].ToString()));
            return;
        }

        System.Text.StringBuilder output = new System.Text.StringBuilder();
        
        JsonWriter writer = new JsonWriter (output);
        
        writer.Write(response["user"]);

        // Set user info
        Models.User user = JsonReader.Deserialize<Models.User>(output.ToString());

        _playerId = user._id;
        _isAuthenticated = Convert.ToBoolean(response["auth"]);
        
    }

    public void SaveData(Dictionary<string, object> saveFields, Action<Dictionary<string, object>> response=null) {

        // Insert user ID
        saveFields.Add("user_id", _playerId);

        // Save user info
        NetworkManager.Instance.PostURL(DataManager.config.serverRoot + "/user/save/", saveFields, response, true);
    }

}
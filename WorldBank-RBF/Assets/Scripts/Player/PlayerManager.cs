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

    public static bool _isAuthenticated;
    public static string _userId;

    private static Models.Plan _userCurrentPlan;

    public void Authenticate() {

        Dictionary<string, object> authFields = new Dictionary<string, object>();

        authFields.Add("email", "johnny@elab.emerson.edu");
        authFields.Add("password", "password");

        NetworkManager.Instance.PostURL(DataManager.config.serverRoot + "/user/auth/", authFields, AuthCallback);
        
    }

    public void AuthCallback(Dictionary<string, object> response) {

        System.Text.StringBuilder output = new System.Text.StringBuilder();
        
        JsonWriter writer = new JsonWriter (output);
        
        writer.Write(response["user"]);

        // Debug.Log(response["user"].ToObject<Models.User>());

        // Set user info
        Models.User user = JsonReader.Deserialize<Models.User>(output.ToString());

        Debug.Log(user.ToString());

        _userId = user._id;
        _isAuthenticated = Convert.ToBoolean(response["auth"]);

        // PlayerData.UnlockImplementation("unlockable_use_ngo");
    }

    public void SaveData(string[] data) {

        Dictionary<string, object> saveFields = new Dictionary<string, object>();

        _userCurrentPlan = new Models.Plan();
        _userCurrentPlan.unlocks = data;
        _userCurrentPlan.pbc = true;
        
        saveFields.Add("user_id", _userId);
        saveFields.Add("plan", _userCurrentPlan);

        // Save user info
        NetworkManager.Instance.PostURL(DataManager.config.serverRoot + "/user/save/", saveFields, null, true);
    }
}
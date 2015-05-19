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
    public static long _userId;

    public void Authenticate() {

        Dictionary<string, object> authFields = new Dictionary<string, object>();

        authFields.Add("email", "johnny@elab.emerson.edu");
        authFields.Add("password", "password");

        NetworkManager.Instance.PostURL(DataManager.config.serverRoot + "/user/auth/", authFields, AuthCallback);
        
    }

    public void AuthCallback(Dictionary<string, object> response) {

        // Set user info
        // Models.User user = JsonReader.Deserialize<Models.User>(response["user"].ToString());

        // Debug.Log(response["user"].ToString());
        // _userId = user._id;
        _isAuthenticated = Convert.ToBoolean(response["auth"]);

        // PlayerData.UnlockImplementation("unlockable_incentivise_improvement");

    }

    public void SaveData(string[] data) {

        Dictionary<string, object> saveFields = new Dictionary<string, object>();

        saveFields.Add("user_id", _userId);
        saveFields.Add("unlocks", data);

        // Save user info
        NetworkManager.Instance.PostURL(DataManager.config.serverRoot + "/user/save/", saveFields);
    }
}
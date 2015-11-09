/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 DialogueManager.cs
 Unity networking manager.

 Created by Johnny Richardson on 4/3/15.
==============
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using JsonFx.Json;
 
public class NetworkManager : MonoBehaviour {

    protected NetworkManager() {}
    private static NetworkManager _instance = null;

    private static Action<Dictionary<string, object>> _currentResponseHandler;
    public static string _sessionCookie;
    public static string _userCookie;
    
    public delegate void OnServerDown();
    public delegate void OnNotLoggedIn();

    class PostCache {
        
        public string url;
        public Dictionary<string, object> fields;
        public Action<Dictionary<string, object>> responseHandler;

    }
    List<PostCache> _cachedRequests = new List<PostCache>();
        
    public static NetworkManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType (typeof (NetworkManager)) as NetworkManager;

                if (_instance == null) {
                    GameObject obj = new GameObject ();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent<NetworkManager> ();
                }
            }
            return _instance;
        }
    }

    public string Cookie {
        
        set {
            _sessionCookie = value;

            Debug.Log("Set cookie to " + _sessionCookie);

            // Client was authenticated -- call any requests that were waiting
            foreach(PostCache req in _cachedRequests)
                PostURL(req.url, req.fields, req.responseHandler);

            _cachedRequests.Clear();
        }

    }

    public static Action<Dictionary<string, object>> CurrentResponseHandler {
        
        get {
            return _currentResponseHandler;
        }
        set {
            _currentResponseHandler = value;
        }

    }

    /// <summary>
    /// Called when there is a server error.
    /// </summary>
    public OnServerDown onServerDown;

    /// <summary>
    /// Called when the user has no session.
    /// </summary>
    public OnNotLoggedIn onNotLoggedIn;

    public void Authenticate(Action<Dictionary<string, object>> responseHandler=null) {

        PostURL(
            "/auth/",
            new Dictionary<string, object>() {{ "key", DataManager.APIKey }},
            responseHandler
        );

    }
    
    /// <summary>
    /// Calls a URL endpoint and gives an optional response handler.
    /// </summary>
    /// <param name="url">The URL to get.</param>
    /// <param name="responseHandler">An Action that handles the response (optional).</param>
    public void GetURL(string url, Action<string> responseHandler=null) {

        string absoluteURL = DataManager.RemoteURL + url;
        
        if(responseHandler == null)
            StartCoroutine(WaitForRequest(absoluteURL));
        else
            StartCoroutine(WaitForRequest(absoluteURL, responseHandler));

    }


    /// <summary>
    /// Post to a a URL endpoint with required field(s) and gives an optional response handler.
    /// </summary>
    /// <param name="url">The URL to post to.</param>
    /// <param name="responseHandler">An Action that handles the response (optional).</param>
    public void PostURL(string url, Dictionary<string, object> fields, Action<Dictionary<string, object>> responseHandler=null) {

        string absoluteURL = DataManager.RemoteURL + url;

        // Send form as raw byte array
        System.Text.StringBuilder output = new System.Text.StringBuilder();
        
        JsonWriter writer = new JsonWriter (output);
        
        if(_sessionCookie != null)
            fields.Add("sessionID", System.Convert.ChangeType(_sessionCookie, typeof(object)));
        
        // If no session cookie, client has no auth, so cache this request to do later
        else if(url != "/auth/") {
            PostCache cacheObj = new PostCache();
            cacheObj.url = url;
            cacheObj.fields = fields;
            cacheObj.responseHandler = responseHandler;
            _cachedRequests.Add(cacheObj);

            return;
        }

        writer.Write(fields);
     
        // Encode output as UTF8 bytes
        StartCoroutine(WaitForForm(absoluteURL, Encoding.UTF8.GetBytes(output.ToString()), responseHandler));
    
    }

    /// <summary>
    /// Download data from specified URL.
    /// </summary>
    /// <param name="url">The URL to download from.</param>
    public string DownloadDataFromURL(string url) {

        WebClient client = new WebClient();

        try {
            
            Stream data = client.OpenRead(DataManager.RemoteURL + url);

            StreamReader reader = new StreamReader(data);
            
            return reader.ReadToEnd();

        }
        catch(WebException e) {

            throw new Exception("Unable to download data from " + url + ": " + e.Message);

        }
    }

    IEnumerator WaitForRequest(string url, Action<string> responseAction=null)
     {
        Debug.Log("Requesting: " + url);

        WWW www = new WWW(url);

        yield return www;

        // Check for errors
        if (www.error == null) 
        {
            if(responseAction != null)
            {
                // Respond w/ error text
                responseAction(www.text);
                yield return null;
            }
            else
                Debug.Log("WWW Ok!: " + www.text);
        }
        else
        {
            string exceptionMsg = "WaitForRequest error: " + www.error;
            
            throw new Exception(exceptionMsg);
        }
     }

    IEnumerator WaitForForm(string url, byte[] formData=null, Action<Dictionary<string, object>> responseAction=null, WWWForm form=null)
     {
        WWW www;

        // Specified raw form as JSON
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");

        // This is a raw UTF8 form
        if(form == null && formData != null) {
        
            if(_sessionCookie != null)
                postHeader.Add("x-sessionID", _sessionCookie);

            Debug.Log("PostURL: " + System.Text.Encoding.UTF8.GetString(formData));
        
           www = new WWW(url, formData, postHeader);
        }
        
        // Bail out!
        else
            throw new Exception("WaitForForm: both form and form byte data not specified.");
        
        yield return www;

        Debug.Log(www.text);

        // Deserialize the response and handle it below
        Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(www.text);

        Debug.Log(response);
        // User is not logged in
        if((www.responseHeaders.Count > 0) && www.responseHeaders.ContainsKey("STATUS") && www.responseHeaders["STATUS"].ToString().Contains("401"))
        {
            Debug.LogError("User is not logged in. Call to " + url + " rejected.");
            
            onNotLoggedIn();
        }
        // check for errors
        else if (www.error == null) 
        {

            if(responseAction != null)
            {
                responseAction(response);
                yield return null;
            }
            else
                Debug.Log("WWW Ok!: " + www.text);
        
        }
        else
        {
            string exceptionMsg = "WaitForForm unknown error. No response to parse and no registered callback.";

            if(response == null)
            {
                if(www.error.Equals("couldn't connect to host"))
                    onServerDown();

                // If in editor, always throw so we catch issues
                #if UNITY_EDITOR
                    exceptionMsg = "General WWW issue: " + www.error;
                    throw new Exception(exceptionMsg);
                #endif
            }
            else if(responseAction != null && !response.ContainsKey("error")) 
            {
                Debug.Log(response);
                responseAction(response);
                yield return null;
            }
            else
            {
                Debug.Log(response);
                responseAction (response);
            }
            
        }
     }

}
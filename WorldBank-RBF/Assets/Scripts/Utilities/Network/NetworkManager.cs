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
    private static string _sessionCookie;
    public static string _userCookie;
        
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

    public static Action<Dictionary<string, object>> CurrentResponseHandler {
        
        get {
            return _currentResponseHandler;
        }
        set {
            _currentResponseHandler = value;
        }

    }

    public void Authenticate() {

        PostURL(
            "/auth/",
            new Dictionary<string, object>() {{ "key", DataManager.APIKey }},
            ClientAuthenticated
        );

    }

    private void ClientAuthenticated(Dictionary<string, object> response) { 

        _sessionCookie = response["session_cookie"].ToString();

        _currentResponseHandler(response);

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
    /// <param name="rawPost">The form should be sent as a byte array (default is true).</param>
    public void PostURL(string url, Dictionary<string, object> fields, Action<Dictionary<string, object>> responseHandler=null, bool rawPost=true) {

        string absoluteURL = DataManager.RemoteURL + url;

        // Send form as raw byte array?
        if(rawPost) {

            System.Text.StringBuilder output = new System.Text.StringBuilder();
            
            JsonWriter writer = new JsonWriter (output);
            
            if(_sessionCookie != null)
                fields.Add("sessionID", System.Convert.ChangeType(_sessionCookie, typeof(object)));

            writer.Write(fields);
         
            // Encode output as UTF8 bytes
            StartCoroutine(WaitForForm(absoluteURL, Encoding.UTF8.GetBytes(output.ToString()), responseHandler));

        }
        else {

            WWWForm form = new WWWForm();
            
            // Create WWWForm
            foreach(KeyValuePair<string, object> field in fields)
            {
                string formFieldVal = null;

                // If the field passed in is not a string (likely a model object), serialize to json string
                try {
                    formFieldVal = field.Value.ToString();
                    form.AddField(field.Key, formFieldVal);
                }
                catch(Exception e) {
                    throw new Exception("Unable to coerce form field " + field.Value + " to string for POST to " + url);
                }
            }

            StartCoroutine(WaitForForm(absoluteURL, form.data, responseHandler));

        }
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
        
           www = new WWW(url, formData, postHeader);
        }
        // This is a WWWForm
        /*else if(form != null) {
        
            if(_sessionCookie != null)
                postHeader.Add("sessionID", _sessionCookie);

           www = new WWW(url, form, postHeader);
        }*/
        
        // Bail out!
        else
            throw new Exception("WaitForForm: both form and form byte data not specified.");
        
        yield return www;

        // Deserialize the response and handle it below
        Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(www.text);

        // check for errors
        if (www.error == null) 
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
                exceptionMsg = "General WWW issue: " + www.error;
                throw new Exception(exceptionMsg);
            }
            else if(responseAction != null) 
            {
                responseAction(response);
                yield return null;
            }
            else 
            {
               exceptionMsg = "API Request Error: " + response["error"];
                throw new Exception(exceptionMsg);
            }
            
        }
     }

}
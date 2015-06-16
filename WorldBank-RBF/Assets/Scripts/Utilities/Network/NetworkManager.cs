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
 
// TODO: This needs lots of cleanup
public class NetworkManager : MonoBehaviour {
    protected NetworkManager() {}
    private static NetworkManager _instance = null;
        
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

    public void GetURL(string url, Action<string> responseHandler=null) {
        
        
        if(responseHandler == null)
            StartCoroutine(WaitForRequest(url));
        else
            StartCoroutine(WaitForRequest(url, responseHandler));

    }

    public void PostURL(string url, Dictionary<string, object> fields, Action<Dictionary<string, object>> response=null, bool rawPost=false) {

        if(rawPost) {

            System.Text.StringBuilder output = new System.Text.StringBuilder();
            
            JsonWriter writer = new JsonWriter (output);
            
            writer.Write(fields);

            Debug.Log(output.ToString());
         
            StartCoroutine(WaitForForm(url, Encoding.UTF8.GetBytes(output.ToString()), response));

        }
        else {

            WWWForm form = new WWWForm();
            
            foreach(KeyValuePair<string, object> field in fields)
            {
                string formFieldVal = null;

                // If the field passed in is not a string (likely a model object), serialize to json string
                try {
                    formFieldVal = field.Value.ToString();
                    form.AddField(field.Key, formFieldVal);
                }
                catch(Exception e) {
                    throw new Exception("Unable to coerce form field " + field.Value + " to string");
                }
            }

            StartCoroutine(WaitForForm(url, null, response, form));

        }
    }



    public string DownloadDataFromURL(string url) {

        WebClient client = new WebClient();

        try {
            
            Stream data = client.OpenRead(DataManager.config.serverRoot + url);

            StreamReader reader = new StreamReader(data);
            
            return reader.ReadToEnd();

        }
        catch(Exception e) {

            throw new Exception("Unable to download data from " + url + ": " + e);

        }
    }

    IEnumerator WaitForRequest(string url, Action<string> responseAction=null)
     {
        Debug.Log("Requesting: " + url);

        WWW www = new WWW(url);

        yield return www;
        // check for errors
        if (www.error == null) 
        {
            if(responseAction != null)
            {
                responseAction(www.text);
                yield return null;
            }
            else
                Debug.Log("WWW Ok!: " + www.text);
        }
        else
        {
            string exceptionMsg = "WaitForRequest unknown error.";
            
            throw new Exception(exceptionMsg);
        }
     }

    IEnumerator WaitForForm(string url, byte[] formData=null, Action<Dictionary<string, object>> responseAction=null, WWWForm form=null)
     {
        WWW www; 

        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");

        if(form == null && formData != null)
           www = new WWW(url, formData, postHeader);
        else if(form != null)
           www = new WWW(url, form);
        else
            throw new Exception("WaitForForm: both form and form byte data not specified.");
        
        yield return www;

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
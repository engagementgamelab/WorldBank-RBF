using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
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

    public void GetURL(string url, string enumCallback) {
        
        StartCoroutine(WaitForRequest(url));

    }

    public void PostURL(string url, Dictionary<string, object> fields, Action<Dictionary<string, object>> response=null) {

        WWWForm form = new WWWForm();

        foreach(KeyValuePair<string, object> field in fields)
        {
            string formFieldVal = null;

            // If the field passed in is not a string (likely a model object), serialize to json string
            if(field.Value.GetType() != typeof(string))
            {
                System.Text.StringBuilder output = new System.Text.StringBuilder();
                
                JsonWriter writer = new JsonWriter (output);
                
                writer.Write(field.Value);
                formFieldVal = output.ToString();
                
                Debug.Log(formFieldVal);
            }
            else
            {
                try {
                    formFieldVal = field.Value.ToString();
                }
                catch(Exception e) {
                    throw new Exception("Unable to coerce form field " + field.Value + "to string");
                }
            }

            form.AddField(field.Key, formFieldVal);
        }

        StartCoroutine(WaitForForm(url, form, response));
        
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

    public void GetJSON (string url) {
        GetURL(url, "GetJSONResponse");
    }

    IEnumerator WaitForRequest(string url)
     {
        WWW www = new WWW(url);

        yield return www;

        // check for errors
        if (www.error == null)
            Debug.Log("WWW Ok!: " + www.text);
        else
            Debug.Log("WWW Error: "+ www.error);

     }

    IEnumerator WaitForForm(string url, WWWForm form, Action<Dictionary<string, object>> responseAction=null)
     {
        WWW www = new WWW(url, form);
        
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
               exceptionMsg = "General WWW issue: " + www.error;
            else if(responseAction != null)
               exceptionMsg = "API Request Error: " + response["error"];
            
            throw new Exception(exceptionMsg);
        }
     }


    IEnumerator GetJSONResponse(WWW www)
     {
         yield return www;
     
         // check for errors
         if (www.error == null)
         {
             string strRes = www.text;

             Debug.Log("WWW Ok!: " + strRes);
         } else {
             Debug.Log("WWW Error: "+ www.error);
         }    
     }
}
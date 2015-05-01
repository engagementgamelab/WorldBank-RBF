using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.IO;
 
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

    public void GetURL (string url, string enumCallback) {
        WWW www = new WWW(url);
        StartCoroutine(WaitForRequest(www));
        
    }

    public string DownloadDataFromURL(string url) {
        WebClient client = new WebClient();

        Stream data = client.OpenRead(DataManager.config.serverRoot + url);

        StreamReader reader = new StreamReader(data);
        
        return reader.ReadToEnd();
    }

    public void GetJSON (string url) {
        GetURL(url, "GetJSONResponse");
    }

    IEnumerator WaitForRequest(WWW www)
     {
         yield return www;
     
         // check for errors
         if (www.error == null)
         {
             Debug.Log("WWW Ok!: " + www.text);
         } else {
             Debug.Log("WWW Error: "+ www.error);
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
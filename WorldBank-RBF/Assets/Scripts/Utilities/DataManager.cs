/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 DataManager.cs
 Unity data storage handler.

 Created by Johnny Richardson on 4/7/15.
==============
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

class DataManager {

    private static JSONNode nodes;
    public static string serverRoot;

    public static void SetGameConfig(string data)
    {
        JSONNode configJson = JSON.Parse(data);

        Debug.Log(configJson);
        
        serverRoot = configJson["server_root"];

    }

    public static void SetGameData(string data)
    {

        nodes = JSON.Parse(data)["content"];

        Debug.Log(nodes[1]["phase_one"]);

    }

    public static JSONArray GetDataForPhase(string strPhase)    {
        return nodes[strPhase].AsArray;
    }

}
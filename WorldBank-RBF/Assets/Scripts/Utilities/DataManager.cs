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
using System.IO;
using JsonFx.Json;

// TODO: Cleanup!!
public class DataManager {

    public static string serverRoot;

    private static JsonReaderSettings _readerSettings = new JsonReaderSettings();
    private static Models.GameData currentGameData;

    public static void SetGameConfig(string data)
    {
        _readerSettings.TypeHintName = "__type";

        JsonReader reader = new JsonReader(data, _readerSettings);
        Dictionary<string, object> configDict = (Dictionary<string, object>)reader.Deserialize();

        serverRoot = configDict["server_root"].ToString();

    }

    public static void SetGameData(string data)
    {
        currentGameData = JsonReader.Deserialize<Models.GameData>(data);

        // create file in Assets/Config/
        #if !UNITY_WEBPLAYER
            File.WriteAllText(Application.dataPath + "/Config/data.json", data); 
        #endif

    }

    public static Models.NPC[] GetDataForCity(string strCityName)    {
        
        return currentGameData.phase_one[strCityName];

    }

    public static Models.City[] GetCityData()    {
        
        return currentGameData.cities;

    }

    public static Models.Character GetDataForCharacter(string strCharName)    {
        
        int characterIndex = Array.FindIndex(currentGameData.characters, row => row.symbol == strCharName);

        if(characterIndex == -1)
            throw new Exception("Unable to find Character with symbol '" + strCharName + "'");
     
        return currentGameData.characters[characterIndex];

    }
    
}
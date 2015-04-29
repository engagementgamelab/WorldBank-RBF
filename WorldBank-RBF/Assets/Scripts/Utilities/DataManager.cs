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
    public static string currentSceneContext;

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
        // Set global data only if there is none
        if(currentGameData != null) 
            return;

        currentGameData = JsonReader.Deserialize<Models.GameData>(data);

        // create/save to file in Assets/Config/
        #if !UNITY_WEBPLAYER    
<<<<<<< HEAD
            using (StreamWriter outfile = new StreamWriter(Application.dataPath + "/Resources/Config/data.json"))
=======
            using (StreamWriter outfile = new StreamWriter(Application.dataPath + "/Resources/data.json"))
>>>>>>> ec4b5cac4efb86a669e7c9ba9420f808bbf99b3c
            {
                outfile.Write(data);
            }
        #endif

    }

    public static void SetSceneContext(string context) {

        currentSceneContext = context;

    }

    public static Models.City[] GetAllCities()    {
        
        return currentGameData.cities;

    }

    public static Models.City GetCityInfo(string strCityName)    {
        
        foreach(Models.City city in currentGameData.cities)
        {
            if(city.symbol == strCityName)
                return city;
        }

        return null;

    }

    public static Models.NPC[] GetNPCsForCity(string strSelector=null)    {
        
        if(strSelector == null)
            return currentGameData.phase_one[currentSceneContext];
        else
            return new Models.NPC[] { Array.Find(currentGameData.phase_one[currentSceneContext], npc => npc.character == strSelector) };

    }

    public static Models.Character GetDataForCharacter(string strCharName)    {
        
        int characterIndex = Array.FindIndex(currentGameData.characters, row => row.symbol == strCharName);

        if(characterIndex == -1)
            throw new Exception("Unable to find Character with symbol '" + strCharName + "'");
     
        return currentGameData.characters[characterIndex];

    }
    
}
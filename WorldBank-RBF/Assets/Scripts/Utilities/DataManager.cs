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
class DataManager {

    public static string serverRoot;

    private static JsonReaderSettings _readerSettings = new JsonReaderSettings();
    private static GameData currentGameData;
        
    public class GameData {

        public Character[] characters { get; set; }
		public PhaseOne phase_one { get; set; }
        public Dictionary<string, object> phase_two { get; set; }

    }

    [System.Serializable]
    public class CityStruct {

        public Dictionary<string, object> city = new Dictionary<string, object>();

    }

    [System.Serializable]
    public class City {

        public string symbol { get; set; }
        public string display_name { get; set; }
        public string description { get; set; }

    }

    public class NPC {

        public string symbol { get; set; }
        public string character { get; set; }
        public List<Dictionary<string, object>> dialogue = new List<Dictionary<string, object>>();

    }

    [System.Serializable]
    public class Character {

        public string symbol { get; set; }
        public string display_name { get; set; }
        public string description { get; set; }

    }

    [System.Serializable]
    public class PhaseOne {

        public List<Dictionary<string, object>>[] city { get; set; }

    }

    public class Characters {
        public List<Dictionary<string, object>> dialogue = new List<Dictionary<string, object>>();
    }

    public static void SetGameConfig(string data)
    {
        _readerSettings.TypeHintName = "__type";

        JsonReader reader = new JsonReader(data, _readerSettings);
        Dictionary<string, object> configDict = (Dictionary<string, object>)reader.Deserialize();

        serverRoot = configDict["server_root"].ToString();

    }

    public static void SetGameData(string data)
    {
        
        JsonReader reader = new JsonReader(data, _readerSettings);

        currentGameData = JsonReader.Deserialize<GameData>(data);

        Debug.Log(currentGameData.characters[0].symbol);
        Debug.Log(currentGameData.phase_one);
        Debug.Log(currentGameData.phase_two);

        // create file in Assets/Config/
        #if !UNITY_WEBPLAYER
            File.WriteAllText(Application.dataPath + "/Config/data.json", data); 
        #endif

    }


    // public static Dictionary<string, object> GetDataForCity(string strCityName)    {
        
    //     return currentGameData.phase_one[strCityName];

    // }
    
}
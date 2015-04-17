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
    private static GameData currentGameData;
        
    public class GameData {

        public Character[] characters { get; set; }
		public Dictionary<string, NPC[]> phase_one { get; set; }
        public Dictionary<string, object> phase_two { get; set; }

    }

    [System.Serializable]
    public class Character {

        public string symbol { get; set; }
        public string display_name { get; set; }
        public string description { get; set; }

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
		public Dictionary<string, Dictionary<string, string>> dialogue { get; set; }

    }

    public class Dialogue {

        public string symbol { get; set; }
        public string character { get; set; }
        public Dictionary<string, object> dialogue { get; set; }

    }

	public class PhaseOne {
		public Dictionary<string, object>[] npcs { get; set; }
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

        // create file in Assets/Config/
        #if !UNITY_WEBPLAYER
            File.WriteAllText(Application.dataPath + "/Config/data.json", data); 
        #endif

    }


    public static NPC[] GetDataForCity(string strCityName)    {
        
        return currentGameData.phase_one[strCityName];

    }

    public static Character GetDataForCharacter(string strCharName)    {
        
        int characterIndex = Array.FindIndex(currentGameData.characters, row => row.symbol == strCharName);

        if(characterIndex == -1)
            throw new Exception("Unable to find Character with symbol '" + strCharName + "'");
     
        return currentGameData.characters[characterIndex];

    }
    
}
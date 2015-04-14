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
using SimpleJSON;

// TODO: Cleanup!!
class DataManager {

    private static JSONNode nodes;
    public static string serverRoot;

    public class TestClass {

        public string test { get; set; }
        public string hi { get; set; }

    }

    public class GameData {

        public Character[] characters { get; set; }
        public Dictionary<string, object> phase_one { get; set; }
        public Dictionary<string, object> phase_two { get; set; }

    }

    public class City {

        public string symbol { get; set; }
        public string display_name { get; set; }
        public string description { get; set; }

    }

    public class NPC {

        public string symbol { get; set; }
        public List<string> dialogue = new List<string>();

    }

    [System.Serializable]
    public class Character {

        public string symbol { get; set; }
        public string display_name { get; set; }
        public string description { get; set; }

    }

    public class Characters {
        public List<Character> chars = new List<Character>();
    }

    public static void SetGameConfig(string data)
    {
        JSONNode configJson = JSON.Parse(data);

        // Debug.Log(configJson);
        
        serverRoot = configJson["server_root"];

    }

    public static void SetGameData(string data)
    {

        JsonReaderSettings readerSettings = new JsonReaderSettings();
        readerSettings.TypeHintName = "__type";
        
        JsonReader reader = new JsonReader(data, readerSettings);

        // nodes = JSON.Parse(data);
        var deserialized = JsonReader.Deserialize<GameData>(data);
        Dictionary<string, object> dict = (Dictionary<string, object>)reader.Deserialize();

        Debug.Log(deserialized.characters[0].symbol);
        Debug.Log(deserialized.phase_one);
        Debug.Log(deserialized.phase_two);

        // create file in Assets/Config/
        #if !UNITY_WEBPLAYER
            File.WriteAllText(Application.dataPath + "/Config/data.json", data); 
        #endif

    }

    public static JSONNode GetDataForPhase(string strPhase)    {
        return nodes[strPhase];
    }


    public static Dictionary<string, string> GetDataForCity(string strCityName)    {

        Dictionary<string, string> dictCityData = new Dictionary<string, string>();

        JSONNode cityData = DataManager.GetDataForPhase("phase_one")[strCityName];

		Debug.Log (DataManager.GetDataForPhase ("phase_one") [strCityName]);
        
//        while(cityData.MoveNext())
//        {
//			KeyValuePair<object, object> kvp = DataManager.GetKVP(cityData.Current);
//			JSONNode node = kvp.Value as JSONNode;
//			IEnumerator nodeEnum = node.AsObject.GetEnumerator();
//
//			dictCityData.Add(kvp.Key.ToString(), node.ToString());
//
//        }

        return dictCityData;
    }
    
}
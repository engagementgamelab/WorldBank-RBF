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

// TODO: Cleanup!!
class DataManager {

    private static JSONNode nodes;
    public static string serverRoot;

    public static void SetGameConfig(string data)
    {
        JSONNode configJson = JSON.Parse(data);

        // Debug.Log(configJson);
        
        serverRoot = configJson["server_root"];

    }

    public static void SetGameData(string data)
    {

        nodes = JSON.Parse(data);

    }

    public static JSONNode GetDataForPhase(string strPhase)    {
        return nodes[strPhase];
    }


    public static Dictionary<string, IEnumerator> GetDataForCity(string strCityName)    {

        Dictionary<string, IEnumerator> dictCityData = new Dictionary<string, IEnumerator>();

        IEnumerator cityData = DataManager.GetDataForPhase("phase_one")[strCityName].AsObject.GetEnumerator();
        
        while(cityData.MoveNext())
        {
			KeyValuePair<object, object> kvp = DataManager.GetKVP(cityData.Current);
			JSONNode node = kvp.Value as JSONNode;
			IEnumerator nodeEnum = node.AsObject.GetEnumerator();

			dictCityData.Add(kvp.Key.ToString(), nodeEnum);

        }

        return dictCityData;
    }

    public static KeyValuePair<object,object> GetKVP(object currentData)
    {
        var dataType = currentData.GetType();

        if (dataType.IsGenericType)
        {
            if (dataType == typeof (KeyValuePair<string,JSONNode>))
            {
                var key = dataType.GetProperty("Key");
                var value = dataType.GetProperty("Value");

                var keyObj = key.GetValue(currentData, null);
                var valueObj = value.GetValue(currentData, null);

               return new KeyValuePair<object,object>(keyObj, valueObj);
            }
        }

        return new KeyValuePair<object,object>(null, null);
    }

}
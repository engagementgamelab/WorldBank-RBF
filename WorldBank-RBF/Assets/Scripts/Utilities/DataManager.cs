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

public class DataManager {

    public static string currentSceneContext;

    private static JsonReaderSettings _readerSettings = new JsonReaderSettings();

    public static Models.GameConfig config;
    private static Models.GameData gameData;

    /// <summary>
    /// Set global game config data, such as API endpoints, given a valid input string
    /// </summary>
    /// <param name="data">Data to be used to set config; must conform to GameConfig model.</param>
    public static void SetGameConfig(string data)
    {

        // Set config only if there is none set
        if(config != null) 
            return;

        // Set global config
        config = JsonReader.Deserialize<Models.GameConfig>(data);

    }

    /// <summary>
    /// Set global game data, given a valid input string
    /// </summary>
    /// <param name="data">String to be used to set game data; must conform to GameData model.</param>
    public static void SetGameData(string data)
    {
        // Set global data only if there is none
        if(gameData != null) 
            return;

        gameData = JsonReader.Deserialize<Models.GameData>(data);

        // create/save to file in Assets/Resources/Config/
        #if !UNITY_WEBPLAYER    
            using (StreamWriter outfile = new StreamWriter(Application.dataPath + "/Resources/data.json"))
            {
                outfile.Write(data);
            }
        #endif

    }

    /// <summary>
    /// Set the current scene context, which is essentially a key name used for global data lookup (e.g. a city)
    /// </summary>
    /// <param name="strContext">String to be used as the scene context</param>
    public static void SetSceneContext(string strContext) {

        currentSceneContext = strContext;

    }

    /// <summary>
    /// Get all current cities available in game data.
    /// </summary>
    /// <returns>An array of Models.City.</returns>
    public static Models.City[] GetAllCities()    {
        
        return gameData.cities;

    }

    /// <summary>
    /// Get a reference to a particular city in the game, given its name
    /// </summary>
    /// <param name="strCityName">Name of the city</param>
    /// <returns>The Models.City for the given city</returns>
    public static Models.City GetCityInfo(string strCityName)    {
        
        foreach(Models.City city in gameData.cities)
        {
            if(city.symbol == strCityName)
                return city;
        }

        return null;

    }

    /// <summary>
    /// Get data for NPC with name specified, or all NPCs in current city.
    /// </summary>
    /// <returns>(Optional) Symbol of the character to get NPC data for; if not used all NPCs in current city are returned.</returns>
    public static Models.NPC[] GetNPCsForCity(string strSelector=null)    {
        
        if(strSelector == null)
            return gameData.phase_one[currentSceneContext];
        else {
            
            Models.NPC[] npcRef = new Models.NPC[] { Array.Find(gameData.phase_one[currentSceneContext], row => row.character == strSelector) };

            if(npcRef == null)
                throw new Exception("Unable to find NPC with symbol '" + strSelector + "' for this city (" + currentSceneContext + ")! Fiddlesticks.");
            
            return npcRef;
        }

    }

    /// <summary>
    /// Get a reference to a particular Unlockable given its symbol
    /// </summary>
    /// <param name="strSymbol">Symbol of the unlockable</param>
    /// <returns>The Models.Unlockable for the symbol matching the input</returns>
    public static Models.Unlockable GetUnlockableBySymbol(string strSymbol) {

        Models.Unlockable unlockRef = new Models.Unlockable[] { Array.Find(gameData.unlockables, unlockable => unlockable.symbol == strSymbol) }[0];

        if(unlockRef == null)
            throw new Exception("Unable to find Unlockable with symbol '" + strSymbol + "'! Uh oh.");
        
        return unlockRef;
    }

    /// <summary>
    /// Get a character's data given its symbol
    /// </summary>
    /// <param name="strCharSymbol">Symbol of the character</param>
    /// <returns>The Models.Character for the symbol matching the input</returns>
    public static Models.Character GetDataForCharacter(string strCharSymbol)    {
        
        int characterIndex = Array.FindIndex(gameData.characters, row => row.symbol == strCharSymbol);

        if(characterIndex == -1)
            throw new Exception("Unable to find Character with symbol '" + strCharSymbol + "'! Uh oh.");
     
        return gameData.characters[characterIndex];

    }
    
}
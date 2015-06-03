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
using System.Linq;
using JsonFx.Json;

public class DataManager {

    public static string currentSceneContext;

    private static JsonReaderSettings _readerSettings = new JsonReaderSettings();

    public static Models.GameConfig config;
    public static List<string> tacticNames;

    private static Models.GameData gameData;

    private static Models.ScenarioCard[] currentScenario;

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

        try {

            System.Text.StringBuilder output = new System.Text.StringBuilder();
            // _readerSettings.AddTypeConverter (new Models.GameDataConverter());

            JsonReader reader = new JsonReader(data, _readerSettings);

            
            gameData = reader.Deserialize<Models.GameData>();

            // Store current tactic names in a list
            tacticNames = gameData.phase_two.tactics.Select(tactic => tactic.tactic_name).ToList();

        }
        catch(JsonDeserializationException e) {
            throw new Exception("Unable to set game data: " + e.Message);
        }

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
    public static Models.NPC[] GetNPCsForCity(string strSelector=null) {
        
        if(strSelector == null) {
            return gameData.phase_one[currentSceneContext];
        } else {

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
    /// Get the phase two scenario card specified by the index input.
    /// </summary>
    /// <param name="cardIndex">Index of the scenario card</param>
    /// <returns>The Models.ScenarioCard for the symbol matching the input</returns>
    public static Models.ScenarioCard GetScenarioCardByIndex(int cardIndex) {

        if(currentScenario == null)
            currentScenario =  gameData.phase_two.GetScenario(currentSceneContext);

        Models.ScenarioCard scenarioRef = currentScenario[cardIndex];
        
        return scenarioRef;
    }

    /// <summary>
    /// Get the phase two tactic card specified by the tactic's name.
    /// </summary>
    /// <param name="cardName">Index of the scenario card</param>
    /// <returns>The Models.TacticCard for the symbol matching the input</returns>
    public static Models.TacticCard GetTacticCardByName(string cardName) {

        Models.TacticCard tacticRef = gameData.phase_two.tactics.FirstOrDefault(card => card.tactic_name == cardName);
        
        return tacticRef;
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
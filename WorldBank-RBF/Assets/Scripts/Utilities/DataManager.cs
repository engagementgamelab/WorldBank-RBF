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
    
    public static string RemoteURL {
        get {
            #if UNITY_EDITOR
               return config.serverLocalRoot;
            #else
               return config.serverRoot;
            #endif
        }
    }
    
    /// <summary>
    /// Get/set the current scene context, which is essentially a key name used for global data lookup (e.g. a city)
    /// </summary>
    public static string SceneContext {
        get {
            return currentSceneContext;
        }
        set {
            currentSceneContext = value;
        }
    }

    private static string currentSceneContext;

    private static JsonReaderSettings _readerSettings = new JsonReaderSettings();

    public static Models.GameConfig config;
    public static List<string> tacticNames;

    private static Models.GameData gameData;
    private static Models.GameDataTest gameDataTest;

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
            // gameDataTest = reader.Deserialize<Models.GameDataTest>();

            // Store current tactic names in a list
            if(gameData.phase_two.tactics != null && gameData.phase_two.tactics.Length > 0)
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
    public static Models.City GetCityInfo(string strCityName) {
        
        foreach(Models.City city in gameData.cities)
        {
            if(city.symbol == strCityName)
                return city;
        }

        return null;

    }

    /// <summary>
    /// Get all current routes available in game data.
    /// </summary>
    /// <returns>An array of Models.Route.</returns>
    public static Models.Route[] GetAllRoutes ()    {
        
        try {
            return gameData.routes;
        } catch {
            throw new Exception ("Could not load routes. Make sure the MapCanvas is disabled before entering play mode.");
        }

    }

    /// <summary>
    /// Get a reference to a particular route in the game, given its name
    /// </summary>
    /// <param name="strCityName">Name of the route</param>
    /// <returns>The Models.Route for the given route</returns>
    public static Models.Route GetRouteInfo(string strRouteName) {
        
        foreach(Models.Route route in gameData.routes)
        {
            if(route.symbol == strRouteName)
                return route;
        }

        return null;

    }

    /// <summary>
    /// Get data for NPC with name specified, or all NPCs in current city.
    /// </summary>
    /// <returns>(Optional) Symbol of the character to get NPC data for; if not used all NPCs in current city are returned.</returns>
    public static Models.NPC[] GetNPCsForCity(string strSelector=null) {
        
        if(strSelector == null) {
            try {
                return gameData.phase_one[currentSceneContext];
            } catch (System.Exception e) {
                throw new System.Exception ("No city with the symbol '" + currentSceneContext + "' could be found.");
            }
        } else {

            Models.NPC[] npcRef = new Models.NPC[] { Array.Find(gameData.phase_one[currentSceneContext], row => row.character == strSelector) };
            
            if(npcRef[0] == null)
                throw new Exception("Unable to find NPC with symbol '" + strSelector + "' for this city (" + currentSceneContext + ")! Fiddlesticks.");
            
            return npcRef;
        }
        
    }

    public static int GetCityNPCCount (string citySymbol) {
        return gameData.phase_one[citySymbol].Length;
    }

    /// <summary>
    /// Get a reference to a particular Unlockable given its symbol
    /// </summary>
    /// <param name="strSymbol">Symbol of the unlockable</param>
    /// <returns>The Models.Unlockable for the symbol matching the input</returns>
    public static Models.Unlockable GetUnlockableBySymbol(string strSymbol) {
        
        Models.Unlockable unlockRef = new Models.Unlockable[] { 
            Array.Find(gameData.unlockables, unlockable => unlockable.symbol == strSymbol) }[0];

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
        {
            currentScenario =  gameData.phase_two.GetScenario(currentSceneContext);
            Array.Sort(currentScenario);
        }

        Models.ScenarioCard scenarioRef = currentScenario[cardIndex];
        
        return scenarioRef;
    }

    /// <summary>
    /// Get the length of the current scenario.
    /// </summary>
    /// <returns>Scenario length (int)</returns>
    public static int ScenarioLength() { return gameData.phase_two.GetScenario(currentSceneContext).Length; }

    /// <summary>
    /// Get the phase two tactic card specified by the tactic's name.
    /// </summary>
    /// <param name="cardName">Index of the scenario card</param>
    /// <returns>The Models.TacticCard for the symbol matching the input</returns>
    public static Models.TacticCard GetTacticCardByName(string cardName) {

        Models.TacticCard tacticRef = gameData.phase_two.tactics.FirstOrDefault(card => card.tactic_name == cardName);

        if(tacticRef == null)
            throw new Exception("Unable to find TacticCard with tactic name '" + cardName + "'! Damn.");
                
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
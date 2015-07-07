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
using Models;

public class DataManager {
    
    /// <summary>
    /// Get the current API authentication key.
    /// </summary>
    public static string APIKey {
        get {
            return currentConfig.authKey;
        }
    }
    
    /// <summary>
    /// Get the current remote server URL.
    /// </summary>
    public static string RemoteURL {
        get {
            return currentConfig.root;
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

    public static List<string> tacticNames;

    static string currentSceneContext;
    static JsonReaderSettings _readerSettings = new JsonReaderSettings();

    static GameConfig config;
    static GameEnvironment currentConfig;

    static GameData gameData;
    static GameDataTest gameDataTest;

    static ScenarioCard[] currentScenario;

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
        config = JsonReader.Deserialize<GameConfig>(data);
        
        // Set the current game config based on the environment
        #if UNITY_EDITOR
           currentConfig = config.local;
        #elif DEVELOPMENT_BUILD
           currentConfig = config.development;
        #else
           currentConfig = config.staging;
        #endif

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
            // _readerSettings.AddTypeConverter (new GameDataConverter());

            JsonReader reader = new JsonReader(data, _readerSettings);
            
            gameData = reader.Deserialize<GameData>();
            // gameDataTest = reader.Deserialize<GameDataTest>();

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
    /// <returns>An array of City.</returns>
    public static City[] GetAllCities()    {
        
        return gameData.cities;

    }

    /// <summary>
    /// Get a reference to a particular city in the game, given its name
    /// </summary>
    /// <param name="strCityName">Name of the city</param>
    /// <returns>The City for the given city</returns>
    public static City GetCityInfo(string strCityName) {
        
        foreach(City city in gameData.cities)
        {
            if(city.symbol == strCityName)
                return city;
        }

        return null;

    }

    /// <summary>
    /// Get all current routes available in game data.
    /// </summary>
    /// <returns>An array of Route.</returns>
    public static Route[] GetAllRoutes ()    {
        
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
    /// <returns>The Route for the given route</returns>
    public static Route GetRouteInfo(string strRouteName) {
        
        foreach(Route route in gameData.routes)
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
    public static NPC[] GetNPCsForCity(string strSelector=null) {
        
        if(strSelector == null) {
            try {
                return gameData.phase_one[currentSceneContext];
            } catch (System.Exception e) {
                throw new System.Exception ("No city with the symbol '" + currentSceneContext + "' could be found.");
            }
        } else {

            NPC[] npcRef = new NPC[] { Array.Find(gameData.phase_one[currentSceneContext], row => row.character == strSelector) };
            
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
    /// <returns>The Unlockable for the symbol matching the input</returns>
    public static Unlockable GetUnlockableBySymbol(string strSymbol) {
        
        Unlockable unlockRef = new Unlockable[] { 
            Array.Find(gameData.unlockables, unlockable => unlockable.symbol == strSymbol) }[0];

        if(unlockRef == null)
            throw new Exception("Unable to find Unlockable with symbol '" + strSymbol + "'! Uh oh.");
        
        return unlockRef;
    }

    /// <summary>
    /// Get the phase two scenario card specified by the index input.
    /// </summary>
    /// <param name="cardIndex">Index of the scenario card</param>
    /// <returns>The ScenarioCard for the symbol matching the input</returns>
    public static ScenarioCard GetScenarioCardByIndex(int cardIndex) {

        if(currentScenario == null)
        {
            currentScenario =  gameData.phase_two.GetScenario(currentSceneContext);
            Array.Sort(currentScenario);
        }

        ScenarioCard scenarioRef = currentScenario[cardIndex];
        
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
    /// <returns>The TacticCard for the symbol matching the input</returns>
    public static TacticCard GetTacticCardByName(string cardName) {

        TacticCard tacticRef = gameData.phase_two.tactics.FirstOrDefault(card => card.tactic_name == cardName);

        if(tacticRef == null)
            throw new Exception("Unable to find TacticCard with tactic name '" + cardName + "'! Damn.");
                
        return tacticRef;
    }

    /// <summary>
    /// Get a character's data given its symbol
    /// </summary>
    /// <param name="strCharSymbol">Symbol of the character</param>
    /// <returns>The Character for the symbol matching the input</returns>
    public static Character GetDataForCharacter(string strCharSymbol)    {
        
        int characterIndex = Array.FindIndex(gameData.characters, row => row.symbol == strCharSymbol);
        
        if(characterIndex == -1)
            throw new Exception("Unable to find Character with symbol '" + strCharSymbol + "'! Uh oh.");
     
        return gameData.characters[characterIndex];

    }

    /// <summary>
    /// Get an affect given its symbol
    /// </summary>
    /// <param name="strSymbol">Symbol of the affect</param>
    /// <returns>The Dictionary for the affect<returns>
    public static Dictionary<string, int> GetIndicatorBySymbol(string strSymbol)    {
        
        try { 

            return gameData.indicator_affects[strSymbol];

        } 
        catch(Exception e) {

            throw new Exception("Unable to find Affect with symbol '" + strSymbol + "'! Uh oh.");
     

        }

    }
}
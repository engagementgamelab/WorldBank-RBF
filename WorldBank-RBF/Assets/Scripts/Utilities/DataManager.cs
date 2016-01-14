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
using System.Reflection;
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
            // Reset scenario
            if(value.StartsWith("scenario"))
                currentScenario = gameData.phase_two.GetScenario(value);

            currentSceneContext = value;
        }
    }
    
    /// <summary>
    /// Get the current phase two year.
    /// </summary>
    public static int CurrentYear {
        get {
            return gameData.phase_two.Year;
        }
    }
    
    /// <summary>
    /// Get the current phase two config.
    /// </summary>
    public static PhaseTwoConfig PhaseTwoConfig {
        get {
            return gameData.phase_two.phase_two_config;
        }
    }
    
    /// <summary>
    /// Set to production mode.
    /// </summary>
    public static bool Production {
        set {
            isProduction = value;
        }
    }

    public static List<string> tacticNames;
    public static List<string> usedTooltips = new List<string>();
    public static readonly string DataNotLoaded = "<data not loaded>";

    public static bool tutorialEnabled;

    public static string currentPlanId;
    
    public static GameEnvironment currentConfig;
    
    static string currentSceneContext;
    
    static JsonReaderSettings _readerSettings = new JsonReaderSettings();

    static GameConfig config;

    static GameData gameData;
    static GameDataTest gameDataTest;

    static Scenario currentScenario;

    static bool isProduction;

    static Dictionary<string, string> localUIText = new Dictionary<string, string>() {
        {"copy_server_down_header", "Sorry!"},
        {"copy_server_down_body", "The game's server is currently unreachable. Your internet connection may be having some issues, or the server is offline for regular maintenance.\n\nPlease close the application and try again in a few minutes. Apologies for the inconvenience!"}
    };

    /// <summary>
    /// Set global game config data, such as API endpoints, given a valid input string
    /// </summary>
    /// <param name="data">Data to be used to set config; must conform to GameConfig model.</param>
    /// <param name="configTypeOverride">May be used to override config type in editor only (development, staging, production).</param>
    public static void SetGameConfig(string data, string configTypeOverride=null)
    {

        // Set config only if there is none set
        if(config != null) 
            return;

        // Set global config
        config = JsonReader.Deserialize<GameConfig>(data);
        
        // Set the current game config based on the environment
        #if UNITY_EDITOR
           currentConfig = config.local;

            // If override set, use that
            if(configTypeOverride != null) {
                if(configTypeOverride == "development")
                    currentConfig = config.development;
                else if(configTypeOverride == "staging")
                    currentConfig = config.staging;
                else if(configTypeOverride == "production")
                    currentConfig = config.production;
            }


        #elif DEVELOPMENT_BUILD
           currentConfig = config.development;
        #elif IS_PRODUCTION
           currentConfig = config.production;
        #else
           #if !UNITY_WEBGL
               currentConfig = config.staging;
           #else
               // Hack
               currentConfig = config.development;
           #endif
        #endif

        Debug.Log(">>>>>>>>>");
        Debug.Log("Using server at " + currentConfig.root);
        Debug.Log("<<<<<<<<<");

    }

    /// <summary>
    /// Set global game data, given a valid input string
    /// </summary>
    /// <param name="data">String to be used to set game data; must conform to GameData model.</param>
    public static void SetGameData(string data)
    {
        // Set global data only if there is none
//        if(gameData != null) 
//            return;

        try {

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

        // create/save to file in Assets/Resources/
        #if !UNITY_WEBPLAYER

            SaveDataToJson("data", data);

        #endif
    }

    /// <summary>
    /// Save a string to specified JSON file in /Assets/Resources/ or the persistent data path for the app
    /// </summary>
    /// <param name="fileName">The file's name.</param>
    /// <param name="data">String to be used to save in JSON file.</param>
    /// <param name="persistentPath">Use the application persistent resource path?</param>
    public static void SaveDataToJson(string fileName, string data, bool persistentPath=false) {

        string dataPath = (persistentPath ? Application.persistentDataPath : Application.dataPath) + "/Resources/";
        DirectoryInfo dirData = new DirectoryInfo(dataPath);
        dirData.Refresh();
        
        if(!dirData.Exists)
            dirData.Create();

        using (StreamWriter outfile = new StreamWriter(dataPath + fileName + ".json"))
        {
            outfile.Write(data);
        }

    }

    /// <summary>
    /// Get the UI Text associated with the given key.
    /// </summary>
    /// <returns>Copy associated with the key.</returns>
    public static string GetUIText (string key) {
        
        if (gameData == null) {
            try {
                return localUIText[key];
            } catch {
                return DataNotLoaded;
            }
        }

        string val;
        if (gameData.ui_text.TryGetValue (key, out val)) {
            return val;
        }
     
        return "";
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
            } catch {
                throw new System.Exception ("No city with the symbol '" + currentSceneContext + "' could be found.");
            }
        } else {

            NPC[] npcRef = new NPC[] { Array.Find(gameData.phase_one[currentSceneContext], row => row.character == strSelector) };
            
            if(npcRef[0] == null)
                throw new Exception("Unable to find NPC with symbol '" + strSelector + "' for this city (" + currentSceneContext + ")! Fiddlesticks.");
            
            return npcRef;
        }
        
    }

    public static NPC GetNpc (string symbol) {
        foreach (var npc in gameData.phase_one) {
            NPC found = npc.Value.ToList ().Find (e => e.character == symbol);
            if (found != null)
                return found;
        }
        return null;
    }

    public static int GetCityNPCCount (string citySymbol) {
        return gameData.phase_one[citySymbol].Length;
    }

    /// <summary>
    /// Get all characters available in game data.
    /// </summary>
    /// <returns>An array of Character.</returns>
    public static Character[] GetAllCharacters ()    {
        
        try {
            return gameData.characters;
        } catch {
            throw new Exception ("Could not load characters.");
        }

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
    /// Get all current unlockables available in game data.
    /// </summary>
    /// <returns>An array of Models.Unlockable.</returns>
    public static Models.Unlockable[] GetAllUnlockables () {
        
        try {
            return gameData.unlockables;
        } catch {
            throw new Exception ("Could not load unlockables");
        }

    }

    /// <summary>
    /// Get all unlockables with the supplied prefix available in game data.
    /// </summary>
    /// <param name="prefix">Unlockable prefix</param>
    /// <returns>An array of Models.Unlockable.</returns>
    public static Models.Unlockable[] GetUnlockablesWithPrefix (string prefix) {
        
        try {
            if (prefix == "") {
                // not a great way of handling this - tactics should have a prefix like route and dialogue unlockables
                return Array.FindAll (gameData.unlockables, x => !x.symbol.Contains ("_route_") && !x.symbol.Contains ("_dialogue_"));
            } else {
                return Array.FindAll (gameData.unlockables, x => x.symbol.StartsWith ("unlockable_" + prefix + "_"));
            }
        } catch {
            throw new Exception ("Could not load unlockables with prefix of '" + prefix + "'");
        }

    }

    /// <summary>
    /// Get the phase two scenario card specified by the index input.
    /// </summary>
    /// <param name="cardIndex">Index of the scenario card</param>
    /// <param name="scenarioTwist">The index of the scenario twist, if any (default is 0)</param>
    /// <returns>The ScenarioCard for the symbol matching the input</returns>
    public static ScenarioCard GetScenarioCardByIndex(int cardIndex, int scenarioTwist=0) {

        ScenarioCard[] scenarioCards = (scenarioTwist > 0) ? currentScenario.twists : currentScenario.problems; 

        // Get scenario cards only for the current phase two year
        scenarioCards = scenarioCards.Where(scenarioCard => scenarioCard.year == gameData.phase_two.Year && 
                                                            scenarioCard.twist == scenarioTwist
                                           ).ToArray();
		
		Array.Sort(scenarioCards);

        return scenarioCards[cardIndex];

    }

    /// <summary>
    /// Get the phase two scenario's config.
    /// </summary>
    /// <returns>The ScenarioConfig for the current scenario</returns>
    public static ScenarioConfig GetScenarioConfig() {
        
        return currentScenario.path_config;
        
    }

    /// <summary>
    /// Advance phase two current year.
    /// </summary>
    public static void AdvanceScenarioYear() {
        
        gameData.phase_two.Year++;
        
    }

    /// <summary>
    /// Get/set the phase two scenario's player decisions.
    /// </summary>
    /// <param name="decisionName">A decision name to add (optional)</param>
    /// <returns>Phase two's player decisions≥</returns>
    public static List<string> ScenarioDecisions(string decisionName=null) {

        if(decisionName != null)
            gameData.phase_two.AddDecision(decisionName);
        
        return gameData.phase_two.Decisions;
        
    }

    /// <summary>
    /// Get the length of the current scenario.
    /// </summary>
    /// <returns>Scenario length (int)</returns>
    /// <param name="scenarioTwist">Is the scenario in a twist? (default is false)</param>
    public static int ScenarioLength(int scenarioTwist=0) {
        
        ScenarioCard[] scenarioCards = (scenarioTwist > 0) ? currentScenario.twists : currentScenario.problems; 
        
        // Get scenario cards only for the current phase two year
        scenarioCards = scenarioCards.Where(scenarioCard => scenarioCard.year == gameData.phase_two.Year && 
                                                            scenarioCard.twist == scenarioTwist
                                           ).ToArray();

        return scenarioCards.Length;
    
    }

    /// <summary>
    /// Get the phase two tactic cards specified by the tactics' symbols, if there's any for the current year.
    /// </summary>
    /// <param name="cardSymbols">Card symbols</param>
    /// <returns>The TacticCards for the symbols matching the input</returns>
    public static List<TacticCard> GetTacticCardsForSymbols(string[] cardSymbols) {

        List<TacticCard> tacticRefs = gameData.phase_two.tactics.Where(card => cardSymbols.Contains(card.tactic_name) && 
                                                                                 card.year == gameData.phase_two.Year).ToList();

        if(tacticRefs == null)
            throw new Exception("Unable to find any TacticCard for tactic symbols '" + cardSymbols + "' for the current year! Damn.");
                
        return tacticRefs;
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
    /// Get the unlockables for a given NPC's dialogue
    /// </summary>
    /// <param name="npcRef">The NPC reference</param>
    /// <returns>The Character for the symbol matching the input</returns>
    public static string[][] GetUnlocksForCharacter(Models.NPC npcRef) {

        Dictionary<string, Models.Dialogue> diag = npcRef.dialogue;

        return diag.Select(x => x.Value.unlocks).OfType<string[]>().ToArray();

    }

    /// <summary>
    /// Get an affect given its symbol
    /// </summary>
    /// <param name="strSymbol">Symbol of the affect</param>
    /// <returns>The Dictionary for the affect<returns>
    public static Dictionary<string, int> GetIndicatorBySymbol(string strSymbol) {
        
        try { 

            return gameData.indicator_affects[strSymbol];

        } 
        catch {

            throw new Exception("Unable to find Affect with symbol '" + strSymbol + "'! Uh oh.");
     

        }

    }

    /// <summary>
    /// Find if the current indicators are above or below the starting indicator values.
    /// </summary>
    /// <param name="initialAffects">The initial affect values</param>
    /// <param name="currentAffects">The current affect values</param>
    /// <returns>Are the current indicators higher than the initial ones (bool)</returns>
    public static bool IsIndicatorDeltaGood(int[] initialAffects, int[] currentAffects) {

        return (initialAffects.Sum() - currentAffects.Sum()) > 0;

    }
    /// <summary>
    /// Get all tooltip keys.
    /// </summary>
    /// <returns>The tooltip keys currently defined.</returns>
    public static string[] GetTooltipKeys() {

        return gameData.tutorial.Keys.ToArray();

    }

    /// <summary>
    /// Get a tooltip given its key.
    /// </summary>
    /// <param name="strKey">The key of the tooltip.</param>
    /// <returns>The tooltip for this key</returns>
    public static Tooltip GetTooltipByKey(string strKey) {
        
        try { 

            return gameData.tutorial[strKey];

        } 
        catch {

            throw new Exception("Unable to find tutorial tooltip with key '" + strKey + "'! Poppycock!");

        }

    }

    /// <summary>
    /// Get the UI text for the given key.
    /// </summary>
    /// <param name="strKey">Key for the text</param>
    /// <returns>The obtained UI text.<returns>
    public static string GetUITextByKey(string strKey) {
        
        try { 

            return gameData.ui_text[strKey];

        } 
        catch {

            throw new Exception("Unable to find UI text with key '" + strKey + "'! Hogwash!");
     
        }

    }

    public static Dictionary<string, object> GetLocalPlanById(string planId) {

        PlanRecord planRetrieved;

        // If zero plan (user's), load from prefs
        if(planId == "0")
            planRetrieved = JsonReader.Deserialize<Models.PlanRecord>(PlayerPrefs.GetString("current plan"));
        else
        {
            // Open stream to plans JSON config file
            TextAsset plansJson = (TextAsset)Resources.Load("plans", typeof(TextAsset) );
            StringReader strPlansData = new StringReader(plansJson.text);

            PlanRecord[] plans = JsonReader.Deserialize<Models.PlanRecord[]>(strPlansData.ReadToEnd());

            planRetrieved = plans.FirstOrDefault(plan => plan._id == planId);
        }

        return planRetrieved.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(planRetrieved, null));

    }

    public static Grade[] GetGrading() {

        return gameData.grading;

    }

    public static Grade GetGradeForPlan(PlanRecord plan) {

        return gameData.grading.FirstOrDefault(grade =>
                                  (plan.score <= int.Parse(grade.score.Split('-')[0]) && 
                                    plan.score >= int.Parse(grade.score.Split('-')[1])
                              ));

    }

}
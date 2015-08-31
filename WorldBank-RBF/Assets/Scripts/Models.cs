/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 Models.cs
 Game data models.

 Created by Johnny Richardson on 4/21/15.
==============
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JsonFx.Json;

/// <summary>
/// Game data models.
/// </summary>
namespace Models {

    /// <summary>
    /// Stores game config schema.
    /// </summary>
    public class GameConfig {

        public GameEnvironment local { get; set; }
        public GameEnvironment development { get; set; }
        public GameEnvironment staging { get; set; }

        public string parseAppId { get; set; }
        public string parseKey { get; set; }

    }

    /// <summary>
    /// Stores game config environment schema.
    /// </summary>
    public class GameEnvironment {

        public string root { get; set; }
        public string authKey { get; set; }

    }

	/// <summary>
	/// Stores all game data.
	/// </summary>
    public class GameData {

        public Character[] characters { get; set; }
        public City[] cities { get; set; }
        public Dictionary<string, Dictionary<string, int>> indicator_affects { get; set; }
        public Route[] routes { get; set; }
        public Unlockable[] unlockables { get; set; }
        public Dictionary<string, NPC[]> phase_one { get; set; }
        public PhaseTwo phase_two { get; set; }

    }

    // Test model
    public class GameDataTest {

        public Character[] characters { get; set; }
        public City[] cities { get; set; }
        // public Dictionary<string, Dictionary<string, int>> indicator_affects { get; set; }
        // public Route[] routes { get; set; }
        // public Unlockable[] unlockables { get; set; }
        // public Dictionary<string, NPC[]> phase_one { get; set; }
        // public PhaseTwo phase_two { get; set; }

    }

    public class User {

        public string _id { get; set; }
        // public DateTime last_accessed { get; set; }
        // public string location { get; set; }
        public string username { get; set; }
        public string current_scenario { get; set; }
        public Plan plan { get; set; }

    }

    public class Character {

        public string symbol { get; set; }
        public string display_name { get; set; }
        public string[] description { get; set; }

    }

    public class City {

        public string symbol { get; set; }
        public string display_name { get; set; }
        public string description { get; set; }
        public bool enabled { get; set; }
        public int[] npc_interactions { get; set; }

    }

    public class Unlockable : IInventoryModel {

        public string symbol { get; set; }
        public string title { get; set; }
        public string[] description { get; set; }
        public string type { get; set; }
        public string context { get; set; }
        public int priority { get; set; }
        public bool unlocked { get; set; }

    }

    public class Route : Unlockable {

        public string city1 { get; set; }
        public string city2 { get; set; }
        public int cost { get; set; }

    }

    // This a little ugly, but helps me keep phase two data strongly typed (since these siblings are multiple types)
    public class PhaseTwo { 

        public PhaseTwoConfig phase_two_config { get; set; }

        public Scenario scenario_1 { get; set; }
        public Scenario scenario_2 { get; set; }
        public Scenario scenario_3 { get; set; }
        public Scenario scenario_4 { get; set; }
        
        public TacticCard[] tactics { get; set; }

        List<string> selectedDecisions = new List<string>();
        int currentYear = 1;

        // This is slow but we'll only call it when obtaining a scenario card
        public Scenario GetScenario(string scenarioName)
        {
            Scenario theScenario = (Scenario)this.GetType().GetProperty(scenarioName).GetValue(this, null);

            return theScenario;
        }

        public List<string> Decisions {
            get {
                return selectedDecisions;
            }
        }
        public void AddDecision(string value) {
            selectedDecisions.Add(value);
        }

        public int Year {
            get {
                return currentYear;
            }
            set {
                currentYear = value;
            }
        }

    }

    public class PhaseTwoConfig {

        public float[] tactic_card_intervals { get; set; }
        public float phase_length_seconds { get; set; }
        public string[] tactics_not_selected { get; set; }
        
    }

    public class Scenario {

        public ScenarioCard[] problems { get; set; }
        public ScenarioCard[] twists { get; set; }
        public ScenarioConfig path_config { get; set; }

    }

    public class ScenarioCard : IComparable {

        public string symbol { get; set; }
        public string name { get; set; }
        
        public int priority { get; set; }
        public int year { get; set; }
        public int twist { get; set; }

        public string initiating_npc { get; set; }
        public string initiating_dialogue { get; set; }
        
        public string[] starting_options { get; set; }
        public string[] starting_options_affects { get; set; }
        public string[] final_options { get; set; }
        public string[] final_options_affects { get; set; }
        
        public Dictionary<string, Advisor> characters { get; set; }

        // Sort ScenarioCard by priority
        public int CompareTo(object obj)
        {
            if (obj is ScenarioCard) {
              return this.priority.CompareTo((obj as ScenarioCard).priority);
            }
        
            throw new ArgumentException("Object is not a ScenarioCard");
        }

    }

    public class TacticCard : ScenarioCard {

        public string symbol { get; set; }
        public string name { get; set; }
        public string tactic_name { get; set; }
        public int year { get; set; }
        
        public string initiating_dialogue { get; set; }
        public string investigate_dialogue { get; set; }
        public string investigate_further_dialogue { get; set; }

        public float[] investigate_cooldown { get; set; }
        public float[] investigate_further_cooldown { get; set; }
        
        public string[] new_options { get; set; }
        public string[] new_options_affects { get; set; }
        public string[] further_options { get; set; }
        public string[] further_options_affects { get; set; }
        
        public Dictionary<string, string> feedback_dialogue { get; set; }

    }

    public class ScenarioConfig {

        public string[] prompt_year_1 { get; set; }
        public string[] prompt_year_2 { get; set; }
        public Dictionary<string, string>[] choices { get; set; }

    }

    // TODO: move properties to ParallaxNpc and remove this model
    public class NPC {

        public string symbol { get; set; }
        public string character { get; set; }
		public Dictionary<string, Dialogue> dialogue { get; set; }

        public Dialogue GetDialogue(string dialogueKey)
        {
            try {
                Dictionary<string, Dialogue> refDict = new Dictionary<string, Dialogue>(dialogue, StringComparer.OrdinalIgnoreCase);
                return refDict[dialogueKey];
            }
            catch(System.Exception e) {
                throw new System.Exception("Dialogue key '" + dialogueKey + "' not found for dialogue file '" + symbol + "'");
            }
        }

    }

    public class Advisor {

        public string dialogue { get; set; }
        public string[] narrows { get; set; }
        public string[] unlocks { get; set; }
        public string[] unlocks_affects { get; set; }
        public Dictionary<string, object> feedback { get; set; }
        public bool initial { get; set; }
        
        public bool narrowsNpcs { get { return narrows != null && narrows.Length > 0; } }
        public bool hasDialogue { get { return dialogue != null; } }

    }

    public class Dialogue {

        public string display_name { get; set; }
        public string[] text { get; set; }
        public string[] unlocks { get; set; }
        public string[] unlocks_context { get; set; }

    }
    
    [JsonOptIn]
    public class Plan {

        [JsonMember]
        public string _id { get; set; }
        [JsonMember]
        public string name { get; set; }
        [JsonMember]
        public int score { get; set; }
        [JsonMember]
        public string[] tactics { get; set; }
        // [JsonMember]
        // public string[] tactics { get; set; }
    }

    public class GameDataConverter : JsonConverter {

        public override bool CanConvert (Type type) {

            bool convertible = type == typeof(string);

            return convertible;
        }
        
        public override object ReadJson (Type objectType, Dictionary<string,object> value) {

            return new GameData();

        }
        
        public override Dictionary<string,object> WriteJson (Type type, object value) {
            
            Dictionary<string,object> dict = new Dictionary<string, object>();

            return dict;

        }

    }
    
    public class Scene {
        public string cityName { get; set; }
        public int LayerCount { get; set; }
        public List<object> layers { get; set; }
        public float CameraStartPosition { get; set; }
    }

    public class ParallaxLayer {
        public float LocalSeparation { get; set; }
        public List<object> images { get; set; }
        public List<object> npcs { get; set; }
        public List<object> zoomTriggers { get; set; }
    }

    public class ParallaxImage {
        public bool child { get; set; }
        public int Index { get; set; }
        public string TexturePath { get; set; }
        public float LocalPositionX { get; set; }
    }

    public class ParallaxElement : ParallaxImage {
        public float XPosition { get; set; }
        public float YPosition { get; set; }
        public float ColliderXPosition { get; set; }
        public float ColliderYPosition { get; set; }
        public float ColliderWidth { get; set; }
        public float ColliderHeight { get; set; }
    }

    public class ParallaxNpc : ParallaxElement {
        public string symbol { get; set; }
        // public string voice { get; set; } // TODO: uncomment this so that npc voices can be specified - - the game breaks if the field is missing in the model, though
        public bool facingLeft { get; set; }
        public string Texture2Path { get; set; }
    }

    public class ParallaxZoomTrigger : ParallaxElement {
        public float zoomTarget { get; set; }
    }

    public class AmbienceZones {
        public string cityContext { get; set; }
        public List<object> zones { get; set; }
    }

    public class AmbienceZone {
        public bool child { get; set; }
        public int Index { get; set; }
        public string context { get; set; }
        public float offset { get; set; }
        public float width { get; set; }
        public float fadeLength { get; set; }
    }
}

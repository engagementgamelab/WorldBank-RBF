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

/// <summary>
/// Game data models.
/// </summary>
public class Models {

    /// <summary>
    /// Stores game config schema.
    /// </summary>
    public class GameConfig {

        public string serverRoot { get; set; }
        public string authKey { get; set; }

    }

	/// <summary>
	/// Stores all game data.
	/// </summary>
    public class GameData {

        public Character[] characters { get; set; }
        public City[] cities { get; set; }
        public Unlockable[] unlockables { get; set; }
        public Dictionary<string, NPC[]> phase_one { get; set; }
        public Dictionary<string, Dictionary<string, Scenario>> phase_two { get; set; }

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
        public bool enabled { get; set; }
        public bool unlocked { get; set; }
        public int cost { get; set; }

    }

    public class Unlockable {

        public string symbol { get; set; }
        public string title { get; set; }
        public string[] description { get; set; }
        public string type { get; set; }
        public int priority { get; set; }

    }


    public class Scenario {

        public string symbol { get; set; }
        public string name { get; set; }
        public string initiating_npc { get; set; }
        public string initiating_dialogue { get; set; }
        public string[] starting_options { get; set; }
        public string[] final_options { get; set; }
        public Dictionary<string, Advisor> characters { get; set; }

    }

   
    public class NPC {

        public string symbol { get; set; }
        public string character { get; set; }
		public Dictionary<string, Dictionary<string, string>> dialogue { get; set; }

    }

    public class Advisor {

        public string dialogue { get; set; }
        public string[] unlocks { get; set; }
        public Dictionary<string, object> feedback { get; set; }

    }

    public class Dialogue {

        public Dictionary<string, string> dialogue { get; set; }

    }

    /*
    public class UnlocksConverter : JsonConverter {
        public override bool CanConvert (Type type) {
            return type == typeof(Bounds);
        }
        
        public override string[] ReadJson (Type objectType, object value) {

            string[] convertedVal;

            if(value.GetType().IsArray)
            {
                convertedVal = ((IEnumerable)value).Cast<object>()
                                                 .Select(x => x.ToString())
                                                 .ToArray();
            }
            else
                unlocksVal = new string[] { value.ToString() };

            return convertedVal;
        }
        
        public override string[] WriteJson (Type type, string[] value) {
            return value;
        }
    }

    */


    public class Scene {
        public string cityName { get; set; }
        public int LayerCount { get; set; }
        public List<object> layers { get; set; }
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
    }

    public class ParallaxZoomTrigger : ParallaxElement {
        public float zoomTarget { get; set; }
    }

}

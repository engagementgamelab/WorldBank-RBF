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

    public class Scene {
        // TODO: Move scene model into here
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

    public class Test {
        public string hello { get; set; }
    }

    public class Test2 {
        public float happiness { get; set; }
        public float sadness { get; set; }
        // public Texture2D texture { get; set; }
        public List<object> blahs { get; set; }
    }

    public class Test3 {
        public int goop { get; set; }
        public List<string> texts { get; set; }
        // public string[] blahs { get; set; }
        public List<object> test2s { get; set; }
        public object[] test3s { get; set; }
        public object ex2 { get; set; }
    }
}

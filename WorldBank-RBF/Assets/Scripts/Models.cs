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

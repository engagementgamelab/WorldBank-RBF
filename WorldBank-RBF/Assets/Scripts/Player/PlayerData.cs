/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 PlayerData.cs
 Store and manipulate the data specific to a player.

 Created by Johnny Richardson on 4/30/15.
==============
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Stores and manipulates data specific to this instance of the game.
/// </summary>
public class PlayerData {

	static Inventory inventory = null;

	/// <summary>
	/// Gets the Inventory, which contains all the game data. In general it is better 
	/// to reference ItemGroups directly using the other properties in this class.
	/// </summary>
	public static Inventory Inventory {
		get { 
			if (inventory == null) {
				inventory = new Inventory ();
				inventory.Add (new TacticGroup ());
				inventory.Add (new TacticPriorityGroup ());
				inventory.Add (new DialogueGroup ());
				inventory.Add (new DayGroup (9)); // TODO: the initial # of days should be specified in a config file
				inventory.Add (new InteractionGroup ());
				inventory.Add (new RouteGroup ());
				inventory.Add (new CityGroup ());
			}
			return inventory; 
		}
	}

	/// <summary>
	/// Gets the TacticGroup, which contains all the unlockable plan tactics.
	/// </summary>
	public static TacticGroup TacticGroup {
		get { return (TacticGroup)Inventory["tactics"]; }
	}

	/// <summary>
	/// Gets the TacticPriorityGroup, which contains the TacticItems that make up the
	/// plan that will be submitted for phase two.
	/// </summary>
	public static TacticPriorityGroup TacticPriorityGroup {
		get { return (TacticPriorityGroup)Inventory["priorities"]; }
	}

	/// <summary>
	/// Gets the DialogueGroup, which contains all the unlockable dialogue trees.
	/// </summary>
	public static DialogueGroup DialogueGroup {
		get { return (DialogueGroup)Inventory["dialogues"]; }
	}

	/// <summary>
	/// Gets the DayGroup, which stores the number of days that the player has to
	/// travel between cities on the map.
	/// </summary>
	public static DayGroup DayGroup {
		get { return (DayGroup)Inventory["days"]; }
	}

	/// <summary>
	/// Gets the InteractionGroup, which stores the number of interactions that
	/// the player can use to engage with NPCs in a city.
	/// </summary>
	public static InteractionGroup InteractionGroup {
		get { return (InteractionGroup)Inventory["interactions"]; }
	}

	/// <summary>
	/// Gets the RouteGroup, which contains all the routes between cities.
	/// </summary>
	public static RouteGroup RouteGroup {
		get { return (RouteGroup)Inventory["routes"]; }
	}

	/// <summary>
	/// Gets the CityGroup, which contains all the cities in the game.
	/// </summary>
	public static CityGroup CityGroup {
		get { return (CityGroup)Inventory["cities"]; }
	}

	/// <summary>
	/// Unlocks the specified implementation for player.
	/// </summary>
	/// <param name="strSymbol">Symbol of the unlockable</param>
	public static void UnlockItem (string strSymbol) {

		Models.Unlockable unlockRef = DataManager.GetUnlockableBySymbol(strSymbol);
		
		if (strSymbol.StartsWith ("unlockable_route_")) {
			RouteGroup.Unlock (strSymbol);
		} else if (strSymbol.StartsWith ("unlockable_dialogue_")) {
			DialogueGroup.Unlock (strSymbol);
		} else {
			TacticGroup.Unlock (strSymbol);
		}
	}
}

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

public class PlayerData {

	static Inventory inventory = null;

	public static Inventory Inventory {
		get { 
			if (inventory == null) {
				inventory = new Inventory ();
				inventory.Add (new TacticGroup ());
				inventory.Add (new TacticPriorityGroup ());
				inventory.Add (new DialogueGroup ());
				inventory.Add (new DayGroup (15));
				inventory.Add (new InteractionGroup ());
				inventory.Add (new RouteGroup ());
				inventory.Add (new CityGroup ());
			}
			return inventory; 
		}
	}

	public static TacticGroup TacticGroup {
		get { return (TacticGroup)Inventory["tactics"]; }
	}

	public static TacticPriorityGroup TacticPriorityGroup {
		get { return (TacticPriorityGroup)Inventory["priorities"]; }
	}

	public static DialogueGroup DialogueGroup {
		get { return (DialogueGroup)Inventory["dialogues"]; }
	}

	public static DayGroup DayGroup {
		get { return (DayGroup)Inventory["days"]; }
	}

	public static InteractionGroup InteractionGroup {
		get { return (InteractionGroup)Inventory["interactions"]; }
	}

	public static RouteGroup RouteGroup {
		get { return (RouteGroup)Inventory["routes"]; }
	}

	public static CityGroup CityGroup {
		get { return (CityGroup)Inventory["cities"]; }
	}

	/// <summary>
	/// Unlocks the specified implementation for player and increments its unlock count
	/// </summary>
	/// <param name="strSymbol">Symbol of the unlockable</param>
	public static void UnlockImplementation (string strSymbol) {

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

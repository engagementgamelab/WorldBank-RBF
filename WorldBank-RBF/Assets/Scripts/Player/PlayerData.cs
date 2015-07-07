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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerData : MonoBehaviour {

	private static List<Models.Unlockable> playerImplementations = new List<Models.Unlockable>();
	private static Dictionary<string, int> playerUnlockCounts = new Dictionary<string, int>();

	static Inventory inventory 						= new Inventory ();
	static PlanTacticGroup planTacticGroup 			= new PlanTacticGroup ();
	static TacticPriorityGroup tacticPriorityGroup 	= new TacticPriorityGroup ();
	static DialogueGroup dialogueGroup 				= new DialogueGroup ();
	static DayGroup dayGroup 						= new DayGroup (15);
	static InteractionGroup interactionGroup 		= new InteractionGroup ();
	static RouteGroup routeGroup 					= new RouteGroup ();
	static CityGroup cityGroup 						= new CityGroup ();

	public static Inventory Inventory {
		get { return inventory; }
	}

	public static PlanTacticGroup PlanTacticGroup {
		get { return planTacticGroup; }
	}

	public static TacticPriorityGroup TacticPriorityGroup {
		get { return tacticPriorityGroup; }
	}

	public static DialogueGroup DialogueGroup {
		get { return dialogueGroup; }
	}

	public static DayGroup DayGroup {
		get { return dayGroup; }
	}

	public static InteractionGroup InteractionGroup {
		get { return interactionGroup; }
	}

	public static RouteGroup RouteGroup {
		get { return routeGroup; }
	}

	public static CityGroup CityGroup {
		get { return cityGroup; }
	}

	void Awake () {
		inventory.Add (planTacticGroup);
		inventory.Add (tacticPriorityGroup);
		inventory.Add (dialogueGroup);
		inventory.Add (dayGroup);
		inventory.Add (interactionGroup);
		inventory.Add (routeGroup);
		inventory.Add (cityGroup);
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
			PlanTacticGroup.Unlock (strSymbol);
		}

	}

	public static void LockRoute (string symbol) {
		RouteGroup.Lock (symbol);
	}

	public static void SetTactics (PlanTacticGroup tacticGroup) {
		planTacticGroup = tacticGroup;
	}

	public static void SetPriorities (TacticPriorityGroup priorityGroup) {
		tacticPriorityGroup = priorityGroup;
	}
}

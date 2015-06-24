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

	private static Inventory inventory;
	private static Inventory Inventory {
		get {
			if (inventory == null) {
				inventory = new Inventory ();
				inventory.Add (PlanTacticGroup);
				inventory.Add (TacticPriorityGroup);
				inventory.Add (RouteGroup);
				inventory.Add (CityGroup);
			}
			return inventory;
		}
	}

	private static PlanTacticGroup planTacticGroup;
	public static PlanTacticGroup PlanTacticGroup {
		get {
			if (planTacticGroup == null) {
				planTacticGroup = new PlanTacticGroup ();
			}
			return planTacticGroup;
		}
	}

	private static TacticPriorityGroup tacticPriorityGroup;
	public static TacticPriorityGroup TacticPriorityGroup {
		get {
			if (tacticPriorityGroup == null) {
				tacticPriorityGroup = new TacticPriorityGroup ();
			}
			return tacticPriorityGroup;
		}
	}

	private static RouteGroup routeGroup;
	public static RouteGroup RouteGroup {
		get {
			if (routeGroup == null) {
				routeGroup = new RouteGroup ();
				Models.Route[] routes = DataManager.GetAllRoutes ();
				foreach (Models.Route route in routes) {
					routeGroup.Add (new RouteItem (route));
				}
			}
			return routeGroup;
		}
	}

	private static CityGroup cityGroup;
	public static CityGroup CityGroup {
		get {
			if (cityGroup == null) {
				cityGroup = new CityGroup ();
			}
			return cityGroup;
		}
	}

	private static List<Models.Unlockable> playerImplementations = new List<Models.Unlockable>();
	private static Dictionary<string, int> playerUnlockCounts = new Dictionary<string, int>();


	/// <summary>
	/// Unlocks the specified implementation for player and increments its unlock count
	/// </summary>
	/// <param name="strSymbol">Symbol of the unlockable</param>
	public static void UnlockImplementation (string strSymbol) {

		Models.Unlockable unlockRef = DataManager.GetUnlockableBySymbol(strSymbol);
		if (strSymbol.Contains ("unlockable_route_")) {
			Models.Route route = RouteGroup.Unlock (strSymbol.Substring (17));
			if (route != null) {
				CityGroup.AddUnique (route.city1);
				CityGroup.AddUnique (route.city2);
			}
		} else {
			PlanTacticGroup.Add (new PlanTacticItem (unlockRef));
		}

	}

	public static void SetTactics (PlanTacticGroup tacticGroup) {
		planTacticGroup = tacticGroup;
	}

	public static void SetPriorities (TacticPriorityGroup priorityGroup) {
		tacticPriorityGroup = priorityGroup;
	}
}

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

	private static List<Models.Unlockable> playerImplementations = new List<Models.Unlockable>();
	private static Dictionary<string, int> playerUnlockCounts = new Dictionary<string, int>();

	void Awake () {
		PlayerData.PopulateTestTactics ();
	}

	/// <summary>
	/// Unlocks the specified implementation for player and increments its unlock count
	/// </summary>
	/// <param name="strSymbol">Symbol of the unlockable</param>
	public static void UnlockImplementation (string strSymbol) {

		Models.Unlockable unlockRef = DataManager.GetUnlockableBySymbol(strSymbol);
		PlanTacticGroup.Add (new PlanTacticItem (unlockRef));
		PlayerManager.Instance.SaveData (PlanTacticGroup.GetUniqueTacticSymbols ());
		// TODO: I think ^^this^^ replaces everything below (haven't checked w/ PlayerManager yet)

		/*Models.Unlockable unlockRef = DataManager.GetUnlockableBySymbol(strSymbol);

		// Add this unlockable to the player's unlocks inventory
		playerImplementations.Add(unlockRef);

		// If player already unlocked this one, increment the unlock count
		// Otherwise, set count to 1
		if(playerUnlockCounts.ContainsKey(strSymbol))
			playerUnlockCounts[strSymbol]++;
		else
			playerUnlockCounts.Add(strSymbol, 1);

		PlayerManager.Instance.SaveData(playerUnlockCounts.Keys.ToArray());*/

	}

	public static void SetTactics (PlanTacticGroup tacticGroup) {
		planTacticGroup = tacticGroup;
	}

	public static void SetPriorities (TacticPriorityGroup priorityGroup) {
		tacticPriorityGroup = priorityGroup;
	}

	// TODO: just for testing -- don't keep this
	static void PopulateTestTactics () {
		PlanTacticGroup.Add (new PlanTacticItem (null, 1));
		PlanTacticGroup.Add (new PlanTacticItem (null, 2));
		PlanTacticGroup.Add (new PlanTacticItem (null, 3));
	}
}

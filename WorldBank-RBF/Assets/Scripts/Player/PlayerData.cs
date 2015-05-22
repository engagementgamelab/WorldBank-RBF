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

	/// <summary>
	/// Unlocks the specified implementation for player and increments its unlock count
	/// </summary>
	/// <param name="strSymbol">Symbol of the unlockable</param>
	public static void UnlockImplementation(string strSymbol) {

		Models.Unlockable unlockRef = DataManager.GetUnlockableBySymbol(strSymbol);

		// Add this unlockable to the player's unlocks inventory
		playerImplementations.Add(unlockRef);

		// If player already unlocked this one, increment the unlock count
		// Otherwise, set count to 1
		if(playerUnlockCounts.ContainsKey(strSymbol))
			playerUnlockCounts[strSymbol]++;
		else
			playerUnlockCounts.Add(strSymbol, 1);

		// PlayerManager.Instance.SaveData(playerUnlockCounts.Keys.ToArray());

	}

}

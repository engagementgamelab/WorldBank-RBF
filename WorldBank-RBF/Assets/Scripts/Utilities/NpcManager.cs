using UnityEngine;
using System;
using System.Collections;

public class NpcManager {
	
	static Models.NPC[] npcs;

	public static void InitNpcs () {
		npcs = DataManager.GetNPCsForCity();
	}	

	public static Models.NPC GetNpc (string symbol) {

		return new Models.NPC[] { Array.Find(npcs, row => row.character == symbol) }[0];
		
	}
}

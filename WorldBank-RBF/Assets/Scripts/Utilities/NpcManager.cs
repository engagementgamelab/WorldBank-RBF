using UnityEngine;
using System;
using System.Collections;

public class NpcManager {
	
	static Models.NPC[] npcs;

	public static void InitNpcs () {
		npcs = DataManager.GetNPCsForCity();
	}	

	public static Models.NPC GetNpc (string symbol) {

		Models.NPC npc = new Models.NPC[] { Array.Find(npcs, row => row.character == symbol) }[0];
		
		if (npc == null)
			throw new System.Exception ("An NPC with the symbol '" + symbol + "' could not be found");

		return npc;
		
	}
}

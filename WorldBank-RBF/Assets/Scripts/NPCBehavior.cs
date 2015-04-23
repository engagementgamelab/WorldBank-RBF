/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 NPCBehavior.cs
 Unity NPC behavior handler.

 Created by Johnny Richardson on 4/20/15.
==============
*/

using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Timers;
using Extensions;

public class NPCBehavior : MB {

 	public string npcSymbol;

	private Models.NPC npcRef;

 	void Start() {

 		if(npcSymbol == null || npcSymbol.Length == 0)
 			 throw new Exception("NPC symbol not set!");

 		Initialize();

 	}

 	public void Initialize() {

 		// Get reference for this NPC
 		npcRef = DataManager.GetNPCsForCity(npcSymbol)[0];

 	}

 	// On Touch/Click NPC
	void OnMouseDown() {

		// Opens initial dialog for this NPC
		DialogManager.instance.OpenCharacterDialog(npcRef, "Initial");
	}
}
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
	Models.NPC npcRef;
	
	[SerializeField, HideInInspector] bool facingLeft = true;
	public bool FacingLeft {
		get { return facingLeft; }
		set { facingLeft = value; }
	}

 	void Start() {
 		Initialize();
 	}

 	public void Initialize() {

 		if(npcSymbol == null || npcSymbol.Length == 0)
 			 throw new Exception("NPC symbol not set!");

 		// Get reference for this NPC
 		npcRef = DataManager.GetNPCsForCity(npcSymbol)[0];
 	}

	public void OpenDialog () {
		DialogManager.instance.OpenCharacterDialog(npcRef, "Initial", this);
	}
}
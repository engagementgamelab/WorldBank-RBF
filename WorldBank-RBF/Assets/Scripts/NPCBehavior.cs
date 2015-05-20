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

	// Reference ParallaxNpc instead
	/*LayerImage npc = null;
	LayerImage NPC {
		get {
			if (npc == null) {
				npc = Parent.GetScript<LayerImage> ();
			}
			return npc;
		}
	}*/

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
		DialogFocus();
		DialogManager.instance.OpenSpeechDialog(npcRef, "Initial", this);
	}

	public void DialogFocus () {
<<<<<<< HEAD
		NPCFocusBehavior.Instance.SetFocus(NPC, FocusLevel.Dialog);
=======
		// TODO: Make this work with ParallaxNpc instead
		// NPCFocusBehavior.Instance.SetFocus (NPC, FocusLevel.Dialog);
>>>>>>> parallax-layers
	}

	public void OnClick () {
		FocusLevel level = NPCFocusBehavior.Instance.FocusLevel;
		if (level == FocusLevel.Default) {
			// NPCFocusBehavior.Instance.SetFocus (NPC, FocusLevel.Preview);
			DialogManager.instance.OpenIntroDialog(npcRef, this);
		} else if (level == FocusLevel.Dialog) {
			DialogManager.instance.CloseCharacterDialog ();
			CloseDialog ();
		}
	}

	public void CloseDialog (bool zoomOut=false) {
		// TODO: Make this work with ParallaxNpc instead
		// NPCFocusBehavior.Instance.SetFocus (NPC, FocusLevel.Default, zoomOut);
	}
}
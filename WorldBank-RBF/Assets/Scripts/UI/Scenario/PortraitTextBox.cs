/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 PortraitTextBox.cs
 Class for UI that contains text and image portrait for NPC.

 Created by Johnny Richardson on 07/24/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PortraitTextBox : GenericButton {

	public Image portrait;
	public bool phaseOne = false;
	public Transform npcName;

	/// <summary>
    /// Set the NPC Name on the button
    /// </summary>
	public string NPCName {

		set {

			if (npcName == null)
				npcName = GetComponent<Transform>().FindChild("NPCName");

			// if(value.Length > 25)
			// 	value = value.Substring(0, 22) + "...";

			npcName.GetComponent<Text>().text = value;

		}

	}

	/// <summary>
    /// Set the NPC symbol, which will load the corresponding portrait
    /// </summary>
	public virtual string NPCSymbol {

		set {
			
			// Obtain portrait image and load corresponding sprite
			if (phaseOne) {
				Sprite sprite = Resources.Load<Sprite>("Portraits/PhaseOne/" + value);
				if (sprite == null || value == "") 
					sprite = Resources.Load<Sprite>("Portraits/PhaseOne/capitol_city_traveler_malcom");
				portrait.sprite = sprite;
			} else {
				portrait.sprite = Resources.Load<Sprite>("Portraits/PhaseTwo/" + value);
			}
		}

	}
}

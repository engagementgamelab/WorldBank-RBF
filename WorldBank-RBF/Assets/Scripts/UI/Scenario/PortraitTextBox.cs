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

	/// <summary>
    /// Set the NPC Name on the button
    /// </summary>
	public string NPCName {

		set {

			Transform npcName = GetComponent<Transform>().FindChild("NPCName");

			if(value.Length > 20)
				value = value.Substring(0, 17) + "...";

			npcName.GetComponent<Text>().text = value;

		}

	}

	/// <summary>
    /// Set the NPC symbol, which will load the corresponding portrait
    /// </summary>
	public virtual string NPCSymbol {

		set {
			
			// Obtain portrait image and load corresponding sprite
			portrait.sprite = Resources.Load<Sprite>("Portraits/PhaseTwo/" + value);

		}

	}
}

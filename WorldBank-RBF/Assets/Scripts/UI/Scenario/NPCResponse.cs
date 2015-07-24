/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 NPCResponse.cs
 Class for scenario NPC reponse box.

 Created by Johnny Richardson on 07/24/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;

public class NPCResponse : PortraitTextBox {

	public Text responseTextBox;

	bool initial = false;
	bool leftSide = false;
	bool rightSide = false;

	public string Content {
		get { return responseTextBox.text; }
		set {
			responseTextBox.text = value;
		}
	}

	public bool LeftSide { 
		set {
			leftSide = true;
			rightSide = false;
		}
	}
	
	public bool RightSide { 
		set {
			leftSide = false;
			rightSide = true;
		}
	}

	public override string NPCSymbol {

		set {

			GameObject portrait = null;

			if(rightSide) {
				portrait = GetComponent<Transform>().FindChild("RightPortrait").gameObject;
				portrait.SetActive(true);
			}
			else if(leftSide) {
				portrait = GetComponent<Transform>().FindChild("LeftPortrait").gameObject;
				portrait.SetActive(true);
			}

			GetComponent<Transform>().FindChild("LeftArrow").gameObject.SetActive(leftSide);
			GetComponent<Transform>().FindChild("RightArrow").gameObject.SetActive(rightSide);

			// TODO: Set image using symbol as name

		}

	}
}
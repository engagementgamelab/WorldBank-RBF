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
	public Image leftPortrait;
	public Image rightPortrait;

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
			Image portrait = null;

			if(rightSide)
				portrait = rightPortrait;
			else if(leftSide)
				portrait = leftPortrait;

			if(portrait == null)
				return;

			portrait.gameObject.SetActive(true);

			// Obtain portrait image and load corresponding sprite
			portrait.sprite = Resources.Load<Sprite>("Portraits/PhaseTwo/" + value);

			GetComponent<Transform>().FindChild("LeftArrow").gameObject.SetActive(leftSide);
			GetComponent<Transform>().FindChild("RightArrow").gameObject.SetActive(rightSide);

		}

	}
}
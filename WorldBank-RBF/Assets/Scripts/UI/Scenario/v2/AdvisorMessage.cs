using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AdvisorMessage : PortraitTextBox {

	public Transform initialTextContainer;
	public Transform textContainer;

	public Image leftPortrait;
	public Image rightPortrait;

	bool initial = false;
	bool leftSide = true;
	bool rightSide = false;

	Text responseText;

	public string Content {

		get { return responseText.text; }
		set { 

			if(!initial) {
				textContainer.gameObject.SetActive(true);
				initialTextContainer.gameObject.SetActive(false);

				responseText = textContainer.GetChild(1).GetComponent<Text>();
			}
			else {
				initialTextContainer.gameObject.SetActive(true);
				textContainer.gameObject.SetActive(false);
				
				responseText = initialTextContainer.GetChild(1).GetComponent<Text>();
			}

			responseText.text = value;

		}
	}
	
	public bool Initial { 
		set {
			initial = value;
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
}

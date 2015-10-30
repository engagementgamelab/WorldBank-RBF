using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScenarioChatMessage : PortraitTextBox {

	public Transform initialTextContainer;
	public Transform feedbackTextContainer;
	public Transform textContainer;

	public Image leftPortrait;
	public Image defaultArrow;
	public Image initialArrow;
	public Image feedbackArrow;

    public IndicatorsMessage feedbackIndicators;

	bool initial = false;
	bool feedback = false;
	bool leftSide = true;
	bool rightSide = false;

	Text responseText;

	int initPadding = -195;
	int currentPadding; 

	HorizontalLayoutGroup layout;

	public string Content {

		get { return responseText.text; }
		set { 
			
			layout = transform.GetChild(0).gameObject.GetComponent<HorizontalLayoutGroup>();

			currentPadding = initPadding;

			textContainer.gameObject.SetActive(false);
			initialTextContainer.gameObject.SetActive(false);
			feedbackTextContainer.gameObject.SetActive(false);

			defaultArrow.gameObject.SetActive(false);
			initialArrow.gameObject.SetActive(false);
			feedbackArrow.gameObject.SetActive(false);

			if(initial) {
				initialTextContainer.gameObject.SetActive(true);
				initialArrow.gameObject.SetActive(true);

				responseText = initialTextContainer.GetChild(1).GetComponent<Text>();
			}
			else if(feedback) {
				feedbackTextContainer.gameObject.SetActive(true);
				feedbackArrow.gameObject.SetActive(true);
				
				responseText = feedbackTextContainer.GetChild(1).GetComponent<Text>();				
			}
			else {
				textContainer.gameObject.SetActive(true);
				defaultArrow.gameObject.SetActive(true);

				responseText = textContainer.GetChild(1).GetComponent<Text>();
			}
			
			responseText.text = value;

			StartCoroutine(CoShow());

		}
	}
	
	public bool Initial { 
		set {
			initial = value;
		}
	}
	
	public bool Feedback { 
		set {
			feedback = value;
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

	IEnumerator CoShow () {

		float eTime =  0f;
		float time = 0.8f;

		while (eTime < time) {
			eTime += Time.deltaTime;
			float top = Mathf.Lerp (initPadding, 0, eTime / time);

			layout.padding = new RectOffset(0, 0, System.Convert.ToInt32(top), 0);
			yield return null;
		}
	}
}

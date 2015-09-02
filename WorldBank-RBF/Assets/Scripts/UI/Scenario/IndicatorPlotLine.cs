using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndicatorPlotLine : MonoBehaviour {

	public float positionOffset;

	public float StartPoint { 
		set { startingYPos = value; }
	}

	public float EndPoint { 
		set { endingYPos = value; }
	}

	public float Delta { 
		set { 
				xDelta = value;

				animateDelay = (xDelta/positionOffset);
		}
	}

	public int Type { 
		set {
			type = value; 

			if(type == 1)
				currentColor = new Color(1, .82f, .17f);
			else if(type == 2)
				currentColor = new Color(.2f, .76f, 1);
		}
	}

	public Transform Parent { 
		set { transParent = value; }
	}

	Transform transParent;
	
	float startingYPos;
	float endingYPos;
	float xDelta;
	// float targetWidth;

	float animateDelay;
	float animateStart;
	
	int type;

	Color currentColor = new Color(0.78f, 1, 0);

	void Update () {

		// if(animateStart >= animateDelay) {

		// 	while(rectTransform.sizeDelta.x < targetWidth)
		// 		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + .0005f, 7);

		// }
		// else
		// 	animateStart += Time.deltaTime;

	}

	// Use this for initialization
	public void Initialize() {

		RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

		Vector3 differenceVector = new Vector2(xDelta, endingYPos) - new Vector2(xDelta-positionOffset, startingYPos);
		float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;

		transform.SetParent(transParent);

		rectTransform.sizeDelta = new Vector2(differenceVector.magnitude, rectTransform.rect.height);
		rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
		rectTransform.localScale = Vector3.one;

		rectTransform.localPosition = new Vector2(xDelta-positionOffset, startingYPos);
		rectTransform.anchoredPosition = new Vector2(xDelta-positionOffset, startingYPos);

		gameObject.GetComponent<Image>().color = currentColor;

		foreach(Image img in gameObject.GetComponentsInChildren<Image>(true))		
			img.color = currentColor;

		animateStart = 0;

	}
}

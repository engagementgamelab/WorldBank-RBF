using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndicatorPlotLine : Image {

	public float StartPoint { 
		set { startingYPos = value; }
	}

	public float EndPoint { 
		set { endingYPos = value; }
	}

	public float Delta { 
		set { 
				xDelta = value;

				animateDelay = (xDelta/70);
		}
	}

	public int Type { 
		set {
			type = value; 

			if(type == 1)
				currentColor = Color.green;
			else if(type == 2)
				currentColor = Color.red;
		}
	}

	public Transform Parent { 
		set { transParent = value; }
	}

	Transform transParent;
	float startingYPos = 0;
	float endingYPos = 0;
	float xDelta = 0;
	float targetWidth = 0;

	float animateDelay = 0;
	float animateStart = 0;
	
	int type = 0;

	Color currentColor = Color.blue;

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

		Vector3 differenceVector = new Vector2(xDelta, endingYPos) - new Vector2(xDelta-70, startingYPos);
		float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;

		transform.SetParent(transParent);

		// targetWidth = differenceVector.magnitude;

		rectTransform.sizeDelta = new Vector2(differenceVector.magnitude, 7);
		rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
		rectTransform.localScale = Vector3.one;

		rectTransform.localPosition = new Vector2(xDelta-70, startingYPos);
		rectTransform.anchoredPosition = new Vector2(xDelta-70, startingYPos);

		color = currentColor;
		animateStart = 0;

	}
}

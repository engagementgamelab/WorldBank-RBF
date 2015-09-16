using UnityEngine;
using UnityEngine.UI;

public class ActionTaken : MonoBehaviour {

	public Text actionNameText;
	public void Display(string actionName, int[] indicatorValues) {

		actionNameText.text = actionName;

		if(indicatorValues[0] < 0 || indicatorValues[1] < 0 || indicatorValues[2] < 0)
			actionNameText.color = new Color(.96f, .27f, .28f, 1);
		else
			actionNameText.color = new Color(.67f, .8f, .32f, 1);

	}

}

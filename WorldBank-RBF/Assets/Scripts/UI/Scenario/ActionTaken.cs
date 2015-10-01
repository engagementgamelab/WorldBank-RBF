using UnityEngine;
using UnityEngine.UI;

public class ActionTaken : MonoBehaviour {

	public Text actionNameText;

	public Text vaccinationsText;
	public Text birthsText;
	public Text qocText;

	public void Display(string actionName, int[] indicatorValues) {

		actionNameText.text = actionName;

		vaccinationsText.text = indicatorValues[0].ToString();
		birthsText.text = indicatorValues[1].ToString();
		qocText.text = indicatorValues[2].ToString();

		if(indicatorValues[0] < 0 || indicatorValues[1] < 0 || indicatorValues[2] < 0)
			actionNameText.color = new Color(.96f, .27f, .28f, 1);
		else
			actionNameText.color = new Color(.67f, .8f, .32f, 1);

	}

}

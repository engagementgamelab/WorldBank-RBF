using UnityEngine;
using UnityEngine.UI;

public class ActionTaken : MonoBehaviour {

	public Text actionNameText;
	public Text vacText;
	public Text facilityText;
	public Text qocText;

	public void Display(string actionName, int[] indicatorValues) {

		actionNameText.text = actionName;

		vacText.text = (indicatorValues[0] > 0) ? "+" : "-";
		facilityText.text = (indicatorValues[1] > 0) ? "+" : "-";
		qocText.text = (indicatorValues[2] > 0) ? "+" : "-";

	}

}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResultsScreen : MonoBehaviour {

	public Text grade;
	public Text description;
	public IndicatorsCanvas indicatorsCanvas;

	public void SetResults (Dictionary<string, object> results) {

		int[] indicators = results["indicators"] as int[];
		grade.text = results["grade"].ToString ();
		description.text = results["description"].ToString ();
		IndicatorsCanvas.GoalAffects = results["goal"] as int[];

		indicatorsCanvas.UpdateIndicators (indicators[1], indicators[0], indicators[2]);
		indicatorsCanvas.Open ();
	}

	public void OnPressNext () {
		MenusManager.defaultScreen = "plan";
		Application.LoadLevel ("Menus");
	}
}

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DataCanvas : NotebookCanvas {

	public GameObject feedbackPanel;

	public Text textBirthsValue;
	public Text textVaccinationsValue;
	public Text textQOCValue;

	public override void Open () {
		feedbackPanel.SetActive (NotebookManager.Instance.MakingPlan);
	}

	// Update indicators
	public override void UpdateIndicators(int intBirths, int intVaccinations, int intQOC) {

		int currBirths;
		int currVaccinations;
		int currQOC;

		// Get current indicator values
		Int32.TryParse(textBirthsValue.text.Replace("%", ""), out currBirths);
		Int32.TryParse(textVaccinationsValue.text.Replace("%", ""), out currVaccinations);
		Int32.TryParse(textQOCValue.text.Replace("%", ""), out currQOC);

		// Update
		textBirthsValue.text = (currBirths + intBirths) + "%";
		textVaccinationsValue.text = (currVaccinations + intVaccinations) + "%";
		textQOCValue.text = (currQOC + intQOC) + "%";

	}
}

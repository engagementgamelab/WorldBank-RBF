using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class IndicatorsCanvas : NotebookCanvas {

	public RectTransform dataColumns;

	int currentYearShown = 1;
	List<string[]> appliedAffects = new List<string[]>();

	// Display indicators when made visible
	void OnEnable() {

		ObjectPool.DestroyChildren<IndicatorColumn>(dataColumns.transform);

		foreach(string[] affects in appliedAffects) {
		
			IndicatorColumn indicators = ObjectPool.Instantiate<IndicatorColumn>();

			indicators.Vaccinations = affects[0];
			indicators.FacilityBirths = affects[1];
			indicators.QualityOfCare = affects[2];

			indicators.transform.SetParent(dataColumns.transform);

		}

		ShowYear(currentYearShown);

    }

	// Update indicators
	public override void UpdateIndicators(int intBirths, int intVaccinations, int intQOC) {

		appliedAffects.Add(new string[] { intBirths.ToString(), intVaccinations.ToString(), intQOC.ToString() });

	}

	public void ShowYear(int intYr=1) {

		int columnIndex = 0;
		int max = (12 * intYr);
		int floor = max - 12;

		foreach(Transform column in dataColumns.transform)
		{
			column.gameObject.SetActive(!(columnIndex < floor || columnIndex > max));
			columnIndex++;
		}

		currentYearShown = intYr;

	}
}

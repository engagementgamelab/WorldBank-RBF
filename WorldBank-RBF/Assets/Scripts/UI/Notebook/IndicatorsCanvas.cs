using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class IndicatorsCanvas : NotebookCanvas {

	public RectTransform dataColumns;

	int currentYearShown = 1;

	// Update indicators
	public override void UpdateIndicators(int intBirths, int intVaccinations, int intQOC) {
		IndicatorColumn indicators = ObjectPool.Instantiate<IndicatorColumn>();

		indicators.Vaccinations = intBirths.ToString();
		indicators.FacilityBirths = intVaccinations.ToString();
		indicators.QualityOfCare = intQOC.ToString();

		indicators.transform.SetParent(dataColumns.transform);

		ShowYear(currentYearShown);

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

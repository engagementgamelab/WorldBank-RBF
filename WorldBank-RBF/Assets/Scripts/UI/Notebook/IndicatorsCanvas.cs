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

		RenderIndicators();

		ShowYear(currentYearShown);

    }

    void RenderIndicators() {

		ObjectPool.DestroyChildren<IndicatorColumn>(dataColumns.transform);

		int ind = 0;
		float xPos = dataColumns.rect.width / 12;
		float factor = (dataColumns.rect.height / 100);

		foreach(string[] affects in appliedAffects) {

			int affectType = 0;

			foreach(string affect in affects) {

				float startingYPos = 0;
				float yPos = 0;

				Single.TryParse(affect, out yPos);

				if(ind > 0)
					Single.TryParse(appliedAffects[ind-1][affectType], out startingYPos);
				else
					Single.TryParse(appliedAffects[0][affectType], out startingYPos);

				startingYPos *= factor;
				float yPosFactored = yPos * factor;

				IndicatorPlotLine plotLine = ObjectPool.Instantiate<IndicatorPlotLine>();
				
				plotLine.StartPoint = startingYPos;
				plotLine.EndPoint = yPosFactored;
				plotLine.Delta = xPos;
				plotLine.Type = affectType;

				plotLine.Parent = dataColumns.transform;

				plotLine.Initialize();
				
				affectType++;

			}

			xPos += (dataColumns.rect.width / 12);
			ind++;
		}

    }

	// Update indicators
	public override void UpdateIndicators(int intBirths, int intVaccinations, int intQOC) {

		intBirths = Mathf.Clamp(intBirths, 0, 100);
		intVaccinations = Mathf.Clamp(intVaccinations, 0, 100);
		intQOC = Mathf.Clamp(intQOC, 0, 100);

		appliedAffects.Add(new string[] { intBirths.ToString(), intVaccinations.ToString(), intQOC.ToString() });

		RenderIndicators();

	}

	public void ShowYear(int intYr=1) {

		int columnIndex = 0;
		int max = (36 * intYr);
		int floor = max - 36;

		foreach(Transform column in dataColumns.transform)
		{
			column.gameObject.SetActive(!(columnIndex < floor || columnIndex > max));
			columnIndex++;
		}

		currentYearShown = intYr;

	}
}

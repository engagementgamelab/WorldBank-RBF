using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class IndicatorsCanvas : NotebookCanvas {

	public Animator scenarioAnimator;

	public RectTransform dataColumns;
	public RectTransform scenarioInfo;

	public static List<int[]> AppliedAffects = new List<int[]>();
	List<string[]> displayedAffects = new List<string[]>();

	Animator animator;
	
	int currentYearShown = 1;

	void Start() {

		animator = gameObject.GetComponent<Animator>();

	}

	// Display indicators when made visible
	void OnEnable() {

		RenderIndicators();

		ShowYear(currentYearShown);

    }

    void OnDisable() {

		scenarioInfo.gameObject.SetActive(true);

    }

    void RenderIndicators() {

		ObjectPool.DestroyChildren<IndicatorColumn>(dataColumns.transform);

		int ind = 0;
		float xPos = dataColumns.rect.width / 12;
		float factor = (dataColumns.rect.height / 100);

		foreach(string[] affects in displayedAffects) {

			int affectType = 0;

			foreach(string affect in affects) {

				float startingYPos = 0;
				float yPos = 0;

				Single.TryParse(affect, out yPos);

				if(ind > 0)
					Single.TryParse(displayedAffects[ind-1][affectType], out startingYPos);
				else
					Single.TryParse(displayedAffects[0][affectType], out startingYPos);

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

		AppliedAffects.Add(new [] { intBirths, intVaccinations, intQOC });
		displayedAffects.Add(new [] { intBirths.ToString(), intVaccinations.ToString(), intQOC.ToString() });

		RenderIndicators();

		// SFX
		// AudioManager.Sfx.Play ("graphupdated", "Phase2");

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

	public override void Open() {

		animator.Play("IndicatorsOpen");
		scenarioAnimator.Play("ScenarioClose");

	}

}
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class IndicatorsCanvas : NotebookCanvas {

	public Animator scenarioAnimator;

	public RectTransform dataColumns;
	public RectTransform scenarioInfo;

	public RectTransform[] dataBarBgs;
	public RectTransform[] dataBarFills;

	public Text[] dataBarCurrentText;
	public Text[] dataBarGoalText;

	public static List<int[]> AppliedAffects = new List<int[]>();
	public static int[] GoalAffects;

	string[] previousAffects;
	string[] currentAffects;

	Animator animator;
	
	void Start() {

		animator = gameObject.GetComponent<Animator>();

	}

    void OnDisable() {

		scenarioInfo.gameObject.SetActive(true);

    }

    void RenderIndicators() {

		int ind = 0;

		foreach(string affect in currentAffects) {
		
				float affectVal = 0;
				float affectValPrevious = 0;

				Single.TryParse(currentAffects[ind], out affectVal);
				
				if(previousAffects[ind] != null) {
					Single.TryParse(previousAffects[ind], out affectValPrevious);
					dataBarCurrentText[ind].text = (affectVal - affectValPrevious).ToString();
				}

				Debug.Log(affectVal);

				affectVal = Mathf.Clamp(affectVal, 0, (float)GoalAffects[ind]);

				affectVal = (affectVal / (float)GoalAffects[ind]) * dataBarBgs[ind].rect.width;

				dataBarFills[ind].sizeDelta = new Vector2(affectVal, dataBarFills[ind].rect.height);
				dataBarGoalText[ind].text = GoalAffects[ind].ToString();

		

			ind++;

		}


    }

	// Update indicators
	public override void UpdateIndicators(int intBirths, int intVaccinations, int intQOC) {

		intBirths = Mathf.Clamp(intBirths, 0, 100);
		intVaccinations = Mathf.Clamp(intVaccinations, 0, 100);
		intQOC = Mathf.Clamp(intQOC, 0, 100);

		AppliedAffects.Add(new [] { intBirths, intVaccinations, intQOC });
		currentAffects = new [] { intBirths.ToString(), intVaccinations.ToString(), intQOC.ToString() };

		RenderIndicators();

		// SFX
		// AudioManager.Sfx.Play ("graphupdated", "Phase2");

	}
	
	public void DebugIndicators() {
		GoalAffects = new [] {80, 85, 90};

		if(currentAffects != null)
			previousAffects = currentAffects;

		UpdateIndicators(new System.Random().Next(0, 100), new System.Random().Next(0, 100), new System.Random().Next(0, 100));
	}

	public override void Open() {

		animator.Play("IndicatorsOpen");
		scenarioAnimator.Play("ScenarioClose");

	}

}
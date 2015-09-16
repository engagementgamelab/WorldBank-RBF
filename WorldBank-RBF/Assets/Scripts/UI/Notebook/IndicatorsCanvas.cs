using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class IndicatorsCanvas : NotebookCanvas {

	public Animator scenarioAnimator;
	public Text yearEndPromptText;

	public RectTransform actionsColumn;
	public RectTransform scenarioInfo;

	public RectTransform[] dataBarBgs;
	public RectTransform[] dataBarFills;

	public Text[] dataBarCurrentText;

	public static Dictionary<string, int[]> SelectedOptions = new Dictionary<string, int[]>();
	public static List<int[]> AppliedAffects = new List<int[]>();
	public static int[] GoalAffects;

	public List<ScenarioOptionButton> _btnListOptions;

	string[] previousAffects;
	string[] currentAffects;

	List<float> barSizesTarget = new List<float>();
	List<float> barSizesCurrent = new List<float>();

	bool showIndicators;

	Animator animator;
	
	void Start() {

		animator = gameObject.GetComponent<Animator>();

	}

	void Update() {

		if(currentAffects != null && showIndicators) {
			int ind = 0;
			
			foreach(string affect in currentAffects) {
				if(barSizesCurrent[ind] < barSizesTarget[ind])
					barSizesCurrent[ind] = barSizesCurrent[ind]+2;

				dataBarFills[ind].sizeDelta = new Vector2(barSizesCurrent[ind], dataBarFills[ind].rect.height);
				ind++;
			}

		}

	}

    void OnDisable() {

		scenarioInfo.gameObject.SetActive(true);

    }

    void RenderIndicators() {

		int ind = 0;

		barSizesTarget.Clear();
		barSizesCurrent.Clear();

		foreach(string affect in currentAffects) {
	
			float affectVal = 0;
			float affectValPrevious = 0;

			Single.TryParse(currentAffects[ind], out affectVal);
			
			if(previousAffects !=null && previousAffects[ind] != null) {
				Single.TryParse(previousAffects[ind], out affectValPrevious);
				dataBarCurrentText[ind].text = ((float)GoalAffects[ind] - affectVal).ToString();
			}

			affectVal = Mathf.Clamp(affectVal, 0, (float)GoalAffects[ind]);

			affectVal = (affectVal / (float)GoalAffects[ind]) * dataBarBgs[ind].rect.width;

			dataBarFills[ind].sizeDelta = new Vector2(0, dataBarFills[ind].rect.height);

			barSizesCurrent.Add(0f);
			barSizesTarget.Add(affectVal);
			
			ind++;

		}

		foreach(KeyValuePair<string, int[]> action in SelectedOptions) {

			ActionTaken actionTaken = ObjectPool.Instantiate<ActionTaken>("Scenario");

			actionTaken.Display(action.Key, action.Value);

			actionTaken.transform.SetParent(actionsColumn.transform);

		}

    }

	// Update indicators
	public override void UpdateIndicators(int intBirths, int intVaccinations, int intQOC) {

		intBirths = Mathf.Clamp(intBirths, 0, 100);
		intVaccinations = Mathf.Clamp(intVaccinations, 0, 100);
		intQOC = Mathf.Clamp(intQOC, 0, 100);

		if(currentAffects != null)
			previousAffects = currentAffects;

		AppliedAffects.Add(new [] { intBirths, intVaccinations, intQOC });
		currentAffects = new [] { intBirths.ToString(), intVaccinations.ToString(), intQOC.ToString() };

		RenderIndicators();

		// SFX
		// AudioManager.Sfx.Play ("graphupdated", "Phase2");

	}

	public override void Open() {

		animator.Play("IndicatorsOpen");
		scenarioAnimator.Play("ScenarioClose");

		showIndicators = true;

	}

	public override void Close() {

		animator.Play("IndicatorsClose");
		scenarioAnimator.Play("ScenarioOpen");

		showIndicators = false;
	}

    public void EndYear (Models.ScenarioConfig scenarioConfig, int currentYear) {

    	bool indicatorsNegative = !DataManager.IsIndicatorDeltaGood(
														    		IndicatorsCanvas.AppliedAffects[0], 
														    		IndicatorsCanvas.AppliedAffects[IndicatorsCanvas.AppliedAffects.Count-1]
														    	  );

    	string strActionsSummary = "<i>Your actions for this year:</i>\n";
    	string[] yearEndPrompts = (currentYear == 1) ? scenarioConfig.prompt_year_1 : scenarioConfig.prompt_year_2;
    	string yearEndMessage;

    	// If player has not made any changes, choose first prompt
    	if(SelectedOptions.Count == 0)
    		yearEndMessage = yearEndPrompts[0];

    	// If player has made changes, choose prompt based on if indicators are positive
    	else
    		yearEndMessage = yearEndPrompts[Convert.ToInt32(indicatorsNegative)+1];

	    // else
		   //  strActionsSummary = "<i><b>You did not take any actions this year!</b></i>";

    	yearEndPromptText.text = yearEndMessage;

    	AddYearEndOptions(scenarioConfig.choices);

		SelectedOptions.Clear();
    }

	void AddYearEndOptions (Dictionary<string, string>[] options) {

		if (options.Length > 3)
			throw new System.Exception ("Only 3 year-end options can be displayed on the screen at a time.");

		int btnIndex = 0;

		foreach(ScenarioOptionButton button in _btnListOptions)
			button.gameObject.SetActive(false);

		foreach (Dictionary<string, string> option in options) {
			
			string optionTxt = option["text"];
			string optionVal = option["load"];

			ScenarioOptionButton btnChoice = _btnListOptions[btnIndex];
			btnChoice.gameObject.SetActive (true);
			btnIndex ++;

			btnChoice.Text = optionTxt;
			btnChoice.Button.onClick.RemoveAllListeners ();
			btnChoice.Button.onClick.AddListener (() => YearEndOptionSelected (optionTxt, optionVal));
		}

		ScenarioOptionButton btnNextYear = _btnListOptions[btnIndex];
		btnNextYear.gameObject.SetActive (true);
		btnNextYear.Text = "Go to next year";
		btnNextYear.Button.onClick.RemoveAllListeners ();
		btnNextYear.Button.onClick.AddListener(() => Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT_YEAR)));

	}

	void YearEndOptionSelected (string optionTxt, string optionVal) {

		// Update selected decisions
		DataManager.ScenarioDecisions(optionTxt);

		// Broadcast to affect current scenario path with the config value
		Events.instance.Raise(new ScenarioEvent(ScenarioEvent.DECISION_SELECTED, optionVal));
		Events.instance.Raise(new ScenarioEvent(ScenarioEvent.NEXT_YEAR));
	}
	
	public void DebugIndicators() {
		GoalAffects = new [] {80, 85, 90};

		UpdateIndicators(new System.Random().Next(0, 100), new System.Random().Next(0, 100), new System.Random().Next(0, 100));
	}

}
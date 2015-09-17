using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class IndicatorsCanvas : NotebookCanvas {

	public Animator scenarioAnimator;
	public Text yearEndPromptText;
	public Text phaseEndPromptText;

	public RectTransform actionsView;
	public RectTransform actionsColumn;
	public RectTransform decisionPanel;
	public RectTransform summaryPanel;

	public RectTransform[] dataBarBgs;
	public RectTransform[] dataBarFills;
	public CanvasGroup[] dataBarStars;

	public Text[] dataBarCurrentText;
	public Image[] dataBarArrowsUp;
	public Image[] dataBarArrowsDown;

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
				if(barSizesCurrent[ind] < barSizesTarget[ind]+3)
					barSizesCurrent[ind] = barSizesCurrent[ind]+3;
				else if(ind < 2)
					ind++;

				dataBarFills[ind].sizeDelta = new Vector2(Mathf.Clamp(barSizesCurrent[ind], 0, dataBarBgs[ind].rect.width), dataBarFills[ind].rect.height);
			}

		}

	}

    void RenderIndicators() {

		int ind = 0;

		barSizesTarget.Clear();
		barSizesCurrent.Clear();

		foreach(string affect in currentAffects) {
	
			float affectVal = 0;
			float affectGoal = (float)GoalAffects[ind];

			Single.TryParse(currentAffects[ind], out affectVal);
			
			// Set text to affect delta
			if(previousAffects !=null && previousAffects[ind] != null) {
				dataBarCurrentText[ind].text = affectVal.ToString();

				dataBarArrowsUp[ind].gameObject.SetActive(affectVal > 0);
				dataBarArrowsDown[ind].gameObject.SetActive(affectVal < 0);
			}

			// Get affect vs goal
			float currentVal = Mathf.Clamp( (affectGoal + affectVal), 0, affectGoal );

			float barWidth = Mathf.Clamp( (currentVal / affectGoal) * dataBarBgs[ind].rect.width, 0, dataBarBgs[ind].rect.width );

			dataBarFills[ind].sizeDelta = new Vector2(0, dataBarFills[ind].rect.height);

			// Show star?
		 	dataBarStars[ind].alpha = (currentVal >= affectGoal) ? 1 : 0;

			barSizesCurrent.Add(0f);
			barSizesTarget.Add(barWidth);
			
			ind++;

		}

		foreach(KeyValuePair<string, int[]> action in SelectedOptions) {

			ActionTaken actionTaken = ObjectPool.Instantiate<ActionTaken>("Scenario");

			actionTaken.Display(action.Key, action.Value);

			actionTaken.transform.SetParent(actionsColumn.transform);
			actionTaken.transform.localScale = Vector3.one;

		}

		// TODO: No options?
		if(SelectedOptions.Count == 0) {

		   //  strActionsSummary = "<i><b>You did not take any actions this year!</b></i>"

		}

    }

	// Update indicators
	public override void UpdateIndicators(int intBirths, int intVaccinations, int intQOC) {

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

    	string[] yearEndPrompts = (currentYear == 1) ? scenarioConfig.prompt_year_1 : scenarioConfig.prompt_year_2;
    	string yearEndMessage;

    	// If player has not made any changes, choose first prompt
    	if(SelectedOptions.Count == 0)
    		yearEndMessage = yearEndPrompts[0];

    	// If player has made changes, choose prompt based on if indicators are positive
    	else
    		yearEndMessage = yearEndPrompts[Convert.ToInt32(indicatorsNegative)+1];;

    	if(currentYear == 3)
    	{
    		summaryPanel.gameObject.SetActive(true);
    		decisionPanel.gameObject.SetActive(false);
    		actionsView.gameObject.SetActive(false);

	    	phaseEndPromptText.text = yearEndMessage;
    	}
    	else
    	{
    		summaryPanel.gameObject.SetActive(false);
    		decisionPanel.gameObject.SetActive(true);
    		actionsView.gameObject.SetActive(true);

	    	yearEndPromptText.text = yearEndMessage;
    	}

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
		GoalAffects = new [] {10, 25, 50};

		SelectedOptions.Clear();

		SelectedOptions.Add("Contract Outside Organization to Administer Program", new [] {-1, 4, -3});
		SelectedOptions.Add("Conditional Cash Transfer", new [] {1, 4, 3});
		SelectedOptions.Add("Clarify the separation of functions and responsibilities", new [] {4, -2, -9});
		SelectedOptions.Add("Terminate the contract at the end of this year", new [] {0, 3, 1});
		SelectedOptions.Add("Award bonuses to hospital staff", new [] {0, 3, 1});

		int a = new System.Random().Next(-10, 4);
		int b = new System.Random().Next(-6, 10);
		int c = new System.Random().Next(-4, 4);

		Debug.Log (a + ", " + b + ", " + c);

		UpdateIndicators(a, b, c);

		showIndicators = true;
	}

}
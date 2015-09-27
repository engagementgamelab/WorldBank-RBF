using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class IndicatorsCanvas : NotebookCanvas {

	public bool phaseOne = false;
	public Animator scenarioAnimator;

	public Text yearEndPromptText;
	public Text phaseEndPromptText;
	public Text timerText;

	public RectTransform actionsView;
	public RectTransform actionsColumn;
	public RectTransform decisionPanel;
	public RectTransform summaryPanel;

	public GameObject prevArrow;
	public GameObject nextArrow;

	public RectTransform[] dataBarBgs;
	public RectTransform[] dataBarFills;
	public Animator[] dataBarStars;

	public Text[] dataBarCurrentText;
	public Image[] dataBarArrowsUp;
	public Image[] dataBarArrowsDown;

	public static Dictionary<string, int[]> SelectedOptions = new Dictionary<string, int[]>();
	public static List<int[]> AppliedAffects = new List<int[]>();
	public static int[] GoalAffects;

	public List<ScenarioOptionButton> _btnListOptions;

	string[] previousAffects;
	string[] currentAffects;

	bool[] atGoal = new bool[3] {false, false, false};

	List<float> barSizesTarget = new List<float>();
	List<float> barSizesCurrent = new List<float>();

	bool showIndicators;
	float indicatorAnimateDelta = 5;

	int currentActionTakenIndex;

	// Animator animator;
	Animator animator = null;
	Animator Animator {
		get {
			if (animator == null) {
				animator = GetComponent<Animator> ();
			}
			return animator;
		}
	}

	void Update() {

		if(currentAffects != null && barSizesTarget.Count > 0 && showIndicators) {
			int ind = 0;
			
			foreach(string affect in currentAffects) {
				indicatorAnimateDelta -= 0.003f;
				if(barSizesCurrent[ind] < barSizesTarget[ind]+indicatorAnimateDelta)
					barSizesCurrent[ind] = barSizesCurrent[ind]+indicatorAnimateDelta;
				else {

					// Show star?
					if(atGoal[ind])
						dataBarStars[ind].Play("IndicatorsStarShow");

					if(ind < 2) {
						indicatorAnimateDelta = 5;
						ind++;
					}

				}

				dataBarFills[ind].sizeDelta = new Vector2(Mathf.Clamp(barSizesCurrent[ind], 0, dataBarBgs[ind].rect.width), dataBarFills[ind].rect.height);
			}

		}

	}

    IEnumerator RenderIndicators() {

		int ind = 0;

		barSizesTarget.Clear();
		barSizesCurrent.Clear();

		if (!phaseOne)
			ObjectPool.DestroyChildren<ActionTaken>(actionsColumn.transform, "Scenario");

		while(ind < currentAffects.Length) {
	
			float affectVal;
			float affectValPrev;

			float affectGoal = (float)GoalAffects[ind];

			Single.TryParse(currentAffects[ind], out affectVal);
			Single.TryParse(previousAffects[ind], out affectValPrev);
			
			// Set text to affect delta
			if(!phaseOne && previousAffects !=null && previousAffects[ind] != null) {
				
				dataBarCurrentText[ind].text = (affectVal - affectValPrev).ToString();
				
				if((affectVal - affectValPrev) == 0) {
					dataBarArrowsUp[ind].gameObject.SetActive(false);
					dataBarArrowsDown[ind].gameObject.SetActive(false);
				} else {
					dataBarArrowsUp[ind].gameObject.SetActive((affectVal - affectValPrev) > 0);
					dataBarArrowsDown[ind].gameObject.SetActive((affectVal - affectValPrev) < 0);
				}

			}

			// Get affect vs goal
			float currentVal = Mathf.Clamp( affectVal, 0, affectGoal );

			float barWidth = Mathf.Clamp( (currentVal / affectGoal) * dataBarBgs[ind].rect.width, 0, dataBarBgs[ind].rect.width );

			dataBarFills[ind].sizeDelta = new Vector2(0, dataBarFills[ind].rect.height);

			// Show star?
			atGoal[ind] = (currentVal >= affectGoal);

			barSizesCurrent.Add(0f);
			barSizesTarget.Add(barWidth);
			
			ind++;

		}

		if(SelectedOptions.Count > 0)
			ShowActionTaken(0);

		// TODO: No options?
		if(SelectedOptions.Count == 0) {

		   //  strActionsSummary = "<i><b>You did not take any actions this year!</b></i>"

		}

		SelectedOptions.Clear();

    	yield return new WaitForSeconds(1.1f);

		showIndicators = true;

    }

	// Update indicators
	public override void UpdateIndicators(int intBirths, int intVaccinations, int intQOC) {

		if(currentAffects != null)
			previousAffects = currentAffects;

		if (phaseOne)
			previousAffects = new [] { 0.ToString (), 0.ToString (), 0.ToString () };

		AppliedAffects.Add(new [] { intBirths, intVaccinations, intQOC });
		currentAffects = new [] { intBirths.ToString(), intVaccinations.ToString(), intQOC.ToString() };

	}

	public override void Open() {

		Animator.Play("IndicatorsOpen");

		if (!phaseOne) {
			scenarioAnimator.Play("ScenarioClose");
			timerText.gameObject.SetActive(false);
		}
		
	 	StartCoroutine("RenderIndicators");

	}

	public override void Close() {

		Animator.Play("IndicatorsClose");

		if (!phaseOne) {
			scenarioAnimator.Play("ScenarioOpen");
			timerText.gameObject.SetActive(true);
		}

		showIndicators = false;

	}

    public void EndYear (Models.ScenarioConfig scenarioConfig, int currentYear, int twistIndex) {

    	bool indicatorsNegative = !DataManager.IsIndicatorDeltaGood(
														    		IndicatorsCanvas.AppliedAffects[0], 
														    		IndicatorsCanvas.AppliedAffects[IndicatorsCanvas.AppliedAffects.Count-1]
														    	  );

    	string[] yearEndPrompts = (currentYear == 1) ? scenarioConfig.prompt_year_1 : scenarioConfig.prompt_year_2;

    	if(currentYear == 2 && twistIndex == 1) {
	    	yearEndPrompts = scenarioConfig.prompt_year_2_twist;
    		AddYearEndOptions(scenarioConfig.choices_twist);
    	}
    	else
    		AddYearEndOptions(scenarioConfig.choices);

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

		 	// End of year 3; update user record
	        Dictionary<string, object> formFields = new Dictionary<string, object>();
	        formFields.Add("save_phase_2", true);

			PlayerManager.Instance.SaveData (formFields, UserSaveCallback);

    	}
    	else
    	{
    		summaryPanel.gameObject.SetActive(false);
    		decisionPanel.gameObject.SetActive(true);
    		actionsView.gameObject.SetActive(true);

	    	yearEndPromptText.text = yearEndMessage;
    	}
    }

    public void ShowPreviousAction() {
    	ShowActionTaken(currentActionTakenIndex - 1);
    }

    public void ShowNextAction() {
    	ShowActionTaken(currentActionTakenIndex + 1);
    }

    public void ShowActionTaken(int index) {

		ObjectPool.DestroyChildren<ActionTaken>(actionsColumn.transform, "Scenario");

		string actionName = SelectedOptions.Keys.ToList()[index];
		int[] actionIndicators = SelectedOptions[actionName];

		ActionTaken actionTaken = ObjectPool.Instantiate<ActionTaken>("Scenario");

		actionTaken.Display(actionName, actionIndicators);

		actionTaken.transform.SetParent(actionsColumn.transform);
		actionTaken.transform.localScale = Vector3.one;
		actionTaken.GetComponent<RectTransform>().offsetMax = Vector2.zero;
		actionTaken.GetComponent<RectTransform>().offsetMin = Vector2.zero;

		currentActionTakenIndex = index;

		prevArrow.SetActive(!(index == 0));
		nextArrow.SetActive(!(index == (SelectedOptions.Count-1)));

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

	// Get response from saving user state
	void UserSaveCallback(Dictionary<string, object> response) {
		
		Debug.Log("Phase two user status updated.");
	
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
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PrioritizationManager : NotebookCanvas {

	// public GameObject continueButton;
	public Button continueButton;

	public RectTransform namingPanel;
	public RectTransform feedbackPanel;

	public Text scoreText;
	public Text feedbackText;

	new public PilotYearAnimation animation;
	public ResultsScreen results;

	public Dictionary<string, object> Results { get; private set; }

	float animationTime = 15f;

	void Awake () {
		PlayerData.TacticPriorityGroup.onUpdate += OnUpdatePriorities;
		continueButton.interactable = false;
	}

	public void NamePlan() {

		namingPanel.gameObject.SetActive(true);

	}

	public void SubmitPlan(Text planNameInput) {

		AudioManager.Music.StopAll ();
		AudioManager.Ambience.StopAll ();

    Dictionary<string, object> formFields = new Dictionary<string, object>();

    Models.Plan plan = new Models.Plan();

    plan.name = planNameInput.text;
    plan.tactics = PlayerData.TacticPriorityGroup.Tactics;

    formFields.Add("plan", plan);
    formFields.Add("save_plan", true);

		PlayerManager.Instance.SaveData (formFields, SubmitPlanCallback);

	}
	
	void OnUpdatePriorities () {

		continueButton.interactable = PlayerData.TacticPriorityGroup.Count == 6;
	
	}

	// Get response from submitting a plan
	void SubmitPlanCallback(Dictionary<string, object> response) {

	 	Results = response;

	 	namingPanel.gameObject.SetActive (false);
	 	animation.gameObject.SetActive (true);
	 	animation.Animate (animationTime);
	 	Invoke ("OpenResults", animationTime);		

		PlayerManager.Instance.TrackEvent("Plan Saved", "Phase One");

	}

	void OpenResults () {
		results.gameObject.SetActive (true);
		results.SetResults (Results);
	}
}

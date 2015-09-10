using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PrioritizationManager : NotebookCanvas {

	public GameObject continueButton;

	public RectTransform namingPanel;
	public RectTransform feedbackPanel;

	public Text scoreText;
	public Text feedbackText;

	void Awake () {
		PlayerData.TacticPriorityGroup.onUpdate += OnUpdatePriorities;
	}

	public override void Open () {
		continueButton.SetActive (NotebookManager.Instance.MakingPlan);
	}

	public void NamePlan() {

		namingPanel.gameObject.SetActive(true);

	}

	public void SubmitPlan(Text planNameInput) {

        Dictionary<string, object> formFields = new Dictionary<string, object>();

        Models.Plan plan = new Models.Plan();

        plan.name = planNameInput.text;
        plan.tactics = PlayerData.TacticPriorityGroup.Tactics;

        formFields.Add("plan", plan);

		PlayerManager.Instance.SaveData (formFields, SubmitPlanCallback);

	}

	// Continues to phase  two
	// Also, skips to phase two via "skip tab" button (won't be in test or final game)
	public void Continue() {

		// Clear all Object Pool objects and pools before loading new scene
		ObjectPool.Clear();

		Application.LoadLevel("PhaseTwo");

	}

	void OnUpdatePriorities () {
		continueButton.SetActive (PlayerData.TacticPriorityGroup.Count == 6);
	}

	// Get response from submitting a plan
	void SubmitPlanCallback(Dictionary<string, object> response) {

	 	scoreText.text = "Score: " + response["score"].ToString();
	 	feedbackText.text = response["description"].ToString();

	 	// Show feedback in data panel (allows player to continue)
		feedbackPanel.gameObject.SetActive(true);
		

		PlayerManager.Instance.TrackEvent("Plan Saved", "Phase One");

	}
}

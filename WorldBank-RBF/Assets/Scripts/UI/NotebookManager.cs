using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NotebookManager : MB {

	public GameObject map;
	public GameObject priorities;
	public GameObject data;
	public GameObject tabGroup;
	public GameObject notebookCollider;

	public RectTransform namingPanel;
	public RectTransform feedbackPanel;

	public Text scoreText;
	public Text feedbackText;

	Dictionary<string, GameObject> canvases;
	Dictionary<string, GameObject> Canvases {
		get {
			if (canvases == null) {
				canvases = new Dictionary<string, GameObject> ();
				canvases.Add ("map", map);
				canvases.Add ("priorities", priorities);
				canvases.Add ("data", data);
			}
			return canvases;
		}
	}

	CameraPositioner cameraPositioner = null;
	CameraPositioner CameraPositioner {
		get { 
			if (cameraPositioner == null) {
				cameraPositioner = MainCamera.Instance.Positioner; 
			}
			return cameraPositioner;
		}
	}

	bool open = false;
	string activeCanvas = "map";

	void Awake () {

		Close ();

	}

	public void OpenMap () {
		OpenCanvas ("map");
	}

	public void OpenPriorities () {
		OpenCanvas ("priorities");
	}

	public void OpenData () {
		OpenCanvas ("data");
	}

	public void ToggleNotebook () {
		if (open) {
			open = false;
			Close ();
		} else if (NPCFocusBehavior.Instance.FocusLevel == FocusLevel.Default) {
			open = true;
			Open ();
		}
	}

	public void NamePlan() {

		namingPanel.gameObject.SetActive(true);

	}


	public void SubmitPlan(Text planNameInput) {

        Dictionary<string, object> formFields = new Dictionary<string, object>();

        Models.Plan plan = new Models.Plan();

        plan.name = planNameInput.text;
        plan.tactics = PlayerData.PlanTacticGroup.GetUniqueTacticSymbols ();
        
        formFields.Add("plan", plan);

		PlayerManager.Instance.SaveData (formFields, SubmitPlanCallback);

	}

	// Continues to phase  two
	public void Continue() {

		Application.LoadLevel("PhaseTwo");

	}

	void OpenCanvas (string id) {
		foreach (var canvas in Canvases) {
			canvas.Value.SetActive (canvas.Key == id);
		}
		activeCanvas = id;
	}

	void Open () {
		OpenCanvas (activeCanvas);
		tabGroup.SetActive (true);
		notebookCollider.SetActive (true);
		CameraPositioner.Drag.Enabled = false;
	}

	void Close () {
		foreach (var canvas in Canvases) {
			canvas.Value.SetActive (false);
		}
		tabGroup.SetActive (false);
		notebookCollider.SetActive (false);
		CameraPositioner.Drag.Enabled = true;
	}

	// Get response from submitting a plan
	void SubmitPlanCallback(Dictionary<string, object> response) {

	 	scoreText.text = "Score: " + response["score"].ToString();
	 	feedbackText.text = response["description"].ToString();

	 	// Show feedback in data panel (allows player to continue)
		feedbackPanel.gameObject.SetActive(true);
		OpenData();

	}
}

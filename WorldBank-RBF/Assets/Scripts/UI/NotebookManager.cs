using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NotebookManager : MB {

	public NotebookCanvas map;
	public NotebookCanvas priorities;
	public NotebookCanvas data;
	public GameObject tabGroup;
	public GameObject notebookCollider;
	public DayCounter dayCounter;
	public CitiesManager citiesManager;

	public RectTransform namingPanel;
	public RectTransform feedbackPanel;

	public Text scoreText;
	public Text feedbackText;

	Dictionary<string, NotebookCanvas> canvases;
	Dictionary<string, NotebookCanvas> Canvases {
		get {
			if (canvases == null) {
				canvases = new Dictionary<string, NotebookCanvas> ();
				canvases.Add ("map", map);
				canvases.Add ("priorities", priorities);
				canvases.Add ("data", data);
			}
			return canvases;
		}
	}

	static NotebookManager instance = null;
	static public NotebookManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (NotebookManager)) as NotebookManager;
			}
			return instance;
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

	bool CanCloseNotebook {
		get {
			return (
				(open
				&& PlayerData.CityGroup.CurrentCity != "capitol" // temp
				&& state != State.MakingPlan)
				|| !isPhaseOne
			);
		}
	}

	public enum State {
		Traveling, MakingPlan
	}

	State state = State.Traveling;

	public bool MakingPlan {
		get { return state == State.MakingPlan; }
	}

	bool open = true;
	public bool IsOpen {
		get { return open; }
	}

	public bool openAtStart = true;
	public bool isPhaseOne = true;

	string activeCanvas = "map";

	void Start () {
		// Need to find a better way to approach this, but for now
		if (openAtStart) {
			open = false;
			Open ();
		} else {
			open = true;
			Close ();
		}
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
			Close ();
		} else if (NPCFocusBehavior.Instance.Unfocused) {
			Open ();
		}
	}

	public void Open () {
		if (open) return;
		UpdateState ();
		SetActiveCanvasOnOpen ();
		OpenCanvas (activeCanvas);
		tabGroup.SetActive (true);
		notebookCollider.SetActive (true);
		CameraPositioner.Drag.Enabled = false;
		open = true;
	}

	public void Close () {
		if (!CanCloseNotebook) return;
		foreach (var canvas in Canvases) {
			canvas.Value.Close ();
			canvas.Value.gameObject.SetActive (false);
		}
	
		// Hide tabs only in phase one; are always visible in two
		if(isPhaseOne)
			tabGroup.SetActive (false);
	
		notebookCollider.SetActive (false);
		CameraPositioner.Drag.Enabled = true;
		open = false;
	}

	// Tell data canvas to update indicators
	public void UpdateIndicators(int intBirths, int intVaccinations, int intQOC) {

		data.UpdateIndicators(intBirths, intVaccinations, intQOC);
		
	}

	public void NamePlan() {

		namingPanel.gameObject.SetActive(true);

	}


	public void SubmitPlan(Text planNameInput) {

		state =	State.MakingPlan;

        Dictionary<string, object> formFields = new Dictionary<string, object>();

        Models.Plan plan = new Models.Plan();

        plan.name = planNameInput.text;
        plan.tactics = PlayerData.TacticPriorityGroup.Tactics;

        formFields.Add("plan", plan);

		PlayerManager.Instance.SaveData (formFields, SubmitPlanCallback);

	}

	void OpenCanvas (string id) {
		foreach (var canvas in Canvases) {
			bool open = canvas.Key == id;
			canvas.Value.gameObject.SetActive (open);
			if (open) {
				canvas.Value.Open ();
			} else {
				canvas.Value.Close ();
			}
		}
		activeCanvas = id;
	}

	void SetActiveCanvasOnOpen () {
		if (state == State.MakingPlan)
			activeCanvas = "priorities";
	}

	void UpdateState () {
		if(dayCounter == null)
			return;

		// if (!dayCounter.HasDays && !InteractionsManager.Instance.HasInteractions) {
		if (PlayerData.DayGroup.Empty && PlayerData.InteractionGroup.Empty) {//!InteractionsManager.Instance.HasInteractions) {
			state = State.MakingPlan;
		}
	}

	// Continues to phase  two
	// Also, skips to phase two via "skip tab" button (won't be in test or final game)
	public void Continue() {

		Application.LoadLevel("PhaseTwo");

	}

	// Get response from submitting a plan
	void SubmitPlanCallback(Dictionary<string, object> response) {

	 	scoreText.text = "Score: " + response["score"].ToString();
	 	feedbackText.text = response["description"].ToString();

	 	// Show feedback in data panel (allows player to continue)
		feedbackPanel.gameObject.SetActive(true);
		OpenData();

		PlayerManager.Instance.TrackEvent("Plan Saved", "Phase One");

	}
}

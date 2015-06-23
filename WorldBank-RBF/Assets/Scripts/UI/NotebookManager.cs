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
	public DayCounter dayCounter;
	public CitiesManager citiesManager;

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
				open
				&& citiesManager.CurrentCitySymbol != "capitol"
				&& state != State.MakingPlan
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

	public bool openAtStart = true;
	bool open = true;
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
		} else if (NPCFocusBehavior.Instance.FocusLevel == FocusLevel.Default) {
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
			canvas.Value.SetActive (false);
		}
		tabGroup.SetActive (false);
		notebookCollider.SetActive (false);
		CameraPositioner.Drag.Enabled = true;
		open = false;
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

	void OpenCanvas (string id) {
		foreach (var canvas in Canvases) {
			canvas.Value.SetActive (canvas.Key == id);
		}
		activeCanvas = id;
	}

	void SetActiveCanvasOnOpen () {
		if (state == State.MakingPlan)
			activeCanvas = "priorities";
	}

	void UpdateState () {
		if (!dayCounter.HasDays && !InteractionsManager.Instance.HasInteractions) {
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

	}
}

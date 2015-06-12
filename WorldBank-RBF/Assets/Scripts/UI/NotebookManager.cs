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

	static NotebookManager instance = null;
	static public NotebookManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (NotebookManager)) as NotebookManager;
				if (instance == null) {
					GameObject go = new GameObject ("NotebookManager");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<NotebookManager>();
				}
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

	bool startOpen = true;
	bool open = true;
	string activeCanvas = "map";

	void Awake () {
		if (startOpen) {
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
		OpenCanvas (activeCanvas);
		tabGroup.SetActive (true);
		notebookCollider.SetActive (true);
		CameraPositioner.Drag.Enabled = false;
		open = true;
	}

	public void Close () {
		if (!open || CitiesManager.Instance.CurrentCitySymbol == "capitol") return;
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

	// Get response from submitting a plan
	void SubmitPlanCallback(Dictionary<string, object> response) {

	 	scoreText.text = "Score: " + response["score"].ToString();
	 	feedbackText.text = response["description"].ToString();

	 	// Show feedback in data panel (allows player to continue)
		feedbackPanel.gameObject.SetActive(true);
		OpenData();

	}
}

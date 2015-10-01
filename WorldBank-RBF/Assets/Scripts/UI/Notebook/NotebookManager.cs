using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NotebookManager : MB {

	/// <summary>
	/// Called any time indicators are updated.
	/// </summary>
	public OnUpdate onUpdate;
	
	public delegate void OnUpdate ();	

	public NotebookCanvas map;
	public NotebookCanvas priorities;
	public NotebookCanvas data;
	public NotebookCanvas indicators;

	public GameObject tabGroup;Dictionary<string, NotebookCanvas> canvases;
	Dictionary<string, NotebookCanvas> Canvases {
		get {
			if (canvases == null) {
				canvases = new Dictionary<string, NotebookCanvas> ();
				canvases.Add ("map", map);
				canvases.Add ("priorities", priorities);
				canvases.Add ("data", data);
				canvases.Add ("indicators", indicators);
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
			string currentCity = PlayerData.CityGroup.CurrentCity;
			return (
				(open
				&& currentCity == DataManager.SceneContext
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

	// string activeCanvas = "map";

	void Start () {
		// Need to find a better way to approach this, but for now
		if (openAtStart) {
			
			// open = false;
			// Open ();

			// Hide indicators tab and show interactions counter in phase one
			/*if(isPhaseOne)
				tabGroup.transform.GetChild(3).gameObject.SetActive(false);
			else*/
			if (!isPhaseOne)
				GameObject.Find("PhaseOne/UI/InteractionsCounter").gameObject.SetActive(false);

		} else {
			// open = true;
			// Close ();
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

	public void OpenIndicators () {
		OpenCanvas ("indicators");
	}

	public void ToggleNotebook () {
		if (open) {
			Close ();
		} else if (NPCFocusBehavior.Instance.Unfocused) {
			Open ();
		}
	}

	public void ToggleTabs () {
		tabGroup.SetActive (!tabGroup.activeSelf);
	}
	
	public void Open () {
		if (open) return;
		UpdateState ();
		/*SetActiveCanvasOnOpen ();
		OpenCanvas (activeCanvas);
		tabGroup.SetActive (true);
		notebookCollider.SetActive (true);
		CameraPositioner.Drag.Enabled = false;
		open = true;*/
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
	
		CameraPositioner.Drag.Enabled = true;
		open = false;
	}

	// Tell data canvas to update indicators
	public void UpdateIndicators(int intBirths, int intVaccinations, int intQOC, bool notify=true) {

		indicators.UpdateIndicators(intBirths, intVaccinations, intQOC);

		if(notify)
			SendUpdateMessage();
		
	}

	protected void SendUpdateMessage () {
		if (onUpdate != null) onUpdate ();
	}

	void OpenCanvas (string id) {
		
		foreach (var canvas in Canvases) {
		
			if(canvas.Equals(null) || canvas.Value == null)
				continue;

			bool open = canvas.Key == id;
			canvas.Value.gameObject.SetActive (open);

			if (open) {
				canvas.Value.Open ();
			} else {
				canvas.Value.Close ();
			}

		}

		// activeCanvas = id;
	}

	void SetActiveCanvasOnOpen () {
		/*if (state == State.MakingPlan)
			activeCanvas = "priorities";*/
	}

	void UpdateState () {

		if (PlayerData.DayGroup.Empty && PlayerData.InteractionGroup.Empty) {
			state = State.MakingPlan;
		}
	}
}

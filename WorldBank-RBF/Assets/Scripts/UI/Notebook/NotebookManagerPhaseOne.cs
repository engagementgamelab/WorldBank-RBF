using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotebookManagerPhaseOne : MonoBehaviour {


	static NotebookManagerPhaseOne instance = null;
	static public NotebookManagerPhaseOne Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (NotebookManagerPhaseOne)) as NotebookManagerPhaseOne;
			}
			return instance;
		}
	}

	bool isOpen = false;
	public bool IsOpen { 
		get { return isOpen; }
		set {
			isOpen = value;
			MainCamera.Instance.Positioner.Drag.Enabled = !isOpen;
		}
	}

	public bool CanCloseNotebook {
		get {
			string currentCity = PlayerData.CityGroup.CurrentCity;
			return (
				(currentCity == DataManager.SceneContext
				&& !NotebookManager.Instance.MakingPlan
				&& !PlayerData.InteractionGroup.Empty)
			);
		}
	}

	public List<CanvasToggle> toggles;

	void Awake () {
		Events.instance.AddListener<ArriveInCityEvent> (OnArriveInCityEvent);

		Events.instance.AddListener<TutorialEvent>(OnTutorialEvent);
	}

	public void CloseCanvases () {
		foreach (CanvasToggle toggle in toggles) {
			toggle.Close ();
		}
	}

	void OnArriveInCityEvent (ArriveInCityEvent e) {
		CloseCanvases ();
	}

    /// <summary>
    // Callback for TutorialEvent, filtering for type of event
    /// </summary>
    void OnTutorialEvent(TutorialEvent e) {

    	switch(e.eventType) {

    		case "skip_tutorial":

	    		DataManager.tutorialEnabled = false;

	    		DialogManager.instance.RemoveTutorialScreen();
	    		
	    		// Reset interactions
				PlayerData.InteractionGroup.ClearTutorial();

				// Open map
				toggles[1].OnClick();
				
    			break;

    		default:

    			DialogManager.instance.CreateTutorialScreen(e.eventType);	    		
    			break;

    	}
    }

}

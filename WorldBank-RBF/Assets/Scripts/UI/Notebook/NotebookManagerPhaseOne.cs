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

	void OnEnable () {
		Events.instance.AddListener<ArriveInCityEvent> (OnArriveInCityEvent);
		Events.instance.AddListener<TutorialEvent>(OnTutorialEvent);
	}

	void OnDisable () {
		Events.instance.RemoveListener<ArriveInCityEvent> (OnArriveInCityEvent);
		Events.instance.RemoveListener<TutorialEvent>(OnTutorialEvent);
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

    		// Unlock tutorial tactics
			PlayerData.UnlockItem ("unlockable_grant_providers_autonomy", DataManager.GetUITextByKey("copy_unlockable_grant_providers_autonomy"), "finance_minister_capitol_city");
			PlayerData.UnlockItem ("unlockable_incentivise_providers_to_deliver_services", DataManager.GetUITextByKey("copy_unlockable_incentivise_providers_to_deliver_services"), "dep_minister_of_health_capitol_city");
			PlayerData.UnlockItem ("unlockable_vouchers_for_services", DataManager.GetUITextByKey("copy_unlockable_vouchers_for_services"), "regional_director_of_health_services_capitol_city");
			PlayerData.UnlockItem ("unlockable_information_campaign_to_explain_changes_to_system", DataManager.GetUITextByKey("copy_unlockable_information_campaign_to_explain_changes_to_system"), "regional_director_of_health_services_capitol_city");
    		
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

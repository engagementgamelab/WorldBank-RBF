using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugMenu : MonoBehaviour {

	bool showOptions = false;
	bool capitolVisited = false;

	#if DEVELOPMENT_BUILD || UNITY_EDITOR
	void Start () {
		PlayerData.CityGroup.onUpdateCurrentCity += OnUpdateCurrentCity;
	}

	void OnUpdateCurrentCity (string symbol) {
		if (symbol == "capitol") capitolVisited = true;
	}

	void OnGUI () {
		GUI.color = Color.black;
		showOptions = GUILayout.Toggle (showOptions, "Show debug options");
		if (!showOptions) return;
		GUI.color = Color.white;
		if (!capitolVisited && GUILayout.Button ("visit capitol")) {
			PlayerData.CityGroup.CurrentCity = "capitol";
		}
		if (GUILayout.Button ("0 interactions")) {
			PlayerData.InteractionGroup.Clear ();
		}
		if (GUILayout.Button ("unlock routes")) {
			List<RouteItem> routes = PlayerData.RouteGroup.Routes;
			foreach (RouteItem route in routes) {
				route.Unlocked = true;
			}
		}
		if (GUILayout.Button ("unlock tactics")) {
			PlayerData.UnlockItem ("unlockable_vouchers_for_services", "This would provide vouchers to poor people so they can receive necessary services at a greatly reduces price.");
			PlayerData.UnlockItem ("unlockable_information_campaign_to_explain_changes_to_system", "If major changes are going to take place in the health system, and educational information campaign to explain these changes may be necessary to avoid confusion.");
			PlayerData.UnlockItem ("unlockable_incentivise_providers_to_deliver_services", "Context text");
    		PlayerData.UnlockItem ("unlockable_incentivise_providers_to_follow_protocols", "Context text");
    		PlayerData.UnlockItem ("unlockable_improve_patient_and_provider_relationship", "Context text");
    		PlayerData.UnlockItem ("unlockable_make_aesthetic_improvements", "Context text");
    		// PlayerData.UnlockItem ("unlockable_information_campaign_to_change_cultural_customs_and_behavior", "Context text");
    		// PlayerData.UnlockItem ("unlockable_dialogue_mr_todd", "Context text");
    	}
    	if (GUILayout.Button ("skip to phase 2")) {
    		ObjectPool.Clear();
			Application.LoadLevel("PhaseTwo");		
    	}
	}
	#endif
}

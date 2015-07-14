using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugMenu : MonoBehaviour {

	bool showOptions = false;

	#if DEVELOPMENT_BUILD || UNITY_EDITOR
	void OnGUI () {
		GUI.color = Color.black;
		showOptions = GUILayout.Toggle (showOptions, "Show debug options");
		if (!showOptions) return;
		GUI.color = Color.white;
		if (GUILayout.Button ("0 interactions")) {
			PlayerData.InteractionGroup.Clear ();
		}
		List<RouteItem> routes = PlayerData.RouteGroup.Routes;
		foreach (RouteItem route in routes) {
			if (route.Unlocked) continue;
			if (GUILayout.Button ("unlock " + route.Terminals.city1 + " to " + route.Terminals.city2)) {
				route.Unlocked = true;
			}
		}
		if (GUILayout.Button ("unlock tactics")) {
    		PlayerData.UnlockImplementation ("unlockable_incentivise_providers_to_follow_protocols");
    		PlayerData.UnlockImplementation ("unlockable_improve_patient_and_provider_relationship");
    		PlayerData.UnlockImplementation ("unlockable_make_aesthetic_improvements");
    		PlayerData.UnlockImplementation ("unlockable_information_campaign_to_change_cultural_customs_and_behavior");
    	}
	}
	#endif
}

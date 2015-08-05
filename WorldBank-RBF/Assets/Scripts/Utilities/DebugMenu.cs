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
			PlayerData.UnlockItem ("unlockable_vouchers_for_services", "This would provide vouchers to poor people so they can receive necessary services at a greatly reduces price.");
			PlayerData.UnlockItem ("unlockable_information_campaign_to_explain_changes_to_system", "If major changes are going to take place in the health system, and educational information campaign to explain these changes may be necessary to avoid confusion.");
			PlayerData.UnlockItem ("unlockable_incentivise_providers_to_deliver_services", "Context text");

    		PlayerData.UnlockItem ("unlockable_incentivise_providers_to_follow_protocols", "Context text");
    		PlayerData.UnlockItem ("unlockable_improve_patient_and_provider_relationship", "Context text");
    		PlayerData.UnlockItem ("unlockable_make_aesthetic_improvements", "Context text");
    		PlayerData.UnlockItem ("unlockable_information_campaign_to_change_cultural_customs_and_behavior", "Context text");
    		PlayerData.UnlockItem ("unlockable_dialogue_mr_todd", "Context text");
    	}
	}
	#endif
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugMenu : MonoBehaviour {

	bool showOptions = false;
	bool capitolVisited = false;
	bool twoTacticsUnlocked = false;

	#if DEVELOPMENT_BUILD || UNITY_EDITOR
	void Start () {
		PlayerData.CityGroup.onUpdateCurrentCity += OnUpdateCurrentCity;
	}

	void OnUpdateCurrentCity (string symbol) {
		if (symbol == "capitol") capitolVisited = true;
	}

	void AutoResize(int screenWidth, int screenHeight)
	{
	}

	void OnGUI () {
		GUI.color = Color.black;
		GUILayout.Space (16);

    Vector2 resizeRatio = new Vector2(1, 1);

    #if UNITY_IPHONE || UNITY_ANDROID
    	resizeRatio = new Vector2(3, 3);
    #endif

    GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f));

		showOptions = GUILayout.Toggle (showOptions, "Debug options");
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
		if (GUILayout.Button ("unlock tutorial tactics")) {
			PlayerData.UnlockItem ("unlockable_grant_providers_autonomy", "no context because you used the freakin debug button");
			PlayerData.UnlockItem ("unlockable_incentivise_providers_to_deliver_services", "no context because yer a silly debuggerbob");
			PlayerData.UnlockItem ("unlockable_vouchers_for_services", "no context because you are boop doop debugging yore goop");
			PlayerData.UnlockItem ("unlockable_information_campaign_to_explain_changes_to_system", "NO FMUPPING CONTEXT because blug blug dree buggabot");
		}
		if (!twoTacticsUnlocked && GUILayout.Button ("unlock 2 tactics")) {
    		PlayerData.UnlockItem ("unlockable_incentivise_providers_to_follow_protocols", "Context text");
    		PlayerData.UnlockItem ("unlockable_make_aesthetic_improvements", "plz plz you can't get context if you debug the game like that :(");
    		twoTacticsUnlocked = true;
    }
  	if (GUILayout.Button ("1 day")) {
  		PlayerData.DayGroup.Set (1);
  	}
	}
	#endif
}

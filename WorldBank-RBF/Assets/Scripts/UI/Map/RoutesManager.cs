using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoutesManager : MB {

	List<MapRoute> mapRoutes;
	List<MapRoute> MapRoutes {
		get {
			if (mapRoutes == null) {
				mapRoutes = new List<MapRoute> ();
				foreach (Transform child in Transform) {
					mapRoutes.Add (child.GetScript<MapRoute> ());
				}
			}
			return mapRoutes;
		}
	}

	void Start () {
		InitRoutes ();
	}

	void InitRoutes () {
		List<RouteItem> routes = PlayerData.RouteGroup.Routes;
		foreach (MapRoute route in MapRoutes) {
			route.RouteItem = routes.Find (x => x.Terminals == route.Terminals);
		}
	}

	#if DEBUG
	void OnGUI () {
		GUILayout.Space (40);
		List<RouteItem> routes = PlayerData.RouteGroup.Routes;
		foreach (RouteItem route in routes) {
			if (route.Unlocked) continue;
			if (GUILayout.Button ("unlock " + route.Terminals.city1 + " to " + route.Terminals.city2)) {
				route.Unlocked = true;
			}
		}
	}
	#endif
}
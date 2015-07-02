using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoutesManager : MB {

	/*struct Terminals {
		
		public string city1;
		public string city2;

		public Terminals (string city1, string city2) {
			this.city1 = city1;
			this.city2 = city2;
		}
	}*/

	/*Dictionary<Terminals, MapRoute> routes;
	Dictionary<Terminals, MapRoute> Routes {
		get {
			if (routes == null) {
				routes = new Dictionary<Terminals, MapRoute> ();
				foreach (Transform child in Transform) {
					MapRoute route = child.GetScript<MapRoute> ();
					Terminals terminals = new Terminals (route.Cities[0], route.Cities[1]);
					routes.Add (terminals, route);
				}
			}
			return routes;
		}
	}*/

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

	bool unlockAll = false; // use for debugging

	void Start () {
		// UpdateRoutes (DataManager.GetAllRoutes ());
		List<Terminals> unlockedTerminals = PlayerData.RouteGroup.UnlockedTerminals;
		foreach (Terminals t in unlockedTerminals) {
			// Debug.Log (t.city1 + ", " + t.city2);
		}
		foreach (MapRoute route in MapRoutes) {
			// Debug.Log (route.Terminals.city1 + ", " + route.Terminals.city2);
			route.Unlocked = unlockedTerminals.Contains (route.Terminals);
		}
	}

	/*public void UpdateRoutes () {
		UpdateRoutes (PlayerData.RouteGroup.Routes);
	}

	void UpdateRoutes (Models.Route[] routes) {
		if (unlockAll) {
			for (int i = 0; i < routes.Length; i ++) {
				Models.Route route = routes[i];
				MapRoute mapRoute = FindRoute (route.city1, route.city2);
				mapRoute.Cost = route.cost;
				mapRoute.Unlocked = true;
			}
		} else {
			for (int i = 0; i < routes.Length; i ++) {
				Models.Route route = routes[i];
				MapRoute mapRoute = FindRoute (route.city1, route.city2);
				mapRoute.Cost = route.cost;
				mapRoute.Unlocked = route.unlocked;
			}
		}
	}*/

	/*public MapRoute FindRoute (string city1, string city2) {
		MapRoute route;
		Terminals terminals = new Terminals (city1, city2);
		if (Routes.TryGetValue (terminals, out route)) {
			return route;
		}
		terminals = new Terminals (city2, city1);
		if (Routes.TryGetValue (terminals, out route)) {
			return route;
		}
		throw new System.Exception ("Route between '" + city1 + "' and '" + city2 + "' does not exist");
	}

	public bool RouteExists (string city1, string city2) {
		MapRoute route;
		Terminals terminals = new Terminals (city1, city2);
		if (Routes.TryGetValue (terminals, out route)) {
			return route.Unlocked;
		}
		terminals = new Terminals (city2, city1);
		if (Routes.TryGetValue (terminals, out route)) {
			return route.Unlocked;
		}
		return false;
	}*/

	#if DEBUG
	void OnGUI () {
		/*Models.Route[] playerRoutes = PlayerData.RouteGroup.Routes;
		GUILayout.Space (40);
		foreach (Models.Route route in playerRoutes) {
			if (route.unlocked) continue;
			if (GUILayout.Button ("unlock " + route.city1 + " to " + route.city2)) {
				route.unlocked = true;
				NotebookManager.Instance.OpenMap ();
			}
		}*/
	}
	#endif
}
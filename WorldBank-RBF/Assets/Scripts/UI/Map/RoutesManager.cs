using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Keeps track of all the routes in the map screen.
/// </summary>
public class RoutesManager : MB {

	List<MapRoute> mapRoutes;

	/// <summary>
	/// Gets a list of MapRoutes in the game.
	/// </summary>
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

	/// <summary>
	/// Assigns the appropriate RouteItem to each route in the game.
	/// </summary>
	void InitRoutes () {
		List<RouteItem> routes = PlayerData.RouteGroup.Routes;
		foreach (MapRoute route in MapRoutes) {
			route.RouteItem = routes.Find (x => x.Terminals == route.Terminals);
		}
	}
}
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sets up all the routes in the map screen.
/// </summary>
public class RoutesManager : MB {

	/// <summary>
	/// This is a singleton.
	/// </summary>
	static RoutesManager instance = null;
	static public RoutesManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (RoutesManager)) as RoutesManager;
				if (instance == null) {
					GameObject go = new GameObject ("RoutesManager");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<RoutesManager>();
				}
			}
			return instance;
		}
	}

	List<MapRoute> mapRoutes;

	/// <summary>
	/// Gets a list of MapRoutes in the game.
	/// </summary>
	public List<MapRoute> MapRoutes {
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

	/// <summary>
	/// Gets a route with the given terminals.
	/// </summary>
	public MapRoute GetRoute (Terminals terminals) {
		return MapRoutes.Find (x => x.Terminals == terminals);
	}
}
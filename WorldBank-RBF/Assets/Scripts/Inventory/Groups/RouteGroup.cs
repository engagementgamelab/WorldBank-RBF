using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RouteGroup : ModelGroup<RouteItem> {

	public override string Name { get { return "Routes"; } }

	public List<Terminals> UnlockedTerminals {
		get { return UnlockedItems.ConvertAll (x => x.Terminals); }
	}

	Models.Route[] routeModels = null;
	public Models.Route[] RouteModels {
		get {
			if (routeModels == null) {
				routeModels = DataManager.GetAllRoutes ();
			}
			return routeModels;
		}
	}

	public RouteGroup () : base ("route") {}

	public Models.Route[] Routes {
		// get { return Items.ConvertAll (x => ((RouteItem)x).route).ToArray (); }
		get { return null; }
	}

	/*public Models.Route Unlock (string symbol) {
		Models.Route route = Array.Find (Routes, x => x.symbol == symbol);
		if (route != null) route.unlocked = true;
		return route;
	}

	public void Lock (string symbol) {
		Models.Route route = Array.Find (Routes, x => x.symbol == symbol);
		if (route != null) route.unlocked = false;
	}*/
}

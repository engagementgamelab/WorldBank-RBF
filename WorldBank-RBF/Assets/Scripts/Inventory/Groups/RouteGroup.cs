using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RouteGroup : ItemGroup<RouteItem> {

	public override string Name { get { return "Routes"; } }

	public Models.Route[] Routes {
		get { return Items.ConvertAll (x => ((RouteItem)x).route).ToArray (); }
	}

	public Models.Route Unlock (string symbol) {
		Models.Route route = Array.Find (Routes, x => x.symbol == symbol);
		if (route != null) route.unlocked = true;
		return route;
	}
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains all the routes in the game.
/// </summary>
public class RouteGroup : ModelGroup<RouteItem> {

	public override string ID { get { return "routes"; } }

	/// <summary>
	/// Gets a list of RouteItems.
	/// </summary>
	public List<RouteItem> Routes {
		get { return Items.ConvertAll (x => (RouteItem)x); }
	}

	Models.Route[] routeModels = null;

	/// <summary>
	/// Gets an array of data models for each route.
	/// </summary>
	public Models.Route[] RouteModels {
		get {
			if (routeModels == null) {
				routeModels = DataManager.GetAllRoutes ();
			}
			return routeModels;
		}
	}

	public RouteGroup () : base ("route") {}
}

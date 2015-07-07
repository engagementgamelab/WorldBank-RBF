using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RouteGroup : ModelGroup<RouteItem> {

	public override string Name { get { return "Routes"; } }

	public List<RouteItem> Routes {
		get { return Items.ConvertAll (x => (RouteItem)x); }
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

}

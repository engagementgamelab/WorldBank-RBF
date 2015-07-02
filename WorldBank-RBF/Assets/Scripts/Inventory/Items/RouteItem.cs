using UnityEngine;
using System;
using System.Collections;

public class RouteItem : ModelItem {

	public override string Name { get { return "Route"; } }

	Terminals terminals;
	public Terminals Terminals {
		get { return terminals; }
	}

	Models.Route routeModel;

	public override void OnSetModel () {
		// TODO: left off here
		// routeModel = Array.Find (((RouteGroup)Group).RouteModels, x => x.symbol == Symbol.Substring (17));
		// this.terminals = new Terminals (routeModel.city1, routeModel.city2);
		// Debug.Log (terminals.city1 + ", " + terminals.city2);
	}

	// public readonly Models.Route route;

	/*public RouteItem () {}
	public RouteItem (Models.Unlockable unlockable) {
		// route = DataManager.GetRouteInfo (unlockable.unlocked[0]);
	}*/

	/*public RouteItem (Models.Route route) {
		this.route = route;
	}*/
}

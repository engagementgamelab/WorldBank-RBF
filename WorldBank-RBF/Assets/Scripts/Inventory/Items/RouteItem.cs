using UnityEngine;
using System.Collections;

public class RouteItem : InventoryItem {

	public override string Name { get { return "Route"; } }

	public readonly Models.Route route;

	public RouteItem () {}
	public RouteItem (Models.Unlockable unlockable) {
		route = DataManager.GetRouteInfo (unlockable.unlocked[0]);
	}

	public RouteItem (Models.Route route) {
		this.route = route;
	}
}

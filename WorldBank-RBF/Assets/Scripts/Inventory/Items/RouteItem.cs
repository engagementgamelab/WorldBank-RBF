using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// A route based on a data model.
/// </summary>
public class RouteItem : ModelItem {

	public override string Name { get { return "Route"; } }

	/// <summary>
	/// Gets the Terminals (the two cities that this route connects).
	/// </summary>
	public Terminals Terminals { get; private set; }

	/// <summary>
	/// Gets the cost to travel along the route.
	/// </summary>
	public int Cost { get; private set; }

	RouteGroup routeGroup;
	Models.Route routeModel;

	/// <summary>
	/// On initialization, sets the terminals and cost based on the given data model.
	/// </summary>
	public override void OnInit () {

		routeGroup = Group as RouteGroup;
		
		if (routeGroup != null) {
			routeModel = Array.Find (routeGroup.RouteModels, x => x.symbol == Symbol.Substring (17));
			Terminals = new Terminals (routeModel.city1, routeModel.city2);
			Cost = routeModel.cost;
		}
	}
}

using UnityEngine;
using System;
using System.Collections;

public class RouteItem : ModelItem {

	public override string Name { get { return "Route"; } }

	public Terminals Terminals { get; private set; }
	public int Cost { get; private set; }

	RouteGroup routeGroup;
	Models.Route routeModel;

	public override void OnInit () {

		routeGroup = Group as RouteGroup;
		
		if (routeGroup != null) {
			routeModel = Array.Find (routeGroup.RouteModels, x => x.symbol == Symbol.Substring (17));
			Terminals = new Terminals (routeModel.city1, routeModel.city2);
			Cost = routeModel.cost;
		}
	}
}

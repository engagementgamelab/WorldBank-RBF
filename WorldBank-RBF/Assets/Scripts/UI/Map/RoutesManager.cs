using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoutesManager : MB {

	struct Terminals {
		
		public string city1;
		public string city2;

		public Terminals (string city1, string city2) {
			this.city1 = city1;
			this.city2 = city2;
		}
	}

	Dictionary<Terminals, MapRoute> routes;
	Dictionary<Terminals, MapRoute> Routes {
		get {
			if (routes == null) {
				routes = new Dictionary<Terminals, MapRoute> ();
				foreach (Transform child in Transform) {
					MapRoute route = child.GetScript<MapRoute> ();
					Terminals terminals = new Terminals (route.Cities[0], route.Cities[1]);
					routes.Add (terminals, route);
				}
			}
			return routes;
		}
	}

	public MapRoute FindRoute (string city1, string city2) {
		MapRoute route;
		Terminals terminals = new Terminals (city1, city2);
		if (Routes.TryGetValue (terminals, out route)) {
			return route;
		}
		terminals = new Terminals (city2, city1);
		if (Routes.TryGetValue (terminals, out route)) {
			return route;
		}
		throw new System.Exception ("Route between '" + city1 + "' and '" + city2 + "' does not exist");
	}

	public bool RouteExists (string city1, string city2) {
		MapRoute route;
		Terminals terminals = new Terminals (city1, city2);
		if (Routes.TryGetValue (terminals, out route)) {
			return true;
		}
		terminals = new Terminals (city2, city1);
		if (Routes.TryGetValue (terminals, out route)) {
			return true;
		}
		return false;
	}
}

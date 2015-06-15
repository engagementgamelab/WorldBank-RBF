using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RouteGroup : ItemGroup<RouteItem> {

	public override string Name { get { return "Routes"; } }

	public Models.Route[] Routes {
		get { return Items.ConvertAll (x => ((RouteItem)x).route).ToArray (); }
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapManager2 : NotebookCanvas {

	Canvas canvas = null;
	Canvas Canvas {
		get {
			if (canvas == null) {
				canvas = GetComponent<Canvas> ();
			}
			return canvas;
		}
	}

	public CitiesManager citiesManager;
	public RoutesManager routesManager;

	public override void Open () {
		Canvas.enabled = true;
		UpdateMap ();
	}

	public override void Close () {
		Canvas.enabled = false;
	}

	public void UpdateMap () {
		routesManager.UpdateRoutes ();
		citiesManager.UpdateCities ();
	}
}

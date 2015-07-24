using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// A representation of the route on the map.
/// </summary>
public class MapRoute : MB {

	Text text = null;
	Text Text {
		get {
			if (text == null) {
				text = Transform.GetChild (1).GetComponent<Text> ();
			}
			return text;
		}
	}

	Transform line = null;
	Transform Line {
		get {
			if (line == null) {
				line = Transform.GetChild (0);
			}
			return line;
		}
	}

	/// <summary>
	/// Sets the unlocked state of the route. Hides the line and text if the route is not unlocked.
	/// </summary>
	bool Unlocked {
		set {
			Line.gameObject.SetActive (value);
			Text.gameObject.SetActive (value);
		}
	}

	/// <summary>
	/// Gets the two cities that the route connects.
	/// </summary>
	public Terminals Terminals {
		get { return new Terminals (city1, city2); }
	}

	/// <summary>
	/// Sets the text that represents the cost to travel along the route.
	/// </summary>
	int Cost {
		set { Text.text = value.ToString (); }
	}

	RouteItem routeItem = null;

	/// <summary>
	/// Gets/sets the RouteItem associated with this MapRoute. Setting also updates the cost and 
	/// unlocked state. This can only be set once.
	/// </summary>
	public RouteItem RouteItem { 
		get { return routeItem; }
		set {
			if(value == null)
				throw new System.Exception("Route item is null!");
				
			if (routeItem == null) {
				routeItem = value;
				routeItem.onUpdateUnlocked += OnUpdateUnlocked;
				Unlocked = routeItem.Unlocked;
				Cost = routeItem.Cost;
			}
		}
	}
	
	public string city1;
	public string city2;

	public void OnUpdateUnlocked () {
		Unlocked = routeItem.Unlocked;
	}
}

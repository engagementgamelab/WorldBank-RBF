using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

	bool unlocked = false;
	public bool Unlocked {
		get { return unlocked; }
		set {
			unlocked = value;
			Line.gameObject.SetActive (unlocked);
			Text.gameObject.SetActive (unlocked);
		}
	}

	Terminals terminals;
	public Terminals Terminals {
		get { return new Terminals (city1, city2); }
	}

	int cost = 1;
	public int Cost {
		get { return cost; }
		set { 
			cost = value;
			Text.text = cost.ToString ();			
		}
	}

	RouteItem routeItem;
	public RouteItem RouteItem { 
		get { return routeItem; }
		set {
			routeItem = value;
			routeItem.onUpdateUnlocked += OnUpdateUnlocked;
			Unlocked = routeItem.Unlocked;
			Cost = routeItem.Cost;
		}
	}
	
	public string city1;
	public string city2;

	public void OnUpdateUnlocked () {
		Unlocked = routeItem.Unlocked;
	}
}

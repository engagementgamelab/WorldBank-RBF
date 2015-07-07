using UnityEngine;
using System.Collections;

public class CityItem : InventoryItem {
	
	public override string Name { get { return Model.display_name; } }

	public string Symbol { get { return Model.symbol; } }
	public Models.City Model { get; private set; }

	// bool visited = false;
	public bool Visited { get; set; }

	// bool stayedExtraDay = false;
	public bool StayedExtraDay { get; set; }

	public CityItem () {}
	public CityItem (Models.City model) {
		this.Model = model;

		// Special case: capitol can't be entered
		if (Model.symbol == "capitol") {
			Visited = true;
			StayedExtraDay = true;
		}
	}
}

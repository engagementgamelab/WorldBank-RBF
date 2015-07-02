using UnityEngine;
using System.Collections;

public class CityItem : InventoryItem {
	
	public override string Name { get { return model.display_name; } }

	Models.City model;

	public CityItem () {}
	public CityItem (Models.City model) {
		this.model = model;
	}
}

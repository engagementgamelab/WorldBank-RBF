using UnityEngine;
using System.Collections;

public class CityItem : InventoryItem {
	
	string cityName;
	public override string Name { get { return cityName; } }

	public CityItem () {}
	public CityItem (string cityName) {
		this.cityName = cityName;
	}
}

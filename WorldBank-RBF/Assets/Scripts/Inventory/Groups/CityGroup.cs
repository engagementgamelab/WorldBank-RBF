using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityGroup : ItemGroup<CityItem> {
		
	public delegate void OnUpdateCurrentCity (string symbol);

	public override string Name { get { return "Cities"; } }

	string currentCity = "capitol";
	public string CurrentCity {
		get { return currentCity; }
		set {
			currentCity = value;
			if (onUpdateCurrentCity != null)
				onUpdateCurrentCity (currentCity);
		}
	}

	public List<CityItem> Cities {
		get { return Items.ConvertAll (x => (CityItem)x); }
	}

	public OnUpdateCurrentCity onUpdateCurrentCity;

	public CityGroup () {
		Models.City[] cities = DataManager.GetAllCities ();
		foreach (Models.City city in cities) {
			if (city.enabled)
				Add (new CityItem (city));
		}
	}
}

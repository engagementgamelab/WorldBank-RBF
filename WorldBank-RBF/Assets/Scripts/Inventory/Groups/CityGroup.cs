using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Contains all the cities in the game.
/// </summary>
public class CityGroup : ItemGroup<CityItem> {
		
	public delegate void OnUpdateCurrentCity (string symbol);

	public override string ID { get { return "cities"; } }

	// The game begins in Capitol City.
	string currentCity = "capitol";

	/// <summary>
	/// Gets/sets the city that the player is currently visiting.
	/// Setting the current city also triggers the onUpdateCurrentCity callback.
	/// </summary>
	public string CurrentCity {
		get { return currentCity; }
		set {
			currentCity = value;
			if (onUpdateCurrentCity != null)
				onUpdateCurrentCity (currentCity);
		}
	}

	/// <summary>
	/// Gets a list of CityItems.
	/// </summary>
	public List<CityItem> Cities {
		get { return Items.ConvertAll (x => (CityItem)x); }
	}

	/// <summary>
	/// Called whenever the CurrentCity changes.
	/// </summary>
	public OnUpdateCurrentCity onUpdateCurrentCity;

	/// <summary>
	/// Populates this group with all the cities defined in the game data.
	/// </summary>
	public CityGroup () {
		Models.City[] cities = DataManager.GetAllCities ();
		foreach (Models.City city in cities) {
			if (city.enabled)
				Add (new CityItem (city));
		}
	}
}

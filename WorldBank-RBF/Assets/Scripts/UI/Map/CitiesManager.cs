using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CitiesManager : MB {

	Dictionary<string, CityButton> cities;
	Dictionary<string, CityButton> Cities {
		get {
			if (cities == null) {
				cities = new Dictionary<string, CityButton> ();
				foreach (Transform child in Transform) {
					CityButton button = child.GetScript<CityButton> ();
					string symbol = button.symbol;
					cities.Add (symbol, button);
				}
			}
			return cities;
		}
	}

	public CityInfoBox cityInfoBox;

	void Start () {
		Models.City[] models = DataManager.GetAllCities ();
		for (int i = 0; i < models.Length; i ++) {
			Models.City model = models[i];
			Cities[model.symbol].Unlocked = model.unlocked;
		}
	}

	public void OnClickCity (CityButton button) {
		cityInfoBox.Open (button.symbol);
	}
}

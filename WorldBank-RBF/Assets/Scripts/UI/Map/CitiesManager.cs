using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CitiesManager : MB {

	static CitiesManager instance = null;
	static public CitiesManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (CitiesManager)) as CitiesManager;
				if (instance == null) {
					GameObject go = new GameObject ("CitiesManager");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<CitiesManager>();
				}
			}
			return instance;
		}
	}

	Dictionary<string, CityButton> cities;
	Dictionary<string, CityButton> Cities {
		get {
			if (cities == null) {
				cities = new Dictionary<string, CityButton> ();
				foreach (Transform child in Transform) {
					CityButton button = child.GetScript<CityButton> ();
					if (button != null) {
						string symbol = button.symbol;
						cities.Add (symbol, button);
					}
				}
			}
			return cities;
		}
	}

	CityButton CurrentCity {
		get { return Cities[currentCitySymbol]; }
	}

	string currentCitySymbol = "capitol";

	public CityInfoBox cityInfoBox;
	public RoutesManager routesManager;
	public DayCounter dayCounter;
	public CurrentCityIndicator currentCityIndicator;

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

	public bool RequestVisitCity (string symbol) {
		if (currentCityIndicator.Moving) return false;
		MapRoute route = routesManager.FindRoute (currentCitySymbol, symbol);
		if (dayCounter.RemoveDays (route.Cost)) {
			Debug.Log ("going to " + symbol);
			Cities[currentCitySymbol].CurrentCity = false;
			currentCitySymbol = symbol;
			Cities[currentCitySymbol].CurrentCity = true;
			UpdateInteractableCities ();
			MoveIndicator ();
			return true;
		} else {
			Debug.Log ("MOTHERFUNKER!!! out of DAYS!!!!!");
			return false;
		}
	}

	void UpdateInteractableCities () {
		foreach (var city in Cities) {
			city.Value.Interactable = 
				city.Value.Unlocked && routesManager.RouteExists (city.Key, currentCitySymbol);
		}
	}

	void MoveIndicator () {
		currentCityIndicator.Move (CurrentCity.Position);
	}
}

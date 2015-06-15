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
	public string CurrentCitySymbol {
		get { return currentCitySymbol; }
	}

	public CityInfoBox cityInfoBox;
	public RoutesManager routesManager;
	public DayCounter dayCounter;
	public CurrentCityIndicator currentCityIndicator;
	public GameObject extraDayPrompt;
	bool initialized = false;

	void Start () {
		UpdateUnlockedCities ();
		UpdateInteractableCities ();
		initialized = true;
	}

	void OnEnable () {
		if (!initialized) return;
		UpdateUnlockedCities ();
		foreach (var city in Cities) {
			city.Value.UpdateState (IsCurrentCity (city.Value.symbol));
		}
		UpdateInteractableCities ();
	}

	public bool IsCurrentCity (string symbol) {
		return symbol == currentCitySymbol;
	}

	public bool CanVisitCity (string symbol) {
		return !currentCityIndicator.Moving 
			&& dayCounter.Count >= RouteCost (currentCitySymbol, symbol);
	}

	public void StayExtraDay (string symbol) {
		dayCounter.RemoveDays (1);
		CurrentCity.StayExtraDay ();
		InteractionsManager.Instance.OnStayExtraDay (symbol);
		NotebookManager.Instance.Close ();
	}

	public void VisitCity (string symbol) {
		dayCounter.RemoveDays (RouteCost (currentCitySymbol, symbol));
		currentCitySymbol = symbol;
		CurrentCity.Visit ();
		InteractionsManager.Instance.OnVisitCity (currentCitySymbol);
		MoveIndicator (OnVisit);
	}

	public void TravelToCity (string symbol) {
		dayCounter.RemoveDays (RouteCost (currentCitySymbol, symbol));
		currentCitySymbol = symbol;
		InteractionsManager.Instance.OnTravelToCity ();
		MoveIndicator (OnTravel);
	}
	
	void UpdateUnlockedCities () {
		Models.City[] models = DataManager.GetAllCities ();
		for (int i = 0; i < models.Length; i ++) {
			Models.City model = models[i];
			string symbol = model.symbol;
			if (model.unlocked && CanVisitCity (symbol)) 
				Cities[symbol].Unlock ();
		}
	}

	void UpdateInteractableCities () {
		foreach (var city in Cities) {
			city.Value.Interactable = 
				city.Value.Clickable 
				&& routesManager.RouteExists (city.Key, currentCitySymbol)
				|| IsCurrentCity (city.Value.symbol);
		}
	}

	void MoveIndicator (System.Action onArrive) {
		currentCityIndicator.Move (CurrentCity.Position, onArrive);
	}

	void OnVisit () {
		UpdateInteractableCities ();
		ParallaxLayerManager.Instance.Load (currentCitySymbol);
		NotebookManager.Instance.Close ();
	}

	void OnTravel () {
		UpdateInteractableCities ();
	}

	int RouteCost (string city1, string city2) {
		if (!routesManager.RouteExists (city1, city2)) return 0;
		return routesManager.FindRoute (city1, city2).Cost;
	}
}

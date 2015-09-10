using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Keeps track of all the cities in the map screen.
/// </summary>
public class CitiesManager : MB {

	/// <summary>
	/// This is a singleton.
	/// </summary>
	static CitiesManager instance = null;
	static public CitiesManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (CitiesManager)) as CitiesManager;
			}
			return instance;
		}
	}

	Dictionary<string, CityButton> cities;

	/// <summary>
	/// Gets a dictionary of CityButtons in the game, with the city's symbol as the key.
	/// </summary>
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

	public CityInfoBox cityInfoBox;
	public CurrentCityIndicator currentCityIndicator;

	bool startInCapitol = true;

	void Start () {
		InitCities ();
	}

	public void SetCity (CityItem city) {
		city.Visited = true;
		if (city.Symbol == "capitol") city.StayedExtraDay = true;
		PlayerData.InteractionGroup.SetInteractions (city.Symbol);
		PlayerData.CityGroup.CurrentCity = city.Symbol;
		OnVisit ();
	}

	/// <summary>
	/// Assigns the appropriate CityItem and RouteItems to each CityButton.
	/// </summary>
	void InitCities () {
		List<RouteItem> routeItems = PlayerData.RouteGroup.Routes;
		List<CityItem> cityItems = PlayerData.CityGroup.Cities;
		foreach (var city in Cities) {
			CityButton button = city.Value;
			string symbol = button.symbol;
			button.CityItem = cityItems.Find (x => x.Symbol == symbol);
			button.Routes = routeItems.FindAll (x => x.Terminals.ContainsCity (symbol));
		}
		if (startInCapitol)
			SetCity (cityItems.Find (x => x.Symbol == "capitol"));
	}

	/// <summary>
	/// Sets the current city, removes days based on cost, and moves the indicator on the map.
	/// </summary>
	/// <param name="city">The city to move to.</param>
	/// <param name="route">The route to move along.</param>
	/// <param name="onArrive">An action to take when the indicator arrives at the city (optional)</param>
	public void TravelToCity (CityItem city, RouteItem route, System.Action onArrive=null) {
		if (PlayerData.CityGroup.CurrentCity != city.Symbol)
			PlayerData.DayGroup.Remove (route.Cost);
		PlayerData.CityGroup.CurrentCity = city.Symbol;
		MoveIndicator (onArrive);
	}

	/// <summary>
	/// Moves to the given city and sets the interaction count.
	/// </summary>
	/// <param name="city">The city to move to.</param>
	/// <param name="route">The route to move along.</param>
	public void VisitCity (CityItem city, RouteItem route) {
		city.Visited = true;
		if (city.Symbol == "capitol") city.StayedExtraDay = true;
		PlayerData.InteractionGroup.SetInteractions (city.Symbol);
		TravelToCity (city, route, OnVisit);
	}

	/// <summary>
	/// Stay an extra day in the given city. Removes 1 day and updates the interaction count.
	/// </summary>
	/// <param name="city">The city to stay an extra day in.</param>
	public void StayExtraDay (CityItem city) {
		city.StayedExtraDay = true;
		PlayerData.DayGroup.Remove ();
		PlayerData.InteractionGroup.SetExtraInteractions (city.Symbol);
		OnVisit ();
	}

	void MoveIndicator (System.Action onArrive) {
		currentCityIndicator.Move (Cities[PlayerData.CityGroup.CurrentCity].Position, onArrive);
	}

	void OnVisit () {

		string currentCity = PlayerData.CityGroup.CurrentCity;

		DataManager.SceneContext = currentCity;
		NpcManager.InitNpcs ();
		ParallaxLayerManager.Instance.LoadFromSymbol (currentCity);
		
		// NotebookManager.Instance.Close ();
		NotebookManagerPhaseOne.Instance.CloseCanvases ();
	}
}

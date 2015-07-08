using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CitiesManager : MB {

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

	void Start () {
		InitCities ();
	}

	void InitCities () {
		List<RouteItem> routeItems = PlayerData.RouteGroup.Routes;
		List<CityItem> cityItems = PlayerData.CityGroup.Cities;
		foreach (var city in Cities) {
			CityButton button = city.Value;
			string symbol = button.symbol;
			button.CityItem = cityItems.Find (x => x.Symbol == symbol);
			button.Routes = routeItems.FindAll (x => x.Terminals.ContainsCity (symbol));
		}
	}

	public void TravelToCity (CityItem city, RouteItem route, System.Action onArrive=null) {
		PlayerData.CityGroup.CurrentCity = city.Symbol;
		PlayerData.DayGroup.Remove (route.Cost);
		MoveIndicator (onArrive);
	}

	public void VisitCity (CityItem city, RouteItem route) {
		city.Visited = true;
		PlayerData.InteractionGroup.Set (city.Model.npc_interactions);
		TravelToCity (city, route, OnVisit);
	}

	public void StayExtraDay (CityItem city) {
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
		
		NotebookManager.Instance.Close ();
	}

	#if DEBUG
	void OnGUI () {
		if (GUILayout.Button ("0 interactions")) {
			PlayerData.InteractionGroup.Clear ();
		}
	}
	#endif
}

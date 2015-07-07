using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CityButton : MB {

	Button button = null;
	Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}

	CityInfoBox infoBox;
	CityInfoBox InfoBox {
		get {
			if (infoBox == null) {
				infoBox = GameObject.Find ("CityInfoBox").GetScript<CityInfoBox> ();
			}
			return infoBox;
		}
	}

	public string symbol;
	Color visitedColor = new Color (1f, 0.5f, 0f, 1f);

	void Awake () {
		Button.onClick.AddListener (HandleClick);
	}

	CityItem cityItem;
	public CityItem CityItem { 
		get { return cityItem; }
		set { 
			cityItem = value;
			PlayerData.CityGroup.onUpdateCurrentCity += OnUpdateCurrentCity;
		}
	}

	List<RouteItem> routes = new List<RouteItem> ();
	public List<RouteItem> Routes {
		get { return routes; }
		set { 
			// This can only be set once
			if (routes.Count > 0) return;
			routes = value; 
			foreach (RouteItem route in routes) {
				route.onUpdateUnlocked += OnUpdateRouteUnlocked;
			}
			OnUpdateRouteUnlocked ();
		}
	}

	bool unlocked = false;
	bool Unlocked {
		get { return unlocked; }
		set { 
			unlocked = value;
			UpdateInteractableState ();
		}
	}

	bool IsCurrentCity {
		get { return PlayerData.CityGroup.CurrentCity == CityItem.Symbol; }
	}

	bool CanStayExtraDay {
		get { return IsCurrentCity && !PlayerData.DayGroup.Empty; }
	}

	bool CanAffordCost {
		get { return PlayerData.DayGroup.Count >= activeRoute.Cost; }
	}

	RouteItem activeRoute = null;
	public RouteItem ActiveRoute {
		get { return activeRoute; }
	}

	void Start () {
		PlayerData.InteractionGroup.onUpdate += UpdateInteractableState;
	}

	void OnUpdateRouteUnlocked () {
		foreach (RouteItem route in Routes) {
			if (route.Unlocked) {
				Unlocked = true;
				return;
			}
		}
		Unlocked = false;
	}

	void OnUpdateCurrentCity (string symbol) {
		UpdateInteractableState ();
	}

	bool IsOnCurrentCityRoute (string symbol) {
		activeRoute = Routes.Find (x => x.Unlocked && x.Terminals.ContainsCity (symbol));
		return activeRoute != null;
	}

	public void HandleClick () {
		InfoBox.Open (this);
	}

	void UpdateInteractableState () {
		bool onRoute = IsOnCurrentCityRoute (PlayerData.CityGroup.CurrentCity);
		if (!PlayerData.InteractionGroup.Empty) {
			Button.interactable = false;
		} else {
			Button.interactable = CanStayExtraDay || (onRoute && CanAffordCost);
		}
	}
}

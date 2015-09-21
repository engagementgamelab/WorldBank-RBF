using UnityEngine;
using UnityEngine.UI;
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

	CityItem cityItem = null;

	/// <summary>
	/// The CityItem associated with this CityButton. This can only be set once.
	/// </summary>
	public CityItem CityItem { 
		get { return cityItem; }
		set { 
			if (cityItem == null) {
				cityItem = value;
				PlayerData.CityGroup.onUpdateCurrentCity += OnUpdateCurrentCity;
			}
		}
	}

	List<RouteItem> routes = new List<RouteItem> ();

	/// <summary>
	/// The RouteItems associated with this CityButton. This can only be set once.
	/// </summary>
	public List<RouteItem> Routes {
		get { return routes; }
		set { 
			if (routes.Count > 0) return;
			routes = value; 
			foreach (RouteItem route in routes) {
				route.onUpdateUnlocked += UpdateInteractableState;
			}
			UpdateInteractableState ();
		}
	}

	bool IsCurrentCity {
		get { return PlayerData.CityGroup.CurrentCity == CityItem.Symbol; }
	}

	bool CanStayExtraDay {
		get { return !CityItem.StayedExtraDay && IsCurrentCity && !PlayerData.DayGroup.Empty; }
	}

	bool CanAffordCost {
		get { return PlayerData.DayGroup.Count >= activeRoute.Cost; }
	}

	RouteItem activeRoute = null;
	bool visitedCapitol = false;

	/// <summary>
	/// Gets the route that the player is currently on.
	/// </summary>
	public RouteItem ActiveRoute {
		get { return activeRoute; }
	}

	void Start () {
		PlayerData.InteractionGroup.onUpdate += UpdateInteractableState;
	}

	void OnEnable () {
		if (CityItem != null)
			UpdateInteractableState ();
	}

	void OnUpdateCurrentCity (string citySymbol) {
		if (citySymbol == "capitol") visitedCapitol = true;
		UpdateInteractableState ();
	}

	bool IsOnCurrentCityRoute (string citySymbol) {
		activeRoute = Routes.Find (x => x.Unlocked && x.Terminals.ContainsCity (citySymbol));
		return activeRoute != null;
	}

	public void HandleClick () {
		if (CurrentCityIndicator.Moving) return;
		AudioManager.Sfx.Play ("buttonpressneutral", "ui");
		InfoBox.Open (this);
	}

	void UpdateInteractableState () {
		
		bool onRoute = IsOnCurrentCityRoute (PlayerData.CityGroup.CurrentCity);

		if (PlayerData.InteractionGroup.Empty) {
			Button.interactable = CanStayExtraDay || (onRoute && CanAffordCost);
		} else {
			Button.interactable = IsCurrentCity;
		}
	}
}

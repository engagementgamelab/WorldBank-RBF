using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CityInfoBox : MB {

	public GameObject panel;
	public Text header;
	public Text body;
	public CityInfoBoxButton button1;
	public CityInfoBoxButton button2;
	public MapManager2 mapManager;

	/// <summary>
	/// Gets/sets the text that appears in the header.
	/// </summary>
	string Header {
		get { return header.text; }
		set { header.text = value; }
	}

	/// <summary>
	/// Gets/sets the text that appears in the body.
	/// </summary>
	string Body {
		get { return body.text; }
		set { body.text = value; }
	}

	/// <summary>
	/// Opens the info box and sets the content based on the information provided by the given CityButton.
	/// </summary>
	/// <param name="button">The CityButton that was clicked on.</param>
	public void Open (CityButton button) {
		
		if (PlayerData.DayGroup.Empty) {
			Header = "Out of days";
			Body = "You're all out of travel days!";
			panel.SetActive (true);
			return;
		}

		CityItem city = button.CityItem;
		bool currentCity = button.CityItem.Symbol == PlayerData.CityGroup.CurrentCity;

		if (city.Visited) {

			// Special case: Players can not spend an extra day in Capitol City
			if (city.StayedExtraDay || city.Symbol == "capitol") {
				Body = "You've already visited this city but you can pass through it.";
				SetButtons ("Cancel", Close, "Visit", () => TravelTo (city, button.ActiveRoute));
			} else {
				Body = "You've already visited this city but you can pass through it or spend an extra day talking to the rest of the people";
				if (currentCity) {
					SetButtons ("Cancel", Close, "Extra Day", () => StayExtraDay (city));
				} else {
					SetButtons ("Cancel", Close, "Visit", () => TravelTo (city, button.ActiveRoute));	
				}
			}
		} else {
			Body = city.Model.description;
			SetButtons ("Cancel", Close, "Visit", () => Visit (city, button.ActiveRoute));
		}

		Header = city.Model.display_name;
		panel.SetActive (true);
	}

	/// <summary>
	/// Handles the special case of the route between Mile and Zima.
	/// </summary>
	public void OpenRouteBlocked () {
		Header = "Blocked";
		Body = "You return to the train station and learn that bad weather has triggered a landslide to the east. The train tracks between Mile and Zima have been destroyed.\nA young man tugs on your shirt and grins. He says he drives a produce truck and can get you to Kibari, the heart of the highlands. \"From there,\" he says, \"you can get anywhere!\"";
		SetButtons ("Ok", UnlockRoute);
		panel.SetActive (true);
	}

	void UnlockRoute () {
		PlayerData.RouteGroup.Lock ("unlockable_route_mile_to_zima");
		PlayerData.UnlockItem("unlockable_route_kibari_to_mile");
		Close ();
	}

	void Close () {
		panel.SetActive (false);
	}

	void TravelTo (CityItem city, RouteItem route) {
		CitiesManager.Instance.TravelToCity (city, route);
		Close ();
	}

	void Visit (CityItem city, RouteItem route) {
		if (route.Terminals == new Terminals ("mile", "zima")) {
			OpenRouteBlocked ();
			return;
		}
		CitiesManager.Instance.VisitCity (city, route);
		Close ();
	}

	void StayExtraDay (CityItem city) {
		CitiesManager.Instance.StayExtraDay (city);
		Close ();
	}

	void SetButtons (string label1, UnityAction onButton1, string label2="", UnityAction onButton2=null) {
		button1.gameObject.SetActive (true);
		button1.Label = label1;
		button1.Button.onClick.RemoveAllListeners ();
		button1.Button.onClick.AddListener (onButton1);
		if (label2 != "") {
			button2.gameObject.SetActive (true);
			button2.Label = label2;
			button2.Button.onClick.RemoveAllListeners ();
			button2.Button.onClick.AddListener (onButton2);
		} else {
			button2.gameObject.SetActive (false);
		}
	}
}

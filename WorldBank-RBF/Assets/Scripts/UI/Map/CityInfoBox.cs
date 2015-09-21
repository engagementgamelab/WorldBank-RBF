using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CityInfoBox : MB {

	public GameObject panel;
	public GameObject background;
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

		CityItem city = button.CityItem;
		bool currentCity = button.CityItem.Symbol == PlayerData.CityGroup.CurrentCity;
		Body = city.Model.description;

		if (currentCity) {
			if (city.StayedExtraDay) {
				if (!PlayerData.InteractionGroup.Empty)
					NotebookManagerPhaseOne.Instance.CloseCanvases ();
				return;
			} 
			SetBody (city, "copy_city_extra_day");
			SetButton ("Re-enter", () => StayExtraDay (city));
		} else {
			if (city.Visited) {
				if (city.StayedExtraDay) {
					SetBody (city, "copy_city_pass_through");
					SetButton ("Continue", () => TravelTo (city, button.ActiveRoute, false));
				} else {
					SetBody (city, "copy_city_pass_through_extra_day");
					SetButton ("Go", () => TravelTo (city, button.ActiveRoute, true));
				}
			} else {
				SetBody (city, "copy_city_visit");
				SetButton ("Visit", () => Visit (city, button.ActiveRoute));
			}
		}

		Header = city.Model.display_name;
		SetActive (true);
	}

	void SetBody (CityItem city, string key) {
		Body = city.Model.description + "\n\n" + DataManager.GetUIText (key);
	}

	/// <summary>
	/// Handles the special case of the route between Mile and Zima.
	/// </summary>
	public void OpenRouteBlocked () {
		Header = "Blocked";
		Body = "You return to the train station and learn that bad weather has triggered a landslide to the east. The train tracks between Mile and Zima have been destroyed.\nA young man tugs on your shirt and grins. He says he drives a produce truck and can get you to Kibari, the heart of the highlands. \"From there,\" he says, \"you can get anywhere!\"";
		SetButtons ("Ok", UnlockRoute);
		SetActive (true);
	}

	void UnlockRoute () {
		PlayerData.RouteGroup.Lock ("unlockable_route_mile_to_zima");
		PlayerData.UnlockItem("unlockable_route_kibari_to_mile");
		Close ();
	}

	void Close () {
		SetActive (false);
	}

	void TravelTo (CityItem city, RouteItem route, bool reopenBox) {
		if (RouteBlocked (city, route)) {
			OpenRouteBlocked ();
			return;
		}
		CitiesManager.Instance.TravelToCity (city, route, reopenBox);
		Close ();
	}

	void Visit (CityItem city, RouteItem route) {
		if (RouteBlocked (city, route)) {
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

	void SetButton (string label, UnityAction onButton) {
		SetButtons ("Cancel", Close, label, onButton);
	}

	void SetButtons (string label1, UnityAction onButton1, string label2="", UnityAction onButton2=null) {
		button1.gameObject.SetActive (true);
		button1.Label = label1;
		button1.Button.onClick.RemoveAllListeners ();
		button1.Button.onClick.AddListener (onButton1);
		button1.Button.onClick.AddListener (() => AudioManager.Sfx.Play ("buttonpressnegative", "ui"));
		if (label2 != "") {
			button2.gameObject.SetActive (true);
			button2.Label = label2;
			button2.Button.onClick.RemoveAllListeners ();
			button2.Button.onClick.AddListener (onButton2);
			button2.Button.onClick.AddListener (() => AudioManager.Sfx.Play ("buttonpresspositive", "ui"));
		} else {
			button2.gameObject.SetActive (false);
		}
	}

	void SetActive (bool active) {
		panel.SetActive (active);
		background.SetActive (active);
	}

	bool RouteBlocked (CityItem city, RouteItem route) {
		return city.Symbol == "zima" && route.Terminals == new Terminals ("mile", "zima");
	}
}

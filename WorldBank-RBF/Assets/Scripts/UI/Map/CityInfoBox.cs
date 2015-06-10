using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CityInfoBox : MB {

	public GameObject panel;
	public Text cityName;
	public Text cityDescription;
	string citySymbol;

	public void Open (string symbol) {
		this.citySymbol = symbol;
		Models.City city = DataManager.GetCityInfo (symbol);
		cityName.text = city.display_name;
		cityDescription.text = city.description;
		panel.SetActive (true);
	}

	public void Close () {
		panel.SetActive (false);
	}

	public void OnPressVisit () {
		if (CitiesManager.Instance.RequestVisitCity (citySymbol)) {
			Close ();
		}
	}

	public void OnPressBack () {
		Close ();
	}
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CityInfoBox : MB {

	public GameObject panel;
	public Text cityName;
	public Text cityDescription;

	public void Open (string symbol) {
		Models.City city = DataManager.GetCityInfo (symbol);
		Debug.Log (city);
		cityName.text = city.display_name;
		cityDescription.text = city.description;
		panel.SetActive (true);
	}

	public void Close () {
		panel.SetActive (false);
	}

	public void OnPressBack () {
		Close ();
	}
}
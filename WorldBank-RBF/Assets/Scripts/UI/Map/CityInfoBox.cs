using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CityInfoBox : MB {

	public GameObject panel;
	public Text cityName;
	public Text cityDescription;
	public GameObject interactionsPanel;
	string citySymbol;

	public void Open (CityButton button) {
		string symbol = button.symbol;
		this.citySymbol = symbol;
		Models.City city = DataManager.GetCityInfo (symbol);
		cityName.text = city.display_name;
		cityDescription.text = button.Visited 
			? "You've already visited this city but you can pass through it."
			: city.description;
		panel.SetActive (true);
	}

	public void Close () {
		panel.SetActive (false);
	}

	public void OnPressVisit () {
		if (InteractionsManager.Instance.HasInteractions) {
			interactionsPanel.SetActive (true);
			Close ();
		} else {
			MoveToCity ();
		}
	}

	public void MoveToCity () {
		if (CitiesManager.Instance.RequestVisitCity (citySymbol)) {
			interactionsPanel.SetActive (false);
			Close ();
		}
	}

	public void CancelMove () {
		interactionsPanel.SetActive (false);
	}

	public void OnPressBack () {
		Close ();
	}
}
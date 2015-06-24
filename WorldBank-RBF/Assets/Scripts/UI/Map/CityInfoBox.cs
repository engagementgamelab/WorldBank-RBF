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

	string Header {
		get { return header.text; }
		set { header.text = value; }
	}

	string Body {
		get { return body.text; }
		set { body.text = value; }
	}

	// TODO: clean up
	public void Open (CityButton button) {
		
		string symbol 			= button.symbol;
		Models.City model 		= DataManager.GetCityInfo (symbol);
		string name 			= model.display_name;
		string description 		= model.description;
		CityButton.State state 	= button.CityState;

		if (CitiesManager.Instance.IsCurrentCity (symbol)) {
			switch (state) {
				case CityButton.State.Unlocked:
				case CityButton.State.Visiting:
				case CityButton.State.StayingExtraDay:
				case CityButton.State.PassThrough:
					Body = description;
					SetButtons ("Ok", Close);
					break;
				case CityButton.State.ExtraDayUnlocked:
					Body = "Would you like to stay an extra day in this city? You will be able to talk to the rest of the nice people :)";
					SetButtons ("Cancel", Close, "Extra Day", () => StayExtraDay (symbol));
					break;
			}
		} else {
			if (InteractionsManager.Instance.HasInteractions) {
				Body = description;
				SetButtons ("Cancel", Close);
			} else {
				switch (state) {
					case CityButton.State.Unlocked:
						Body = description;
						SetButtons ("Cancel", Close, "Visit", () => Visit (symbol));
						break;
					case CityButton.State.ExtraDayUnlocked:
						Body = "You've already visited this city but you can pass through it or spend an extra day talking to the rest of the nice people :)";
						SetButtons ("Cancel", Close, "Visit", () => TravelTo (symbol));
						break;
					case CityButton.State.PassThrough:
						Body = "You've already visited this city but you can pass through it.";
						SetButtons ("Cancel", Close, "Visit", () => TravelTo (symbol));
						break;
				}
			}
		}
		
		Header = name;
		panel.SetActive (true);
	}

	public void OpenRouteBlocked () {
		Header = "Route DESTROYED";
		Body = "Oopsie shippy dip! Can't go this dang way, but it's looking good for a hop/skip/and-a-kick to ~~ kooky ~~ Kibari! ;)";
		SetButtons ("Ok", UnlockRoute);
		panel.SetActive (true);
	}

	void UnlockRoute () {
		PlayerData.UnlockImplementation("unlockable_route_kibari_to_mile");
		mapManager.UpdateMap ();
		Close ();
	}

	void Close () {
		panel.SetActive (false);
	}

	void StayExtraDay (string symbol) {
		CitiesManager.Instance.StayExtraDay (symbol);
		Close ();
	}

	void Visit (string symbol) {
		if (CitiesManager.Instance.VisitCity (symbol)) {
			Close ();
		}
	}

	void TravelTo (string symbol) {
		CitiesManager.Instance.TravelToCity (symbol);
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

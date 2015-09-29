using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityLighting : MonoBehaviour {

	public GameObject kibari;
	public GameObject crup;

	Dictionary<string, GameObject> cities;
	Dictionary<string, GameObject> Cities {
		get {
			if (cities == null) {
				cities = new Dictionary<string, GameObject> ();
				cities.Add ("kibari", kibari);
				cities.Add ("crup", crup);
			}
			return cities;
		}
	}

	void Start () {
		Events.instance.AddListener<ArriveInCityEvent> (OnArriveInCityEvent);
	}

	void OnArriveInCityEvent (ArriveInCityEvent e) {
		SetLighting (e.City);
	}

	void SetLighting (string city) {
		foreach (var c in Cities) {
			c.Value.SetActive (false);
		}
		GameObject activeCity;
		if (Cities.TryGetValue (city, out activeCity)) {
			activeCity.SetActive (true);
		}
	}
}

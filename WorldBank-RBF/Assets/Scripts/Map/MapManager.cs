/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public Button cityButtonPrefab;

	private Transform cityCanvas;

	void Start () {

		cityCanvas = transform.Find("Map Buttons");

	}

	// Use this for initialization
	public void LoadCities() {

        foreach(DataManager.City city in DataManager.GetCityData())
        	GenerateCityButton(city);
	
	}

	private void GenerateCityButton(DataManager.City city) {

		// Create NPC prefab instance
		Button cityButton = (Button)Instantiate(cityButtonPrefab);
	  
	    cityButton.transform.parent = cityCanvas;
	    cityButton.transform.localScale = new Vector3(1, 1, 1);
	    
	    Text label = cityButton.transform.FindChild("Text").GetComponent<Text>();
		label.text = city.display_name + ": " + city.description;
 
	    cityButton.onClick.AddListener(() => gameObject.SetActive(false));
	    cityButton.onClick.AddListener(() => DialogManager.instance.LoadDialogForCity(city.symbol));
	}
}
*/
﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public string parallaxScene;
	public CanvasRenderer dialogueBoxPrefab;
	public Button cityButtonPrefab;

	public string citySceneName;

	private Transform cityCanvas;
	private Light citySpotlight;

	void Start () {

		cityCanvas = transform.Find("Map Buttons");
		citySpotlight = transform.Find("City Light").GetComponent<Light>();

	}

	void Update() {

		// Light glow
		citySpotlight.spotAngle = Mathf.PingPong(Time.time*14, 30) + 5;
		citySpotlight.intensity = Mathf.PingPong(Time.time, 2) + 1;

	}

	// Use this for initialization
	public void LoadCities() {

        foreach(Models.City city in DataManager.GetAllCities())
        	GenerateCityButton(city);
	
	}

	private void GenerateCityButton(Models.City city) {

		// Create NPC prefab instance
		Button cityButton = (Button)Instantiate(cityButtonPrefab);
	  
	    cityButton.transform.parent = cityCanvas;
	    cityButton.transform.localScale = new Vector3(1, 1, 1);
	    
	    Text label = cityButton.transform.FindChild("Text").GetComponent<Text>();
		label.text = city.display_name;
 
	    cityButton.onClick.AddListener(() => cityCanvas.gameObject.SetActive(false));

	}

	public void ShowCityDialog(string citySymbol) {

		Models.City city = DataManager.GetCityInfo(citySymbol);

		CanvasRenderer diagRenderer = DialogManager.instance.CreateGenericDialog(city.description);
	  
	  	// Setup go button
	  	GameObject goBtnObj = diagRenderer.transform.Find("Action Button").gameObject;
	    Button goBtn = goBtnObj.GetComponent<Button>();

	    goBtnObj.SetActive(true);
	    
	    Text label = goBtn.transform.FindChild("Text").GetComponent<Text>();
		label.text = "Go to " + city.display_name;
 
 		// Set city context and go to city
	    goBtn.onClick.AddListener(() => DataManager.SetSceneContext(city.symbol));

	    goBtn.gameObject.SetActive(true);
	    goBtn.onClick.AddListener(() => Application.LoadLevel(citySceneName));
	}
}
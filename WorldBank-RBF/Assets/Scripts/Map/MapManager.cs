using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public string parallaxScene;
	public CanvasRenderer dialogueBoxPrefab;
	public Button cityButtonPrefab;

	private Transform cityCanvas;

	void Start () {

		cityCanvas = transform.Find("Map Buttons");

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

		Instantiate(cityButtonPrefab);

		CanvasRenderer diagRenderer = DialogManager.instance.CreateGenericDialog(city.description);
	  
	  	// Setup go button
	    Button goBtn = (Button)diagRenderer.transform.Find("Action Button").GetComponent<Button>();
	    
	    Text label = goBtn.transform.FindChild("Text").GetComponent<Text>();
		label.text = "Go to " + city.display_name;
 
	    goBtn.onClick.AddListener(() => gameObject.SetActive(false));
	    goBtn.onClick.AddListener(() => DataManager.SetSceneContext(city.symbol));
	    goBtn.onClick.AddListener(() => Application.LoadLevel(parallaxScene));

	    goBtn.gameObject.SetActive(true);

	}
}
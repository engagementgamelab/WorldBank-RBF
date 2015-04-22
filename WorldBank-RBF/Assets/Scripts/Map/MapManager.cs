/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public CanvasRenderer dialogueBoxPrefab;
	public Button cityButtonPrefab;

	private GameObject canvasParent;

	private Transform cityCanvas;

	void Start () {

		cityCanvas = transform.Find("Map Buttons");

		canvasParent = GenerateCanvasParent();

	}

	private GameObject GenerateCanvasParent() {
		GameObject g = new GameObject("CanvasParent");
		g.transform.parent = transform;
	    g.transform.localScale = new Vector3(1, 1, 1);

		Canvas canvas = g.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		
		CanvasScaler cs = g.AddComponent<CanvasScaler>();
		cs.dynamicPixelsPerUnit = 100;
		
		g.AddComponent<GraphicRaycaster>();
		g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3.0f);
		g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 3.0f);
 
 		return g;
	}

	// Use this for initialization
	public void LoadCities() {

        foreach(Models.City city in DataManager.GetCityData())
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
	    cityButton.onClick.AddListener(() => ShowCityDialog(city));
	    // cityButton.onClick.AddListener(() => DialogManager.instance.LoadDialogForCity(city.symbol));
	}

	private void ShowCityDialog(Models.City city) {
		Button goBtn = (Button)Instantiate(cityButtonPrefab);
		CanvasRenderer diagRenderer = (CanvasRenderer)Instantiate(dialogueBoxPrefab);
		
		diagRenderer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;  
		diagRenderer.GetComponent<RectTransform>().pivot = new Vector2(-.5f, -.5f);

	    diagRenderer.transform.parent = canvasParent.transform;
	    diagRenderer.transform.localScale = new Vector3(1, 1, 1);

	    Text diagText = diagRenderer.transform.Find("Panel/Dialogue Text").GetComponent<Text>() as Text;
	    diagText.text = city.description;
	  
	  	// Make go button
	    goBtn.transform.parent = diagRenderer.transform.Find("Panel/Single Button");
	    goBtn.transform.localScale = new Vector3(1, 1, 1);
	    
	    Text label = goBtn.transform.FindChild("Text").GetComponent<Text>();
		label.text = "Go";
 
	    goBtn.onClick.AddListener(() => gameObject.SetActive(false));
	    goBtn.onClick.AddListener(() => DialogManager.instance.LoadDialogForCity(city.symbol));

	}
}
*/

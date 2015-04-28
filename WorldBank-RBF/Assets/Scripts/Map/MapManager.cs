using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public string parallaxScene;
	public CanvasRenderer dialogueBoxPrefab;
	public Button cityButtonPrefab;

	public string citySceneName;
	public float cameraDamping;

	public Vector3 cameraLimitsMin;
	public Vector3 cameraLimitsMax;

	private Transform cityCanvas;
	private Transform cameraTransform;

	private Quaternion initialCamRotation;
	private Quaternion targetCamRotation;

	private Vector3 initialCamPosition;
	private Vector3 targetCamPosition;

	private List<Light> citySpotlights;

	private Vector3 initialDialogScale;

	void Start () {

		cityCanvas = transform.Find("Map Buttons");
		cameraTransform = Camera.main.GetComponent<Transform>();

		citySpotlights = new List<Light>();
		GameObject[] cityHighlights = GameObject.FindGameObjectsWithTag("CityHighlight");

		Array.ForEach(cityHighlights, element => citySpotlights.Add(element.GetComponent<Light>()));

/*		cameraLimitsMin.x = cameraTransform.rotation.x + cameraLimitsMin.x;
		cameraLimitsMin.y = cameraTransform.rotation.y + cameraLimitsMin.y;
		cameraLimitsMin.z = cameraTransform.rotation.z + cameraLimitsMin.z;

		cameraLimitsMax.x = cameraTransform.rotation.x + cameraLimitsMin.x;
		cameraLimitsMax.y = cameraTransform.rotation.y + cameraLimitsMin.y;
		cameraLimitsMax.z = cameraTransform.rotation.z + cameraLimitsMin.z;*/

	}

	void Update() {

		// City lights glow
		citySpotlights.ForEach(delegate(Light citySpotlight)
        {
			citySpotlight.spotAngle = Mathf.PingPong(Time.time*14, 30) + 5;
			citySpotlight.intensity = Mathf.PingPong(Time.time, 2) + 1;
        });

		// Catch clicks/touches
        if (Input.GetMouseButtonDown(0))
		{

			RaycastHit hit;
	        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	        
	        if (Physics.Raycast(ray, out hit)) {

	        	if(Camera.main.GetComponent<Animator>().enabled) {
	        	
	        		initialCamRotation = cameraTransform.rotation;
	        		initialCamPosition = cameraTransform.position;

	        	}

	        	// Disable animator on camera
	        	Camera.main.GetComponent<Animator>().enabled = false;

	        	Vector3 mousePos = Input.mousePosition;
	        	mousePos.z = hit.transform.position.z - cameraTransform.position.z;

	        	Vector3 lookPos = (Camera.main.ScreenToWorldPoint(mousePos) - cameraTransform.position);

				targetCamPosition = lookPos.normalized;
				targetCamPosition.z = cameraTransform.position.z;
	        	
	        	lookPos = Vector3.Scale(lookPos, new Vector3(cameraDamping, cameraDamping, cameraDamping));

	        	// lookPos.x = Mathf.Clamp(lookPos.x, cameraLimitsMin.x, cameraLimitsMax.x);
	        	// lookPos.y = Mathf.Clamp(lookPos.y, cameraLimitsMin.y, cameraLimitsMax.y);
	        	// lookPos.z = Mathf.Clamp(lookPos.z, cameraLimitsMin.z, cameraLimitsMax.z);

				targetCamRotation = Quaternion.LookRotation(lookPos, Vector3.up);
	        }

		}
    	
    	cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetCamRotation, Time.deltaTime * 4);
    	cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCamPosition, 2 * Time.deltaTime);

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

		// Get data for selected city
		Models.City city = DataManager.GetCityInfo(citySymbol);

		GameObject diagRenderer = DialogManager.instance.CreateGenericDialog(city.description);

		initialDialogScale = diagRenderer.transform.localScale;
	  
	  	// Setup go button
	  	GameObject goBtnObj = diagRenderer.transform.Find("Action Button").gameObject;
	    goBtnObj.SetActive(city.unlocked);
		
		Button goBtn = goBtnObj.GetComponent<Button>();
		Text label = goBtn.transform.FindChild("Text").GetComponent<Text>();
		label.text = "Go to " + city.display_name;
 
 		// Set city context and go to city
	    goBtn.onClick.AddListener(() => DataManager.SetSceneContext(city.symbol));
	    goBtn.onClick.AddListener(() => Application.LoadLevel(citySceneName));

	    // Reset camera position
	    targetCamRotation = initialCamRotation;
	    targetCamPosition = initialCamPosition;

	}

	IEnumerator ZoomInMapUI() {

	    yield return null;

	}

}
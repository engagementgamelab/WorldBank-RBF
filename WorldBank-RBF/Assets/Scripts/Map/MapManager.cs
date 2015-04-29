using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

	public CanvasRenderer dialogueBoxPrefab;
	public Button cityButtonPrefab;

	public string citySceneName;
	public float cameraDamping;
	public float dragSpeed = 2;

	public Vector3 cameraLimitsMin;
	public Vector3 cameraLimitsMax;

    private Vector3 dragOrigin;

	private Transform cityCanvas;
	private Transform cameraTransform;

	private Collider mapCollider;

	private Quaternion initialCamRotation;
	private Quaternion targetCamRotation;

	private Vector3 initialCamPosition;
	private Vector3 targetCamPosition;

	private Vector3 initialDialogScale;
	private Vector2 initialDialogAnchor;

	private GameObject cityDialog;
	private Animator dialogAnimator;

	private List<Light> citySpotlights;

	const string dialogOpenID = "Open";
	const string dialogCloseID = "Closed";
	const string dialogSwitchID = "Switch";


	void Start () {

		cityCanvas = transform.Find("Map Buttons");
		mapCollider = transform.Find("Background").GetComponent<Collider>();
		cameraTransform = Camera.main.GetComponent<Transform>();

		cityDialog = Camera.main.transform.Find("DialogueBox").gameObject;

		initialDialogAnchor = cityDialog.GetComponent<RectTransform>().anchoredPosition;

		citySpotlights = new List<Light>();
		GameObject[] cityHighlights = GameObject.FindGameObjectsWithTag("CityHighlight");

		Array.ForEach(cityHighlights, element => citySpotlights.Add(element.GetComponent<Light>()));

	}

	void Update() {

		// City lights glow
		citySpotlights.ForEach(delegate(Light citySpotlight)
        {
			citySpotlight.spotAngle = Mathf.PingPong(Time.time*14, 30) + 5;
			citySpotlight.intensity = Mathf.PingPong(Time.time, 2) + 1;
        });

        if(Camera.main.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        	return;

		// Set drag origin on click/touch
    	if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
        	
        	// Disable animator on camera
        	Camera.main.GetComponent<Animator>().enabled = false;

            return;
        }
 
 		// Do not move camera if mouse button is not down
        if (!Input.GetMouseButton(0)) return;

        // Create camera limits based on map collider bounds
        Vector3 mapCenter = mapCollider.bounds.center;
        float maxYLimit = mapCollider.bounds.max.y - 1;
        float minYLimit = mapCollider.bounds.min.y + 1;
 
 		// Calculate how much to translate camera
        Vector3 posDelta = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 camTranslation = new Vector3(posDelta.x * dragSpeed, posDelta.y * dragSpeed, 0);

        //  Save x/y movement targets
		float xTarget = (cameraTransform.position + camTranslation).x;
		float yTarget = (cameraTransform.position + camTranslation).y;

		bool isWithinXBounds = xTarget >= (mapCenter.x - 1) && xTarget <= (mapCenter.x + 1);
		bool isWithinYBounds = (yTarget <= maxYLimit && yTarget >= minYLimit);

		// Move camera only if target destination is within bounds
		if(isWithinXBounds && isWithinYBounds)
			cameraTransform.Translate(camTranslation, Space.World);

		// Ensure camera is facing map
        Vector3 rotDelta = mapCollider.transform.position - cameraTransform.position;
	    Quaternion targetRotation = Quaternion.LookRotation(rotDelta);
	    cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, Time.deltaTime * 2);

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

	public IEnumerator ShowCityDialog(string citySymbol) {

		// Get data for selected city
		Models.City city = DataManager.GetCityInfo(citySymbol);

		// GameObject diagRenderer = DialogManager.instance.CreateGenericDialog(city.description);
		cityDialog.transform.Find("Content/Text").GetComponent<Text>().text = city.description;
	 	dialogAnimator = cityDialog.GetComponent<Animator>();
		if(!dialogAnimator.GetBool(dialogOpenID))
			dialogAnimator.SetBool(dialogOpenID, true);
		else
		{
			dialogAnimator.SetTrigger(dialogSwitchID);
			yield return new WaitForSeconds(2);
		}

		initialDialogScale = cityDialog.transform.localScale;
	  
	  	// Setup go/go back buttons
	  	GameObject goBtnObj = cityDialog.transform.Find("Action Button").gameObject;
	  	GameObject goBackBtnObj = cityDialog.transform.Find("Go Back").gameObject;
	    
	    goBtnObj.SetActive(city.unlocked);
		
		Button goBtn = goBtnObj.GetComponent<Button>();
		Button goBackBtn = goBackBtnObj.GetComponent<Button>();
		Text label = goBtn.transform.FindChild("Text").GetComponent<Text>();
		label.text = "Go to " + city.display_name;
 
 		// Set city context and go to city
	    goBtn.onClick.AddListener(() => DataManager.SetSceneContext(city.symbol));
	    goBtn.onClick.AddListener(() => Application.LoadLevel(citySceneName));

	    goBackBtn.onClick.AddListener(() => CloseCurrent());

	    // Reset camera position
	    targetCamRotation = initialCamRotation;
	    targetCamPosition = initialCamPosition;

	}

 	public void CloseCurrent()
    {
       /* if (dialogAnimator == null)
            return;

        //Start the close animation.
        dialogAnimator.SetBool(dialogOpenID, false);

        //Start Coroutine to disable the hierarchy when closing animation finishes.
        StartCoroutine(DisablePanelDeleyed(dialogAnimator));*/

        dialogAnimator.SetBool(dialogOpenID, false);
        // dialogAnimator.SetBool(dialogCloseID, true);
        
    }
/*
    //Coroutine that will detect when the Closing animation is finished and it will deactivate the
    //hierarchy.
    IEnumerator DisablePanelDeleyed(Animator anim)
    {
        bool closedStateReached = false;
        bool wantToClose = true;
        while (!closedStateReached && wantToClose)
        {
            if (!anim.IsInTransition(0))
                closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(dialogCloseIDa);

            wantToClose = !anim.GetBool(dialogOpenID);

            yield return new WaitForEndOfFrame();
        }

        if (wantToClose)
            anim.gameObject.SetActive(false);
    }*/

}
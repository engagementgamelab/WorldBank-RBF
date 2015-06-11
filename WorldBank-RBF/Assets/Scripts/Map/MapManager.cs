/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 MapManager.cs
 World map management.

 Created by Johnny Richardson on 4/21/15.
==============
*/

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

// Deprecated 06/07
// TODO: Tons of cleanup/docs!
public class MapManager : MonoBehaviour {

	public CanvasRenderer dialogueBoxPrefab;
	public Button cityButtonPrefab;

	public string citySceneName;
	public float cameraDamping;
	public float cameraBounceFactor;
	public float dragSpeed = 2;
	public float panDrag;

	public Vector3 cameraLimitsMin;
	public Vector3 cameraLimitsMax;

	public GameObject daysLeftPanel;

	private Transform cityCanvas;
	private Transform cameraTransform;

	private Collider mapCollider;

	private Quaternion initialCamRotation;
	private Quaternion targetCamRotation;

	private Vector3 initialCamPosition;
	private Vector3 targetCamPosition;

	private Vector3 initialDialogScale;
	private Vector2 initialDialogAnchor;

    private Vector3 dragOrigin;

	private GameObject cityDialog;
	private Text cityDialogText;
	private Animator dialogAnimator;

	private List<Light> citySpotlights;

	private Text daysLeftText;

	const string dialogOpenID = "Open";
	const string dialogCloseID = "Closed";
	const string dialogSwitchID = "Switch";

	private int daysAllowed = 9;
	
	private Vector3 cameraLimitPush;			// Position of cursor when mouse dragging starts
	private bool isPanning;				// Is the camera being panned?
	private bool isRotating;			// Is the camera being rotated?
	private bool isZooming;				// Is the camera zooming?


	void Start () {

		cityCanvas = transform.Find("Map Buttons");
		mapCollider = transform.Find("Background").GetComponent<Collider>();
		cameraTransform = Camera.main.GetComponent<Transform>();

		cityDialog = Camera.main.transform.Find("DialogueBox").gameObject;
		cityDialogText = cityDialog.transform.Find("Content/Text").GetComponent<Text>();

		initialDialogAnchor = cityDialog.GetComponent<RectTransform>().anchoredPosition;

		daysLeftText = daysLeftPanel.transform.Find("Text").GetComponent<Text>();

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

		// Do nothing else during camera animation
        if(CameraIsAnimating())
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

        float maxXLimit = mapCollider.bounds.max.x;
        float minXLimit = mapCollider.bounds.min.x;

        float maxYLimit = mapCollider.bounds.max.y;
        float minYLimit = mapCollider.bounds.min.y;
 
 		// Calculate how much to translate camera
        Vector3 posDelta = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 camTranslation = new Vector3(posDelta.x * dragSpeed, posDelta.y * dragSpeed, 0);

        //  Save x/y movement targets
		float xTarget = (cameraTransform.position + camTranslation).x;
		float yTarget = (cameraTransform.position + camTranslation).y;

		bool atMinXLimit = xTarget >= minXLimit;
		bool atMaxXLimit = xTarget <= maxXLimit;

		bool atMinYLimit = yTarget >= minYLimit;
		bool atMaxYLimit = yTarget <= maxYLimit;

		bool isWithinXBounds = atMinXLimit && atMaxXLimit;
		bool isWithinYBounds = atMinYLimit && atMaxYLimit;

		// Move camera only if target destination is within bounds
		if(isWithinXBounds && isWithinYBounds) {
			cameraTransform.Translate(camTranslation, Space.World);
			Camera.main.GetComponent<Rigidbody>().velocity = Vector3.zero;
}
	/*			else
		{	
			if(!isWithinXBounds)
				cameraLimitPush = new Vector3((atMinXLimit ? -1 : 1) * cameraBounceFactor, 0, 0);
			if(!isWithinYBounds)
				cameraLimitPush = new Vector3(0, (atMinYLimit ? -1 : 1) * cameraBounceFactor, 0);
			
			isPanning = true;
		}
*/
		// Ensure camera is facing map
        Vector3 rotDelta = mapCollider.transform.position - cameraTransform.position;
	    Quaternion targetRotation = Quaternion.LookRotation(rotDelta);
	    // cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, Time.deltaTime * 2);
	}

	void FixedUpdate()
	{
		// == Movement Code ==
		
		// Rotate camera along X and Y axis
		/*if (isRotating)
		{
			// Get mouse displacement vector from original to current position
	    	Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
			
			// Set Drag
			Camera.main.GetComponent<Rigidbody>().angularDrag = turnDrag;
			
			// Two rotations are required, one for x-mouse movement and one for y-mouse movement
			Camera.main.GetComponent<Rigidbody>().AddTorque(-pos.y * turnSpeed * transform.right, ForceMode.Acceleration);
			Camera.main.GetComponent<Rigidbody>().AddTorque(pos.x * turnSpeed * transform.up, ForceMode.Acceleration);
		}*/
		
		// Move (pan) the camera on it's XY plane
		if (isPanning)
		{

	        // Create camera limits based on map collider bounds
/*	        Vector3 mapCenter = mapCollider.bounds.center;

	        float maxXLimit = mapCollider.bounds.max.x + 1;
	        float minXLimit = mapCollider.bounds.min.x - 1;

	        float maxYLimit = mapCollider.bounds.max.y - 1;
	        float minYLimit = mapCollider.bounds.min.y + 1;
			
			// Get mouse displacement vector from original to current position
    		Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
    		Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);

	        //  Save x/y movement targets
			float xTarget = (cameraTransform.position).x;
			float yTarget = (cameraTransform.position).y;

			bool isWithinXBounds = xTarget <= maxXLimit && xTarget >= minXLimit;
			bool isWithinYBounds = (yTarget <= maxYLimit && yTarget >= minYLimit);

			// Move camera only if target destination is within bounds
			if(!isWithinXBounds)
				move = new Vector3(pos.x * -panSpeed, pos.y * panSpeed, 0);
			if(!isWithinYBounds)
				move = new Vector3(pos.x * panSpeed, pos.y * -panSpeed, 0);

			// Apply the pan's move vector in the orientation of the camera's front
			Quaternion forwardRotation = Quaternion.LookRotation(transform.forward, transform.up);
			move = forwardRotation * move;*/
			
			// Set Drag
			Camera.main.GetComponent<Rigidbody>().drag = panDrag;
			
			// Pan
			Camera.main.GetComponent<Rigidbody>().AddForce(cameraLimitPush, ForceMode.Acceleration);
		}
		
		// Move the camera linearly along Z axis
/*		if (isZooming)
		{
			// Get mouse displacement vector from original to current position
	    		Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
	    		Vector3 move = pos.y * zoomSpeed * transform.forward; 
			
			// Set Drag
			Camera.main.GetComponent<Rigidbody>().drag = zoomDrag;
			
			// Zoom
			Camera.main.GetComponent<Rigidbody>().AddForce(move, ForceMode.Acceleration);
		}*/
	}

	/// <summary>
	/// Reports if the main camera is currently under control of an animation.
	/// </summary>
	public bool CameraIsAnimating() {

		// If animation state's normalized time is less than 1, it's still going, as this is a fractional of the current loop
		return Camera.main.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
	}

	/// <summary>
	/// Generate a dialog with text and relevant buttons for city
	/// </summary>
	/// <param name="strCitySymbol">Text to show in the dialogue</param>
	public IEnumerator ShowCityDialog(string strCitySymbol) {

		dialogAnimator = cityDialog.GetComponent<Animator>();

		// Get data for selected city
		Models.City city = DataManager.GetCityInfo(strCitySymbol);

		string strCityTxt = city.description + "\n   <i><color=orange>";// + city.cost + " days to travel.</color></i>";
	  
	  	// Setup go/go back buttons
	  	GameObject goBtnObj = cityDialog.transform.Find("Action Button").gameObject;
	  	GameObject goBackBtnObj = cityDialog.transform.Find("Go Back").gameObject;
		
		Button goBtn = goBtnObj.GetComponent<Button>();
		Button goBackBtn = goBackBtnObj.GetComponent<Button>();
		
		if(!dialogAnimator.GetBool(dialogOpenID)) {
			cityDialogText.text = strCityTxt;

			dialogAnimator.SetBool(dialogOpenID, true);
		}
		else
		{
			// dialogAnimator.SetTrigger(dialogSwitchID);
			dialogAnimator.SetBool(dialogOpenID, false);
			dialogAnimator.SetBool(dialogCloseID, true);

			yield return new WaitForSeconds(1);

			cityDialogText.text = strCityTxt;
		    
		    goBtn.onClick.RemoveAllListeners();
		    goBackBtn.onClick.RemoveAllListeners();

			dialogAnimator.SetBool(dialogOpenID, true);
			dialogAnimator.SetBool(dialogCloseID, false);

			// yield return null;
			// yield return new WaitForSeconds(2);
		}

		initialDialogScale = cityDialog.transform.localScale;
	    
	    // Show Go button if city is unlocked
	    goBtnObj.SetActive(city.unlocked);
		Text label = goBtn.transform.FindChild("Text").GetComponent<Text>();
		label.text = "Travel to " + city.display_name;
 
 		// Set city context and go to city
	    goBtn.onClick.AddListener(() => DataManager.SetSceneContext(city.symbol));
	    goBtn.onClick.AddListener(() => label.text = "Loading...");
	    // goBtn.onClick.AddListener(() => StartCoroutine( UnlockRoute(city.cost) ));

	    goBackBtn.onClick.AddListener(() => CloseCurrent());

	    // Reset camera position
	    targetCamRotation = initialCamRotation;
	    targetCamPosition = initialCamPosition;

		yield return null;


	}

	// Unlock the route for selected city
	IEnumerator UnlockRoute(int dayCost) {

		daysAllowed = daysAllowed - dayCost; 
		daysLeftText.text = daysAllowed + " Days Left";

		daysLeftPanel.GetComponent<Animator>().enabled = true;

		yield return new WaitForSeconds(1.2f);

		Application.LoadLevel(citySceneName);

	}

 	public void CloseCurrent()
    {
	  	GameObject goBackBtnObj = cityDialog.transform.Find("Go Back").gameObject;
		Button goBackBtn = goBackBtnObj.GetComponent<Button>();

	    goBackBtn.onClick.RemoveAllListeners();

        dialogAnimator.SetBool(dialogOpenID, false);
        // dialogAnimator.SetBool(dialogCloseID, true);
        
    }
    
    private IEnumerator WaitForAnimation()
	{
			
		do
	    {
	        yield return null;
	    } while ( dialogAnimator.GetCurrentAnimatorStateInfo(0).IsName(dialogCloseID) );

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
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MapCity : MB, IClickable {

	public string citySymbol;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public InputLayer[] IgnoreLayers { 
		get { return new InputLayer[] { InputLayer.UI }; } 
	}

 	// On Touch/Click City
	public void OnClick (ClickSettings clickSettings) {

		// Bail if mouse is not directly over city (e.g. on UI)
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		// Get ref to map manager
		MapManager manager = transform.parent.gameObject.GetComponent<MapManager>() as MapManager;

		// Show this city's dialog only if the main camera is not currently in an animation
		if(!manager.CameraIsAnimating())
			StartCoroutine(manager.ShowCityDialog(citySymbol));

	}
}

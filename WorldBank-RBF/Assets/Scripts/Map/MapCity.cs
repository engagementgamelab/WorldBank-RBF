using UnityEngine;
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

		StartCoroutine((transform.parent.gameObject.GetComponent<MapManager>() as MapManager).ShowCityDialog(citySymbol));

	}
}

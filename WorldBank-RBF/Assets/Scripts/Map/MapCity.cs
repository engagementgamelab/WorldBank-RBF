using UnityEngine;
using System.Collections;

public class MapCity : MonoBehaviour {

	public string citySymbol;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

 	// On Touch/Click City
	void OnMouseDown() {

		(transform.parent.gameObject.GetComponent<MapManager>() as MapManager).ShowCityDialog(citySymbol);

	}
}

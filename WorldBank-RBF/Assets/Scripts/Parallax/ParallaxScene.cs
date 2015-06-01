using UnityEngine;
using System.Collections;

public class ParallaxScene : MonoBehaviour {

	public string cityName;

	void Start () {
		CityManager.SetCity (cityName);
	}
}

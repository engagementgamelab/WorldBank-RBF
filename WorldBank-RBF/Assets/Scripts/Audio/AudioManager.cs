using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	Inventory inventory = new Inventory ();

	AmbienceGroup Ambiences {
		get { return (AmbienceGroup)inventory["ambiences"]; }
	}

	void Awake () {
		inventory.Add (new AmbienceGroup ());
		inventory.Add (new SfxGroup ());
		inventory.Add (new MusicGroup ());
	}

	void Start () {
		PlayerData.CityGroup.onUpdateCurrentCity += OnSetCity;
		// Ambiences.Play ("capitol");
	}

	void OnSetCity (string city) {
		// Ambiences.Play (city);
	}
}

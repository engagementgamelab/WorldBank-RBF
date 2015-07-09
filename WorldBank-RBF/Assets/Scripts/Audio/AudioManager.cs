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
		PlayerData.CityGroup.onUpdate += OnSetCity;
		// PlayAmbience ();
	}

	void OnSetCity () {
		Ambiences.Play (PlayerData.CityGroup.CurrentCity);
	}

	/*void PlayAmbience () {
		PlayClip (Ambiences.Get ("capitol"), true);
	}

	void PlayClip (AudioClip clip, bool loop) {
		AudioObject o = ObjectPool.Instantiate<AudioObject> ();
		o.Play (clip, loop);
	}*/
}

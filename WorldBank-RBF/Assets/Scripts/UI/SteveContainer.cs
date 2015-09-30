using UnityEngine;
using System.Collections;

public class SteveContainer : MB {

	void OnEnable () {
		AudioManager.Music.Play ("naive");
	}

	void OnDisable () {
		// AudioManager.Music.Stop ("naive");
		AudioManager.Music.Play ("title_theme");
	}

	void Update () {
		Transform.SetLocalEulerAnglesZ (
			Mathf.Lerp (15f, -15f, Mathf.PingPong (Time.time, 1f)));
	}
}

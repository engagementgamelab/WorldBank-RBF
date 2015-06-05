using UnityEngine;
using System.Collections;

public class CurrentCityIndicator : MB {

	bool pulsing = false;

	void OnEnable () {
		pulsing = true;
		StartCoroutine (CoPulse ());
	}

	void OnDisable () {
		pulsing = false;
	}

	IEnumerator CoPulse () {
		
		while (pulsing) {
			float scale = Mathf.SmoothStep (0.25f, 1f, 0.25f + Mathf.PingPong (Time.time, 0.75f));
			LocalScale = new Vector3 (scale, scale, 1f);
			yield return null;
		}
	}
}

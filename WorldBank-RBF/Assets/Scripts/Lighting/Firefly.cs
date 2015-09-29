using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Light))]
public class Firefly : MonoBehaviour {

	Light light = null;
	Light Light {
		get {
			if (light == null) {
				light = GetComponent<Light> ();
			}
			return light;
		}
	}

	public float minIntensity = 0.1f;
	public float maxIntensity = 2f;
	public float speed = 0.25f;
	public bool randomizeSpeed = true;

	bool pulsing = false;
	float offset;
	float mySpeed;
	float myMaxIntensity;

	void OnEnable () {
		pulsing = true;
		myMaxIntensity = maxIntensity * 2f;
		mySpeed = speed + Random.Range (-0.2f, 0.2f);
		offset = Random.Range (0f, 15f);
		StartCoroutine (CoPulse ());
	}

	void OnDisable () {
		pulsing = false;
	}

	IEnumerator CoPulse () {
		while (pulsing) {
			Light.intensity = Mathf.Lerp (
				minIntensity, 
				myMaxIntensity, 
				Mathf.PingPong ((Time.time+offset)*mySpeed, 1f));
			yield return null;
		}
	}
}

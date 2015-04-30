using UnityEngine;
using System.Collections;

public class NPCHighlight : MB {

	static NPCHighlight instance = null;
	static public NPCHighlight Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (NPCHighlight)) as NPCHighlight;
				if (instance == null) {
					GameObject go = new GameObject ("NPCHighlight");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<NPCHighlight>();
				}
			}
			return instance;
		}
	}

	Light light;
	Light Light {
		get {
			if (light == null) {
				light = GetComponent<Light> ();
			}
			return light;
		}
	}

	Vector3 startPosition;
	float maxIntensity = 3f;

	void Awake () {
		startPosition = Position;
	}

	public void Activate (Vector3 position) {
		Position = position + startPosition;
		Light.enabled = true;
		StartCoroutine (CoFade (0, maxIntensity));
	}

	public void Deactivate () {
		StartCoroutine (CoFade (maxIntensity, 0));
	}

	IEnumerator CoFade (float from, float to) {
		
		float time = 1f;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			Light.intensity = Mathf.Lerp (from, to, progress);
			yield return null;
		}

		if (to == 0) {
			Light.enabled = false;
		}
	}
}

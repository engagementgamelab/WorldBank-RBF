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

	new Light light;
	Light Light {
		get {
			if (light == null) {
				light = GetComponent<Light> ();
			}
			return light;
		}
	}

	Vector3 startPosition;
	const float MIN_INTENSITY = 0f;
	const float MAX_INTENSITY = 5f;

	void Awake () {
		startPosition = Position;
	}

	public void Activate (Vector3 position, float p, float duration) {
		if (p < 1f) return;
		Position = position + startPosition;
		Light.enabled = true;
		StartCoroutine (CoFade (0, MAX_INTENSITY, duration));
	}

	public void Deactivate (float duration) {
		StartCoroutine (CoFade (MAX_INTENSITY, 0, duration));
	}

	IEnumerator CoFade (float from, float to, float duration) {
		
		float eTime = 0f;
	
		while (eTime < duration) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / duration);
			Light.intensity = Mathf.Lerp (from, to, progress);
			yield return null;
		}

		if (to == 0) {
			Light.enabled = false;
		}
	}
}

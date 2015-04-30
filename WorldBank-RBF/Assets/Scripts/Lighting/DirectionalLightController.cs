using UnityEngine;
using System.Collections;

public class DirectionalLightController : MonoBehaviour {

	static DirectionalLightController instance = null;
	static public DirectionalLightController Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (DirectionalLightController)) as DirectionalLightController;
				if (instance == null) {
					GameObject go = new GameObject ("DirectionalLightController");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<DirectionalLightController>();
				}
			}
			return instance;
		}
	}

	public Light inclusive;
	public Light exclusive;
	const float FULL_LIGHT = 1f;
	const float LOW_LIGHT = 0.25f;

	void Start () {
		FocusLayer (LayerController.DepthLayers[1]);
	}

	public void FocusLayer (int layer) {
		inclusive.cullingMask = ~(1 << layer);
		exclusive.cullingMask = 1 << layer;
	}

	public void FadeOut (float duration) {
		StartCoroutine (CoFade (inclusive, FULL_LIGHT, LOW_LIGHT, duration));
	}

	public void FadeIn (float duration) {
		StartCoroutine (CoFade (inclusive, LOW_LIGHT, FULL_LIGHT, duration));
	}

	public void FadeToPercentage (float p, float duration) {
		FadeTo (Mathf.Lerp (FULL_LIGHT, LOW_LIGHT, p), duration);
	}

	public void FadeTo (float to, float duration) {
		StartCoroutine (CoFadeTo (inclusive, to, duration));
	}

	IEnumerator CoFade (Light light, float from, float to, float duration) {

		float eTime = 0f;

		while (eTime < duration) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / duration);
			light.intensity = Mathf.Lerp (from, to, progress);
			yield return null;
		}

		light.intensity = to;
	}

	IEnumerator CoFadeTo (Light light, float to, float duration) {
		
		float eTime = 0f;
		float from = light.intensity;
	
		while (eTime < duration) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / duration);
			light.intensity = Mathf.Lerp (from, to, progress);
			yield return null;
		}

		light.intensity = to;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxLayerLightController : MonoBehaviour {

	static ParallaxLayerLightController instance = null;
	static public ParallaxLayerLightController Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (ParallaxLayerLightController)) as ParallaxLayerLightController;
				if (instance == null) {
					GameObject go = new GameObject ("ParallaxLayerLightController");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<ParallaxLayerLightController>();
				}
			}
			return instance;
		}
	}

	public Light lighting1;
	public Light lighting2;
	bool fading = false;

	const float FULL_LIGHT = 0.8f;
	const float LOW_LIGHT = 0f;

	public float Lighting1Intensity {
		get { return lighting1.intensity; }
		set { lighting1.intensity = Mathf.Lerp (LOW_LIGHT, FULL_LIGHT, value); }
	}

	public float Lighting2Intensity {
		get { return lighting2.intensity; }
		set { lighting2.intensity = Mathf.Lerp (LOW_LIGHT, FULL_LIGHT, value); }
	}

	void Awake () {
		lighting1.intensity = FULL_LIGHT;
		lighting2.intensity = FULL_LIGHT;
	}

	public void FadeOutOtherLayers (GameObject highlight) {
		AssignLayers (highlight);
		FadeOut (lighting1, 1f);
		FadeTo (lighting2, FULL_LIGHT, 1f);
	}

	public void FadeInOtherLayers (GameObject highlight) {
		AssignLayers (highlight);
		FadeIn (lighting1, 1f);
		FadeTo (lighting2, FULL_LIGHT, 1f, () => AssignLayers ());
	}

	public void AssignLayers (GameObject highlight=null) {
		List<ParallaxLayer> layers = ParallaxLayerManager.Instance.Layers;
		foreach (ParallaxLayer layer in layers) {
			layer.gameObject.SetLayerRecursively ("ParallaxLighting1");
		}
		if (highlight != null) highlight.SetLayerRecursively ("ParallaxLighting2");
	}

	void Fade (Light light, float from, float to, float duration, System.Action onEnd=null) {
		StartCoroutine (CoFade (light, from, to, duration, onEnd));
	}

	void FadeOut (Light light, float duration) {
		StartCoroutine (CoFade (light, FULL_LIGHT, LOW_LIGHT, duration));
	}

	void FadeIn (Light light, float duration) {
		StartCoroutine (CoFade (light, LOW_LIGHT, FULL_LIGHT, duration));
	}

	void FadeTo (Light light, float to, float duration, System.Action onEnd=null) {
		StartCoroutine (CoFade (light, light.intensity, to, duration, onEnd));
	}

	void FadeToPercentage (Light light, float p, float duration) {
		FadeTo (light, Mathf.Lerp (FULL_LIGHT, LOW_LIGHT, p), duration);
	}

	IEnumerator CoFade (Light light, float from, float to, float duration, System.Action onEnd=null) {

		if (fading) yield break;
		fading = true;
		float eTime = 0f;

		while (eTime < duration) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / duration);
			light.intensity = Mathf.Lerp (from, to, progress);
			yield return null;
		}

		light.intensity = to;
		if (onEnd != null) onEnd ();
		fading = false;
	}
}

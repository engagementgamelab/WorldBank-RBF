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

	void Start () {
		FocusLayer (9);
	}

	public void FocusLayer (int layer) {
		inclusive.cullingMask = ~(1 << layer);
		exclusive.cullingMask = 1 << layer;
	}

	public void FadeOut (float duration) {
		StartCoroutine (CoFade (inclusive, 1f, 0.25f));
	}

	public void FadeIn (float duration) {
		StartCoroutine (CoFade (inclusive, 0.25f, 1f));
	}

	IEnumerator CoFade (Light light, float from, float to) {

		float time = 1f;
		float eTime = 0f;

		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			light.intensity = Mathf.Lerp (from, to, progress);
			yield return null;
		}

		light.intensity = to;
	}
}

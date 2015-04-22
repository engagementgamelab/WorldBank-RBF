using UnityEngine;
using System.Collections;

public class DirectionalLightController : MonoBehaviour {

	public Light inclusive;
	public Light exclusive;

	void Start () {
		FocusLayer (9);
	}

	public void FocusLayer (int layer) {
		inclusive.cullingMask = ~(1 << layer);
		exclusive.cullingMask = 1 << layer;
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

	void Update () {
		if (Input.GetKeyDown (KeyCode.Q)) {
			StartCoroutine (CoFade (inclusive, 1f, 0.25f));
		}
		if (Input.GetKeyDown (KeyCode.W)) {
			StartCoroutine (CoFade (inclusive, 0.25f, 1f));
		}
	}

	/*float SCurve (float x) {
		return (x < 0.5f)
			? 2 * Mathf.Pow (x, 2)
			: -2 * Mathf.Pow (x, 2) + 4 * x - 1;
	}*/
}

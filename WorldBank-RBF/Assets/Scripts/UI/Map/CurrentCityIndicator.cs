using UnityEngine;
using System.Collections;

public class CurrentCityIndicator : MB {

	bool moving = false;
	public bool Moving {
		get { return moving; }
	}

	bool pulsing = false;

	public void Move (Vector3 to) {
		if (moving) return;
		moving = true;
		StartCoroutine (CoMove (Position, to));
	}

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

	IEnumerator CoMove (Vector3 startPosition, Vector3 endPosition) {
		
		float time = 1f;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			Position = Vector3.Lerp (startPosition, endPosition, progress);
			yield return null;
		}

		moving = false;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurrentCityIndicator : MB {

	bool moving = false;
	public bool Moving {
		get { return moving; }
	}

	bool pulsing = false;

	public void Move (Vector3 to, System.Action onEnd=null) {
		if (moving) return;
		moving = true;
		StartCoroutine (CoMove (Position, to, onEnd));
	}

	public void Move (List<Vector3> positions, System.Action onEnd=null) {
		if (moving) return;
		moving = true;
		StartCoroutine (CoMove (positions, onEnd));
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

	IEnumerator CoMove (Vector3 startPosition, Vector3 endPosition, System.Action onEnd) {
		
		float time = 1f;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			Position = Vector3.Lerp (startPosition, endPosition, progress);
			yield return null;
		}

		moving = false;
		if (onEnd != null) onEnd ();
	}

	IEnumerator CoMove (List<Vector3> positions, System.Action onEnd) {
		
		float time = 2f;
		float eTime = 0f;
		float positionCount = (float)positions.Count-1;
		float interval = time / positionCount;

		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = eTime / time;

			int nextPosition = Mathf.FloorToInt (positionCount * progress);
			int prevPosition = nextPosition > 0 ? nextPosition-1 : 0;
			float p = Mathf.InverseLerp (interval * (float)prevPosition * 0.5f, interval * (float)nextPosition * 0.5f, progress);
			Position = Vector3.Lerp (positions[prevPosition], positions[nextPosition], p);
			yield return null;
		}

		moving = false;
		if (onEnd != null) onEnd ();
	}
}

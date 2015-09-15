using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurrentCityIndicator : MB {

	static bool moving = false;
	public static bool Moving {
		get { return moving; }
	}

	bool pulsing = false;

	public void Move (Transform parent, RouteItem route, System.Action onEnd=null) {
		
		List<Vector3> positions = route.Positions.ConvertAll (x => x); // Clone the list
		bool reverse = route.Terminals.Reverse (PlayerData.CityGroup.CurrentCity);
		float speed = route.Speed;

		Parent = parent;
		if (reverse)
			positions.Reverse ();

		if (moving) return;
		moving = true;
		StartCoroutine (CoMove (positions, speed, onEnd));
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
			float scale = Mathf.SmoothStep (0.1f, 0.5f, 0.1f + Mathf.PingPong (Time.time, 0.75f));
			LocalScale = new Vector3 (scale, scale, 1f);
			yield return null;
		}
	}

	IEnumerator CoMove (List<Vector3> positions, float speed, System.Action onEnd) {
		
		float positionCount = (float)positions.Count-1;
		int index = 0;

		while (index < positionCount) {
			yield return StartCoroutine (CoMove (positions[index], positions[index+1], speed*2f));
			index ++;
			yield return null;
		}

		moving = false;
		if (onEnd != null) onEnd ();
	}

	IEnumerator CoMove (Vector3 fromPoint, Vector3 toPoint, float speed=25f) {
		
		float time = Vector3.Distance (fromPoint, toPoint) / speed;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = eTime / time;
			LocalPosition = Vector3.Lerp (fromPoint, toPoint, progress);
			yield return null;
		}
	}
}

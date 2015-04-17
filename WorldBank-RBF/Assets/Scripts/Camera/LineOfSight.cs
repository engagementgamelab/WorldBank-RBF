#define DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineOfSight : MB {

	int rayCount = 7;
	float width = 4;
	Vector3 direction;
	float distance;

	MainCamera mainCamera = null;
	MainCamera MainCamera {
		get {
			if (mainCamera == null) {
				mainCamera = transform.parent.GetScript<MainCamera> ();
			}
			return mainCamera;
		}
	}

	float[] rayPositions;
	float[] RayPositions {
		get {
			if (rayPositions == null) {
				rayPositions = new float[rayCount];
				for (int i = 0; i < rayCount; i ++) {
					rayPositions[i] = -(width/2) + i * (width/((float)rayCount-1));
				}
			}
			return rayPositions;
		}
	}

	void Awake () {
		float angle = -MainCamera.FOV / 2;
		direction = new Vector3 (
			0,
			Mathf.Sin (angle * Mathf.Deg2Rad),
			Mathf.Cos (angle * Mathf.Deg2Rad)
		);
		StartCoroutine (GetDistance ());
	}

	IEnumerator GetDistance () {
		
		List<Transform> layers = ObjectPool.GetInstances<DepthLayer> ();
		while (layers.Count == 0) {
			layers = ObjectPool.GetInstances<DepthLayer> ();
			yield return null;
		}

		float d = 0;
		for (int i = 0; i < layers.Count; i ++) {
			float z = layers[i].transform.localScale.x;
			if (d < z) d = z;
		}
		distance = d;
	}

	void Update () {
		for (int i = 0; i < rayCount; i ++) {
			Vector3 position = MainCamera.Position;
			position.x += RayPositions[i];
			position.y += 1;
			#if DEBUG
			Debug.DrawRay (position, Transform.TransformDirection (direction) * distance);
			#endif
			if (Physics.Raycast (position, Transform.TransformDirection (direction) * distance)) {
				// raycast hit occurred
			}
		}
	}
}

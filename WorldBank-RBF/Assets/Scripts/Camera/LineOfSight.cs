#define DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineOfSight : MB {

	bool zoomEnabled = true;
	public bool ZoomEnabled {
		get { return zoomEnabled; }
		set { zoomEnabled = value; }
	}

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

	int rayCount = 7;
	float width = 4;
	Vector3 direction;
	float distance;
	bool overForegroundObject = false;

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

		// Set distance to furthest layer
		float d = 0;
		for (int i = 0; i < layers.Count; i ++) {
			float z = layers[i].transform.localScale.x;
			if (d < z) d = z;
		}
		distance = d;
	}

	void Update () {
		if (!ZoomEnabled) return;
		if (CastRaysOnLayer (LayerController.DepthLayers[0]).Count > 0) {
			if (!overForegroundObject) {
				MainCamera.Instance.ZoomTo (0f, 5f);
				overForegroundObject = true;
			}
		} else {
			if (overForegroundObject) {
				MainCamera.Instance.ZoomTo (2f, 5f);
				overForegroundObject = false;
			}
		}
	}

	List<int> CastRaysOnLayer (int layer) {
		List<int> raysHit = new List<int> ();
		for (int i = 0; i < rayCount; i ++) {
			RaycastHit hit;
			Vector3 position = MainCamera.Position;
			position.x += RayPositions[i];
			position.y += 1;
			if (Physics.Raycast (position, Transform.TransformDirection (direction) * distance, out hit, 1 << layer)) {
				// why isn't my layer mask working? :(
				if (hit.transform.gameObject.layer == layer) {
					#if DEBUG
						Debug.DrawRay (position, Transform.TransformDirection (direction) * distance);
					#endif
					raysHit.Add (i);
				}
			}
		}
		return raysHit;
	}
}

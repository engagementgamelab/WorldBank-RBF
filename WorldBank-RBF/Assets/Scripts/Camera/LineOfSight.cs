#undef DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineOfSight : MB {

	public bool zoomEnabled = true;
	public bool ZoomEnabled {
		get { return zoomEnabled; }
		set { zoomEnabled = value; }
	}

	MainCamera mainCamera = null;
	MainCamera MainCamera {
		get {
			if (mainCamera == null) {
				mainCamera = Parent.GetScript<MainCamera> ();
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
	float width = 6f;
	Vector3 direction;
	float distance = 10000f;
	bool overForegroundObject = false;

	void Start () {
		float angle = (-MainCamera.FOV / 2f) + 2f;
		direction = new Vector3 (
			0f,
			Mathf.Sin (angle * Mathf.Deg2Rad),
			Mathf.Cos (angle * Mathf.Deg2Rad)
		);
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

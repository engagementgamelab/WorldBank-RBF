using UnityEngine;
using System.Collections;

public class DepthLayer : MB {

	MainCamera mainCamera;
	MainCamera MainCamera {
		get {
			if (mainCamera == null) {
				mainCamera = GameObject.FindObjectOfType (typeof (MainCamera)) as MainCamera;
			}
			return mainCamera;
		}
	}

	void Awake () {
		if (MainCamera != null) {
			float scale = Mathf.Tan (MainCamera.FOV / 2 * Mathf.Deg2Rad) * Transform.position.z * 2;
			Transform.localScale = new Vector3 (
				scale, scale, 1
			);
		}
	}
}

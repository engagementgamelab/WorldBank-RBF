using UnityEngine;
using System.Collections;

public class DepthLayer : MB {

	MainCamera mainCamera;
	public MainCamera MainCamera {
		get {
			if (mainCamera == null) {
				mainCamera = GameObject.FindObjectOfType (typeof (MainCamera)) as MainCamera;
			}
			return mainCamera;
		}
	}

	void Awake () {
		if (MainCamera != null) {
			SetScale ();
			SetPosition ();
		}
	}

	void SetScale () {
		float scale = Mathf.Tan (MainCamera.FOV / 2 * Mathf.Deg2Rad) * Position.z * 2;
		Transform.localScale = new Vector3 (scale, scale, 1);
	}

	void SetPosition () {
		Vector3 target = MainCamera.ViewportToWorldPoint (new Vector3 (0, 0.5f, Position.z));
		target.x += LocalScale.x / 2;
		Transform.SetPosition (target);
	}
}

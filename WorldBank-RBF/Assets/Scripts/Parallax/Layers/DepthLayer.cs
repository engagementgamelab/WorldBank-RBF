using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
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

	float scale = -1;
	public float Scale {
		get { 
			if (scale == -1) {
				scale = Mathf.Tan (MainCamera.FOV / 2 * Mathf.Deg2Rad) * Position.z * 2;
			}
			return scale; 
		}
	}

	float z = 0;

#if UNITY_EDITOR
	void OnEnable () {
		z = Position.z;
		Init ();
	}

	void OnDisable () {
		Reset ();
		Transform.SetPositionZ (z);
	}
#else
	void Awake () {
		Init ();
	}
#endif

	void Init () {
		if (MainCamera != null) {
			SetScale ();
			SetPosition ();
		}
	}

	void Reset () {
		Transform.Reset ();
	}

	void SetScale () {
		Transform.localScale = new Vector3 (Scale, Scale, 1);
	}

	void SetPosition () {
		Vector3 target = ScreenPositionHandler.ViewportToWorld (new Vector3 (0, 0.5f, Position.z));
		target.x += LocalScale.x / 2;
		Transform.SetPosition (target);
	}
}

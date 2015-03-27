using UnityEngine;
using System.Collections;

public class MainCamera : MB {

	float fov = -1;
	public float FOV {
		get {
			if (fov == -1) {
				fov = Camera.fieldOfView;
			}
			return fov;
		}
	}

	Camera camera;
	Camera Camera {
		get {
			if (camera == null) {
				camera = GetComponent<Camera> ();
			}
			return camera;
		}
	}
}

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

	public float Scale {
		get { return Mathf.Tan (Camera.main.fieldOfView / 2 * Mathf.Deg2Rad) * Position.z * 2;}
	}

	int index = 0;
	public int Index {
		get { return index; }
	}

	public LayerBackground background;

	public void Init (int index, float distanceBetweenLayers) {
		background.Init ();
		this.index = index;
		UpdatePosition (distanceBetweenLayers);
	}

	public void UpdatePosition (float distanceBetweenLayers) {
		Transform.SetPositionZ ((index+1) * distanceBetweenLayers);
		SetScale ();
		SetPosition ();
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

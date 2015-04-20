using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DepthLayer : MB, IEditorPoolable {

	LayerSettings layerSettings;
	public LayerSettings LayerSettings {
		get { return layerSettings; }
		set { 
			layerSettings = value;
			layerSettings.onUpdateSettings += OnUpdateSettings;
			layerSettings.onAddImage += OnAddImage;
			layerSettings.onRemoveImage += OnRemoveImage;
			OnUpdateSettings ();
		}
	}

	public int index;
	public int Index { 
		get { return index; }
		set { 
			index = value;
			SetLayer ();
		}
	}

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

	public static float layerSeparation = 20;
	float localSeparation = 0;
	public float LocalSeparation {
		get { return localSeparation; }
		set {
			float separationConstraint = layerSeparation - 1;
			localSeparation = Mathf.Clamp (value, -separationConstraint, separationConstraint);
			UpdatePosition ();
		}
	}

	public LayerBackground background;
	List<LayerImage> Images {
		get { return background.Images; }
		set { background.Images = value; }
	}

	public void Init () {
		background = CreateChildIfNoneExists<LayerBackground> ();
		background.Init ();
		UpdatePosition ();
	}

	public void UpdatePosition () {
		Transform.SetPositionZ ((Index+1) * layerSeparation + LocalSeparation);
		SetScale ();
		SetPosition ();
	}

	void SetLayer () {
		gameObject.layer = Index + 8;
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

	void OnUpdateSettings () {
		LocalSeparation = layerSettings.LocalSeparation;
		background.ImageSettings = layerSettings.ImageSettings;
		Images = layerSettings.Images;
	}

	void OnAddImage () {
		background.CreateImage ();
	}

	void OnRemoveImage () {
		background.RemoveImage ();
	}
}

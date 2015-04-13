using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DepthLayer : MB, IEditorPoolable {

	public int index;
	public int Index { 
		get { return index; }
		set { index = value; }
	}

	static float layerSeparation = 20;
	public static float LayerSeparation {
		get { return layerSeparation; }
		set { layerSeparation = value; }
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

	float localSeparation = 0;
	public float LocalSeparation {
		get { return localSeparation; }
		set {
			float separationConstraint = DepthLayer.LayerSeparation - 1;
			localSeparation = Mathf.Clamp (value, -separationConstraint, separationConstraint);
			UpdatePosition ();
		}
	}

	public LayerBackground background;
	public Texture2D BackgroundTexture {
		get { return background.Texture; }
		set { background.Texture = value; }
	}

	public List<Texture2D> BackgroundTextures {
		get { return background.Textures; }
		set { background.Textures = value; }
	}

	public void Init () {
		background = CreateChildIfNoneExists<LayerBackground> ();
		background.Init ();
		UpdatePosition ();
	}

	public void UpdatePosition () {
		Transform.SetPositionZ ((Index+1) * LayerSeparation + LocalSeparation);
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

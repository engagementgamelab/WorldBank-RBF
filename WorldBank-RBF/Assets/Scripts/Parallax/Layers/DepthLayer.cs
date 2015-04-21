﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DepthLayer : MB, IEditorPoolable {

	[SerializeField, HideInInspector] bool selected;
	public bool Selected { 
		get { return selected; }
		set { selected = value; }
	}

	LayerSettings layerSettings;
	public LayerSettings LayerSettings {
		get { return layerSettings; }
		set { 
			layerSettings = value;
			Index = layerSettings.Index;
			LocalSeparation = layerSettings.LocalSeparation;
			background.ImageSettings = layerSettings.ImageSettings;
		}
	}

	public int index;
	public int Index { 
		get { return index; }
		set { 
			index = value;
			gameObject.layer = index + 8;
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
	public List<LayerImage> Images {
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

	public void AddImage () {
		background.CreateImage ();
	}

	public void RemoveImage () {
		background.RemoveImage ();
	}

	void UpdateLayerSettings () {
		if (LayerSettings == null) return;
		LayerSettings.Index = Index;
		LayerSettings.LocalSeparation = LocalSeparation;
		LayerSettings.ImageSettings = background.ImageSettings;
	}
}

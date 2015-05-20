using UnityEngine;
using UnityEditor;
using System.Collections;

/**
  * Inheritance structure:
  *	ParallaxImage
  *		> ParallaxElement
  *			> ParallaxNpc
  *			> ParallaxForeground
  */

[RequireComponent (typeof (MeshRenderer), typeof (MeshFilter))]
[JsonSerializable (typeof (Models.ParallaxImage))]
public class ParallaxImage : MB, IEditorPoolable, IEditorRefreshable {

	[HideInInspector] public int index;
	public int Index {
		get { return index; }
		set { index = value; }
	}

	int layerPosition = 3000;
	public int LayerPosition {
		get { return layerPosition; }
		set {
			
			layerPosition = 4000 - (int)value;

			// Prevents z fighting
			if (Material != null) {
				Material.renderQueue = layerPosition;
			}
		}
	}

	protected virtual bool Forward {
		get { return false; }
	}

	#if UNITY_EDITOR
	public string TexturePath {
		get { return AssetDatabase.GetAssetPath (Texture); }
		set { 
			Texture = AssetDatabase.LoadAssetAtPath (value, typeof (Texture2D)) as Texture2D;
			texture = Texture;
		}
	}
	#endif

	[ExposeInWindow, HideInInspector] public Texture2D texture = null;
	Texture2D cachedTexture = null;
	public Texture2D Texture {
		get { return cachedTexture; }
		set { 
			if (cachedTexture == value) return;
			cachedTexture = value;
			if (cachedTexture != null) {
				Material = MaterialsManager.CreateMaterialFromTexture (cachedTexture, cachedTexture.format.HasAlpha ());
				if (MaterialsManager.TextureIsBlank (Texture)) {
					gameObject.SetActive (false);
				} else {
					gameObject.SetActive (true);
				}
			} else {
				Material = MaterialsManager.Blank;
			}
			if (Material != null)
				Material.renderQueue = LayerPosition;
		}
	}

	Material Material {
		get { return MeshRenderer.sharedMaterial; }
		set { MeshRenderer.sharedMaterial = value; }
	}

	MeshRenderer meshRenderer = null;
	MeshRenderer MeshRenderer {
		get {
			if (meshRenderer == null) {
				meshRenderer = transform.GetComponent<MeshRenderer> ();
			}
			return meshRenderer;
		}
	}

	public float LocalPositionX {
		get { return LocalPosition.x; }
		set { Transform.SetLocalPositionX (value); }
	}

	public void Init () {
		Texture = null;
		Reset ();
	}

	public void Refresh () {
		Texture = texture;
		Reset ();
	}

	void Reset () {
		Transform.Reset ();
		if (Forward) Transform.SetLocalPositionZ (-0.01f);
	}
}

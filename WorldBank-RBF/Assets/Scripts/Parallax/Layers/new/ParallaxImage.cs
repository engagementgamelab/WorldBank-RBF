using UnityEngine;
using UnityEditor;
using System.Collections;

[RequireComponent (typeof (MeshRenderer), typeof (MeshFilter))]
[JsonSerializable (typeof (Models.ParallaxImage))]
public class ParallaxImage : MB, IEditorPoolable, IEditorRefreshable {

	[HideInInspector] public int index;
	public int Index {
		get { return index; }
		set { index = value; }
	}

	#if UNITY_EDITOR
	public string TexturePath {
		get { return AssetDatabase.GetAssetPath (Texture); }
		set { Texture = AssetDatabase.LoadAssetAtPath (value, typeof (Texture2D)) as Texture2D; }
	}
	#endif

	[WindowExposed] public Texture2D texture = null;
	Texture2D cachedTexture = null;
	public Texture2D Texture {
		get { return cachedTexture; }
		set { 
			if (cachedTexture == value) return;
			cachedTexture = value;
			if (cachedTexture != null) {
				Material = MaterialsManager.CreateMaterialFromTexture (cachedTexture, cachedTexture.format.HasAlpha ());
			} else {
				Material = MaterialsManager.Blank;
			}
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
		Transform.Reset ();
	}

	public void Refresh () {
		Texture = texture;
		Transform.Reset ();
	}
}

using UnityEngine;
using System.Collections;

// TODO: set render queue to prevent z fighting

[RequireComponent (typeof (BoxCollider), typeof (MeshRenderer), typeof (MeshFilter))]
public class QuadImage : MB, IEditorPoolable {

	[SerializeField] public int index;
	public int Index { 
		get { return index; }
		set { index = value; }
	}

	[SerializeField, HideInInspector] Texture2D texture;
	public Texture2D Texture {
		get { return texture; }
		set { 
			texture = value;
			if (texture != null)
				Material = MaterialsManager.CreateMaterialFromTexture (texture, texture.format.HasAlpha ());
			OnSetTexture ();
		}
	}

	[SerializeField, HideInInspector] bool colliderEnabled = false;	
	public bool ColliderEnabled {
		get { return BoxCollider.enabled; }
		set { 
			colliderEnabled = value;
			BoxCollider.enabled = value; 
		}
	}

	[SerializeField, HideInInspector] float colliderWidth = 1;
	public float ColliderWidth {
		get { return colliderWidth; }
		set { 
			colliderWidth = value;
			BoxCollider.SetSizeX (colliderWidth);
		}
	}

	[SerializeField, HideInInspector] float colliderCenter = 0;
	public float ColliderCenter {
		get { return colliderCenter; }
		set {
			colliderCenter = value;
			BoxCollider.SetCenterX (colliderCenter);
		}
	}

	protected Material Material {
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
		set { meshRenderer = value; }
	}

	BoxCollider boxCollider = null;
	protected BoxCollider BoxCollider {
		get {
			if (boxCollider == null) {
				boxCollider = GetComponent<BoxCollider> ();
			}
			return boxCollider;
		}
	}

	public virtual void Init () {}
	protected virtual void OnSetTexture () {}
}

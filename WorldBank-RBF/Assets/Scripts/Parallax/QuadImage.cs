using UnityEngine;
using System.Collections;

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
		get { return colliderEnabled; }
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

	[SerializeField, HideInInspector] float colliderHeight = 1;
	public float ColliderHeight {
		get { return colliderHeight; }
		set {
			colliderHeight = value;
			BoxCollider.SetSizeY (colliderHeight);
		}
	}

	[SerializeField, HideInInspector] float colliderCenterX = 0;
	public float ColliderCenterX {
		get { return colliderCenterX; }
		set {
			colliderCenterX = value;
			BoxCollider.SetCenterX (colliderCenterX);
		}
	}

	[SerializeField, HideInInspector] float colliderCenterY = 0;
	public float ColliderCenterY {
		get { return colliderCenterY; }
		set {
			colliderCenterY = value;
			BoxCollider.SetCenterY (colliderCenterY);
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

	protected float XOffset {
		get { return -BoxCollider.center.x; }
	}

	public virtual void Init () {}
	protected virtual void OnSetTexture () {}
}

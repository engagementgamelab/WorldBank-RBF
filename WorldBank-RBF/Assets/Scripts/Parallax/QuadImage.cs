using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider), typeof (MeshRenderer), typeof (MeshFilter))]
public class QuadImage : MB, IEditorPoolable {

	public int Index { get; set; }

	Texture2D texture;
	public Texture2D Texture {
		get { return texture; }
		set { 
			texture = value;
			if (texture != null)
				Material = MaterialsManager.CreateMaterialFromTexture (texture, texture.format.HasAlpha ());
			OnSetTexture ();
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
	BoxCollider BoxCollider {
		get {
			if (boxCollider == null) {
				boxCollider = GetComponent<BoxCollider> ();
			}
			return boxCollider;
		}
	}

	float colliderWidth = 1;
	public float ColliderWidth {
		get { return colliderWidth; }
		set { 
			colliderWidth = value;
			BoxCollider.SetSizeX (colliderWidth);
		}
	}

	float colliderCenter = 0;
	public float ColliderCenter {
		get { return colliderCenter; }
		set {
			colliderCenter = value;
			BoxCollider.SetCenterX (colliderCenter);
		}
	}

	bool ColliderEnabled {
		get { return BoxCollider.enabled; }
		set { BoxCollider.enabled = value; }
	}

	public virtual void Init () {}
	protected virtual void OnSetTexture () {}
}

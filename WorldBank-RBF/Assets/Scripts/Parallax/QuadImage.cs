using UnityEngine;
using System.Collections;

public class QuadImage : MB, IPoolable {

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

	bool ColliderEnabled {
		get { return BoxCollider.enabled; }
		set { BoxCollider.enabled = value; }
	}

	public void Init (Material material, bool colliderEnabled=false) {
		MeshRenderer.material = material;
		ColliderEnabled = colliderEnabled;
	}

	public void OnCreate () {}
	public void OnDestroy () {}
}

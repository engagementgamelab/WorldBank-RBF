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

	public void Init (Material material) {
		MeshRenderer.material = material;
	}

	public void OnCreate () {}
	public void OnDestroy () {}
}

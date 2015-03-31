using UnityEngine;
using System.Collections;

public class LayerLight : LayerComponent {

	new Light light = null;
	Light Light {
		get {
			if (light == null) {
				light = GetComponent<Light> ();
			}
			return light;
		}
	}

	protected override void Awake () {
		base.Awake ();
		Light.cullingMask = 1 << Layer;
	}
}

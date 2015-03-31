using UnityEngine;
using System.Collections;

public class LayerComponent : MB {

	DepthLayer depthLayer = null;
	protected DepthLayer DepthLayer {
		get {
			if (depthLayer == null) {
				depthLayer = Transform.parent.GetScript<DepthLayer> ();
			}
			return depthLayer;
		}
	}

	MainCamera mainCamera = null;
	protected MainCamera MainCamera {
		get {
			if (mainCamera == null) {
				mainCamera = DepthLayer.MainCamera;
			}
			return mainCamera;
		}
	}

	int layer = -1;
	protected int Layer {
		get {
			if (layer == -1) {
				layer = DepthLayer.gameObject.layer;
			}
			return layer;
		}
	}

	protected virtual void Awake () {
		gameObject.layer = Layer;
	}
}

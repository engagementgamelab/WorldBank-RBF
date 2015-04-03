using UnityEngine;
using System.Collections;

public class LayerManager : MonoBehaviour {

	public DepthLayer[] layers;

	static LayerManager instance = null;
	public static LayerManager Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType (typeof (LayerManager)) as LayerManager;
			}
			return instance;
		}
	}

	public float distance = -1;
	public float Distance {
		get {
			if (distance == -1) {
				float d = 0;
				for (int i = 0; i < layers.Length; i ++) {
					float z = layers[i].Scale;
					if (d < z) d = z;
				}
				distance = d;
			}
			return distance;
		}
	}
}

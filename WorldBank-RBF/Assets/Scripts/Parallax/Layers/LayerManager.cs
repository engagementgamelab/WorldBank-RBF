using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerManager : MB {

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
				for (int i = 0; i < layers.Count; i ++) {
					float z = layers[i].Scale;
					if (d < z) d = z;
				}
				distance = d;
			}
			return distance;
		}
	}

	List<DepthLayer> layers = new List<DepthLayer> ();
	float distanceBetweenLayers = 20;

	public List<DepthLayer> SetLayerCount (int newCount, List<DepthLayer> oldLayers) {
		
		// TODO: LayerManager doesn't rememer DepthLayers between playmodes
		if (oldLayers != null) {
			layers = oldLayers;
		}

		for (int i = 0; i < layers.Count; i ++) {
			if (layers[i] == null) continue;
			ObjectPool.Destroy<DepthLayer> (layers[i].Transform);
		}
		layers.Clear ();

		for (int i = 0; i < newCount; i ++) {
			CreateDepthLayer (i);
		}
		return layers;
	}

	public void SetDistanceBetweenLayers (float distance) {
		distanceBetweenLayers = distance;
		for (int i = 0; i < layers.Count; i ++) {
			layers[i].UpdatePosition (distanceBetweenLayers);
		}
	}

	void CreateDepthLayer (int index) {
		DepthLayer depthLayer = ObjectPool.Instantiate<DepthLayer> ();
		layers.Add (depthLayer);
		depthLayer.Transform.SetParent (Transform);
		depthLayer.Init (index, distanceBetweenLayers);
	}
}

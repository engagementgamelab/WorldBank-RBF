﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[JsonSerializable (typeof (Models.Scene))]
public class ParallaxLayerManager : MonoBehaviour {

	static ParallaxLayerManager instance = null;
	static public ParallaxLayerManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (ParallaxLayerManager)) as ParallaxLayerManager;
				if (instance == null) {
					GameObject go = new GameObject ("ParallaxLayerManager");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<ParallaxLayerManager>();
				}
			}
			return instance;
		}
	}

	[ExposeInWindow]
	public string cityName;

	[HideInInspector]
	public List<ParallaxLayer> layers = new List<ParallaxLayer> ();
	public List<ParallaxLayer> Layers {
		get { return layers; }
	}

	[SerializeField] int layerCount = 1;
	[ExposeInWindow, ExposeProperty]
	public int LayerCount {
		get { return layerCount; }
		set { 
			layerCount = Mathf.Clamp (value, 1, 10);
			RefreshLayers ();
		}
	}

	[SerializeField] float cameraStartPosition = 0f;
	[ExposeInWindow, ExposeProperty]
	public float CameraStartPosition {
		get { return cameraStartPosition; }
		set {
			cameraStartPosition = value;
			MainCamera.Instance.Positioner.XPosition = cameraStartPosition;
		}
	}

	public ParallaxLayer FurthestLayer {
		get { 
			if (layers == null || layers.Count == 0) {
				return null;
			}
			return layers[layers.Count-1]; 
		}
	}

	public void Load (string symbol) {
		Clear ();
		ModelSerializer.Load (this, 
			Application.dataPath + "/Resources/Config/PhaseOne/Cities/" + symbol + ".json");

		// TODO: ModelSerializer *should be* setting the layers list, but isn't for some reason
		// Also- cities would load faster if existing layers were updated rather than destroyed & instantiated
		ParallaxLayer[] pLayers = GameObject.FindObjectsOfType (typeof (ParallaxLayer)) as ParallaxLayer[];
		layers = pLayers.ToList ();
    }

    void Clear () {
		foreach (ParallaxLayer layer in layers)
			layer.Destroy ();
		layers.Clear ();
    }

	void RefreshLayers () {
        
        layers = EditorObjectPool.GetObjectsOfTypeInOrder<ParallaxLayer> ();

        // Remove layers
        if (layers.Count > layerCount) {
            int remove = layers.Count - layerCount;
            int removed = 0;
            while (removed < remove) {
                EditorObjectPool.Destroy<ParallaxLayer> ();
                removed ++;
            }
            layers.RemoveRange (layerCount, remove);
        }

        // Add layers
        while (layers.Count < layerCount) {
            layers.Add (EditorObjectPool.Create<ParallaxLayer> () as ParallaxLayer);
        }
    }


    #if UNITY_EDITOR
	public void Reset () {
		cityName = "";
	}
	#endif
}

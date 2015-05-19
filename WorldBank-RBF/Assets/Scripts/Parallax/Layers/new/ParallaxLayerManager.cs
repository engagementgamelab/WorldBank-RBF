﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	[WindowExposed]
	public string cityName;

	[HideInInspector]
	public List<ParallaxLayer> layers = new List<ParallaxLayer> ();

	[SerializeField] int layerCount = 3;
	[WindowExposed, ExposeProperty]
	public int LayerCount {
		get { return layerCount; }
		set { 
			layerCount = Mathf.Clamp (value, 0, 10);
			RefreshLayers ();
		}
	}

	public ParallaxLayer FurthestLayer {
		get { return layers[layers.Count-1]; }
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

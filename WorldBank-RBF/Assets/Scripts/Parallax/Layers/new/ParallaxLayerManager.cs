using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[JsonSerializable (typeof (Models.Scene))]
public class ParallaxLayerManager : MonoBehaviour {

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

        // Apply settings
        /*for (int i = 0; i < layerCount; i ++) {
            ParallaxLayer layer = layers[i];
            layer.LayerSettings = layerSettings[layer.Index];
        }  */
    }
}

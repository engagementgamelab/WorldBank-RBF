using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class LayerOptions : ScriptableObject {

	DepthLayer layer = null;
	int layerIndex = 0;

	[Range (1, 20)]
	int tiles = 1;
	int prevTiles = 1;

	public void SetLayer (DepthLayer layer) {
		this.layer = layer;
		layerIndex = layer.Index + 1;
	}

	public void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
    }

    public void OnGUI () {
    	if (layer == null) {
    		GUILayout.Label ("No Layer Selected");
    		return;
    	}
    	GUILayout.Label ("Layer " + layerIndex);
    	tiles = EditorGUILayout.IntSlider ("Background tiles", tiles, 1, 20);
    	if (tiles != prevTiles) {
    		layer.background.TileCount = tiles;
    		prevTiles = tiles;
    	}
    }
}

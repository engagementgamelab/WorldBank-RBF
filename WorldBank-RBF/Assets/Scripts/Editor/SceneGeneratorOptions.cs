using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
 
[Serializable]
public class SceneGeneratorOptions : ScriptableObject {
	
    float space = 10;

    [Range (1, 6)]
    int layerCount = 4;
    int prevLayerCount = 4;

    [Range (2, 20)]
    int width = 2;
    int prevWidth = 2;

    List<DepthLayer> layers;
    LayerOptions layerOptions;
    int selectedLayer = -1;

    public void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
        if (layerOptions == null) {
            layerOptions = CreateInstance<LayerOptions> () as LayerOptions;
        }
        if (layers == null) {
            layers = new List<DepthLayer> ();
        }
    }
 
    public void OnGUI () {

        if (GUILayout.Button ("Refresh")) {
            layers = EditorObjectPool.Create<DepthLayer> (layerCount).ConvertAll (x => x.GetScript<DepthLayer> ());
        }

        layerCount = EditorGUILayout.IntSlider ("Layer Count", layerCount, 1, 6);
        if (layerCount != prevLayerCount) {
            layers = EditorObjectPool.Create<DepthLayer> (layerCount).ConvertAll (x => x.GetScript<DepthLayer> ());
            prevLayerCount = layerCount;
        }

        width = EditorGUILayout.IntSlider ("Width", width, 2, 20);
        if (width != prevWidth) {
            foreach (DepthLayer layer in layers) {
                layer.background.TileCount = width;
            }
            prevWidth = width;
        }

        GUILayout.Space (space);
        GUILayout.Label ("Select a layer to edit", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal ();
        for (int i = 0; i < layers.Count; i ++) {
            DepthLayer layer = layers[i];
            if (layer == null) continue;
            int layerIndex = layer.Index + 1;
            if (selectedLayer == i) {
                GUI.color = Color.green;
            } else {
                GUI.color = Color.white;
            }
            if (GUILayout.Button ("Layer " + layerIndex)) {
                layerOptions.SetLayer (layer);
                selectedLayer = i;
            }
        }
        EditorGUILayout.EndHorizontal ();
        GUILayout.Space (space);

        layerOptions.OnGUI ();
    }
}
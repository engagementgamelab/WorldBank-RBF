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

    [Range (5, 100)]
    float distanceBetweenLayers = 20;
    float prevDistanceBetweenLayers = 20;

    List<DepthLayer> layers;// = new List<DepthLayer> ();
    LayerOptions layerOptions;

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

        layerCount = EditorGUILayout.IntSlider ("Layer Count", layerCount, 1, 6);
        if (layerCount != prevLayerCount) {
            layers = LayerManager.Instance.SetLayerCount (layerCount, layers);
            prevLayerCount = layerCount;
        }

        distanceBetweenLayers = EditorGUILayout.Slider ("Distance Between Layers", distanceBetweenLayers, 5, 100);
        if (distanceBetweenLayers != prevDistanceBetweenLayers) {
            LayerManager.Instance.SetDistanceBetweenLayers (distanceBetweenLayers);
            prevDistanceBetweenLayers = distanceBetweenLayers;
        }

        GUILayout.Space (space);
        GUILayout.Label ("Select a layer to edit", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal ();
        for (int i = 0; i < layers.Count; i ++) {
            DepthLayer layer = layers[i];
            if (layer == null) continue;
            int layerIndex = layer.Index + 1;
            if (GUILayout.Button ("Layer " + layerIndex)) {
                layerOptions.SetLayer (layer);
            }
        }
        EditorGUILayout.EndHorizontal ();
        GUILayout.Space (space);

        layerOptions.OnGUI ();
    }
}
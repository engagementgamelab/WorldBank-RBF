using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
 
[Serializable]
public class ParallaxSceneDesignerOptions : ScriptableObject {
	
    float space = 10;

    [Range (1, 6)]
    int layerCount = 4;
    int prevLayerCount = 4;

    List<DepthLayer> layers;
    LayerOptions layerOptions;
    SaveLoadOptions saveLoadOptions;
    int selectedLayer = -1;

    List<LayerSettings> layerSettings = new List<LayerSettings> ();    

    GUILayoutOption largeButtonHeight = GUILayout.Height (30f);

    public void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
        if (layerOptions == null) {
            layerOptions = CreateInstance<LayerOptions> () as LayerOptions;
        }
        if (saveLoadOptions == null) {
            saveLoadOptions = CreateInstance<SaveLoadOptions> () as SaveLoadOptions;
            saveLoadOptions.Init (this);
        }
        if (layers == null) {
            layers = new List<DepthLayer> ();
        }
    }

    public void OnGUI () {

        EditorGUILayout.BeginHorizontal ();
        GUI.color = Color.green;
        if (GUILayout.Button ("Refresh", largeButtonHeight)) {
            Refresh ();
        }

        GUI.color = Color.yellow;
        if (GUILayout.Button ("Clean up", largeButtonHeight)) {
            EditorObjectPool.CleanUp ();
        }

        GUI.color = Color.red;
        if (GUILayout.Button ("Clear", largeButtonHeight)) {
            Clear ();
        }
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.Separator ();
        saveLoadOptions.OnGUI ();
        EditorGUILayout.Separator ();

        layerCount = EditorGUILayout.IntSlider ("Layer Count", layerCount, 1, 6);
        if (layerCount != prevLayerCount) {
            Refresh ();
            prevLayerCount = layerCount;
        }

        GUI.color = Color.white;
        GUILayout.Space (space);
        GUILayout.Label ("Select a layer to edit", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal ();
        for (int i = 0; i < layerCount; i ++) {
            DepthLayer layer = layers[i];
            if (layer == null) continue;
            int layerIndex = layer.Index+1;
            if (selectedLayer == i) {
                GUI.color = Color.gray;
            } else {
                GUI.color = Color.white;
            }
            if (GUILayout.Button ("Layer " + layerIndex)) {

                // Unselect previously selected layer, then select this one
                if (selectedLayer != -1) {
                    layers[selectedLayer].Selected = false;
                }
                layer.Selected = true;
                selectedLayer = i;
                layerOptions.SetDepthLayer (layer);
                Selection.activeGameObject = layer.gameObject;
                GUI.FocusControl (null);
            }
        }
        EditorGUILayout.EndHorizontal ();

        layerOptions.OnGUI ();
    }

    public void Load (List<LayerSettingsJson> layers, int layerCount) {
        EditorObjectPool.Clear ();
        this.layerCount = layerCount;
        for (int i = 0; i < layers.Count; i ++) {
            LayerSettingsJson layer = layers[i];
            LayerSettings settings = EditorObjectPool.Create<LayerSettings> ();
            settings.Init (layer.GetIndex (), layer.GetLocalSeparation (), layer.GetImages ());
        }
        Refresh ();
    }

    void Clear () {
        EditorObjectPool.Clear ();
        saveLoadOptions.LayerSettings = null;
        saveLoadOptions.LayerCount = 0;
    }

    public void Refresh () {
        CreateLayerSettings ();
        RefreshLayers ();
        SetSelectedLayer ();
        saveLoadOptions.LayerSettings = layerSettings;
        saveLoadOptions.LayerCount = layerCount;
    }

    void CreateLayerSettings () {
        layerSettings = EditorObjectPool.GetObjectsOfTypeInOrder<LayerSettings> ();
        while (layerSettings.Count < layerCount) {
            LayerSettings settings = EditorObjectPool.Create<LayerSettings> ();
            layerSettings.Add (settings);
            settings.Init (layerSettings.Count-1);
        }
    }

    void RefreshLayers () {
        
        layers = EditorObjectPool.GetObjectsOfTypeInOrder<DepthLayer> ();

        // Remove layers
        if (layers.Count > layerCount) {
            int remove = layers.Count - layerCount;
            int removed = 0;
            while (removed < remove) {
                EditorObjectPool.Destroy<DepthLayer> ();
                removed ++;
            }
            layers.RemoveRange (layerCount, remove);
        }

        // Add layers
        while (layers.Count < layerCount) {
            layers.Add (EditorObjectPool.Create<DepthLayer> () as DepthLayer);
        }

        // Apply settings
        for (int i = 0; i < layerCount; i ++) {
            DepthLayer layer = layers[i];
            layer.LayerSettings = layerSettings[layer.Index];
        }  
    }

    void SetSelectedLayer () {
        selectedLayer = -1;
        for (int i = 0; i < layerCount; i ++) {
            DepthLayer layer = layers[i];
            if (layer.Selected) {
                selectedLayer = i;
                layerOptions.SetDepthLayer (layer);
            }
        }
    }
}
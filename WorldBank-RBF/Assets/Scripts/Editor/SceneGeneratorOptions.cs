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

    List<DepthLayer> layers;
    LayerOptions layerOptions;
    int selectedLayer = -1;

    List<LayerSettings> layerSettings = new List<LayerSettings> ();    

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

        EditorGUILayout.BeginHorizontal ();
        GUI.color = Color.green;
        if (GUILayout.Button ("Refresh")) {
            Refresh ();
        }

        GUI.color = Color.yellow;
        if (GUILayout.Button ("Clean up")) {
            EditorObjectPool.CleanUp ();
        }

        GUI.color = Color.red;
        if (GUILayout.Button ("Clear")) {
            EditorObjectPool.Clear ();
        }
        EditorGUILayout.EndHorizontal ();

        GUI.color = Color.white;
        layerCount = EditorGUILayout.IntSlider ("Layer Count", layerCount, 1, 6);
        if (layerCount != prevLayerCount) {
            Refresh ();
            prevLayerCount = layerCount;
        }

        GUILayout.Space (space);
        GUILayout.Label ("Select a layer to edit", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal ();
        for (int i = 0; i < layerCount; i ++) {
            LayerSettings settings = layerSettings[i];
            if (settings == null) continue;
            int layerIndex = settings.Index+1;
            if (selectedLayer == i) {
                GUI.color = Color.green;
            } else {
                GUI.color = Color.white;
            }
            if (GUILayout.Button ("Layer " + layerIndex)) {

                // Unselect previously selected layer, then select this one
                if (selectedLayer != -1) {
                    layerSettings[selectedLayer].Selected = false;
                }
                settings.Selected = true;
                selectedLayer = i;
                layerOptions.SetLayerSettings (settings);
            }
        }
        EditorGUILayout.EndHorizontal ();

        layerOptions.OnGUI ();
    }

    public void Refresh () {
        CreateLayerSettings ();
        RefreshLayers ();
        SetSelectedLayer ();
    }

    void CreateLayerSettings () {
        layerSettings = ObjectPool.GetInstances<LayerSettings> ().ConvertAll (x => x.GetScript<LayerSettings> ());
        layerSettings = OrderLayerSettings ();
        while (layerSettings.Count < layerCount) {
            LayerSettings settings = ObjectPool.Instantiate<LayerSettings> ();
            layerSettings.Add (settings);
            settings.Init (layerSettings.Count-1);
        }
    }

    void RefreshLayers () {
        layers = EditorObjectPool.Create<DepthLayer> (layerCount).ConvertAll (x => x.GetScript<DepthLayer> ());
        for (int i = 0; i < layerCount; i ++) {
            DepthLayer layer = layers[i];
            layer.LayerSettings = layerSettings[layer.Index];
        }        
    }

    void SetSelectedLayer () {
        selectedLayer = -1;
        for (int i = 0; i < layerCount; i ++) {
            LayerSettings settings = layerSettings[i];
            if (settings.Selected) {
                selectedLayer = i;
                layerOptions.SetLayerSettings (settings);
            }
        }
    }

    List<LayerSettings> OrderLayerSettings () {
        List<LayerSettings> orderedSettings = new List<LayerSettings> (new LayerSettings[layerSettings.Count]);
        for (int i = 0; i < layerSettings.Count; i ++) {
            orderedSettings[layerSettings[i].Index] = layerSettings[i];
        }
        return orderedSettings;
    }
}
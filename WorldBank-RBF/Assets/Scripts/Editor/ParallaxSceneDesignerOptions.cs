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
            EditorObjectPool.Clear ();
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
            if (layerSettings.Count <= i) continue;
            LayerSettings settings = layerSettings[i];
            if (settings == null) continue;
            int layerIndex = settings.Index+1;
            if (selectedLayer == i) {
                GUI.color = Color.gray;
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
                Selection.activeGameObject = layers[i].gameObject;
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
            LayerSettings settings = ObjectPool.Instantiate<LayerSettings> ();
            settings.Init (layer.GetIndex (), layer.GetLocalSeparation (), layer.GetImages ());
        }
        Refresh ();
    }

    /*List<Texture2D> LoadTextures (List<string> directories) {
        List<Texture2D> textures = new List<Texture2D> ();
        for (int i = 0; i < directories.Count; i ++) {
            Texture2D texture = AssetDatabase.LoadAssetAtPath (directories[i], typeof (Texture2D)) as Texture2D;
            textures.Add (texture);
        }
        return textures;
    }*/

    public void Refresh () {
        CreateLayerSettings ();
        RefreshLayers ();
        SetSelectedLayer ();
        saveLoadOptions.LayerSettings = layerSettings;
        saveLoadOptions.LayerCount = layerCount;
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
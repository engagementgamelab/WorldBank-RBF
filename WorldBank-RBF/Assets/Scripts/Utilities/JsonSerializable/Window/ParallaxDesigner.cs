// Run only if inside editor
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class ParallaxDesigner : EditorWindow {

    string TEXTURES_PATH { get { return Application.dataPath + "/Textures/Cities/"; } }
    string PATH { get { return Application.dataPath + "/Resources/Config/PhaseOne/Cities/"; } }
    string savePath = "";
    string fileName = "";

    EditorObjectDrawer<ParallaxLayerManager> objectDrawer = 
        new EditorObjectDrawer<ParallaxLayerManager> ();

    ParallaxLayerDesigner layerDesigner;
    int selectedLayer = 1;
    int SelectedLayer {
        get { return selectedLayer; }
        set { selectedLayer = Mathf.Clamp (value, 1, LayerCount); }
    }

    int LayerCount {
        get { return objectDrawer.Target.layers.Count; }
    }

    ParallaxLayerManager Target {
        get { return objectDrawer.Target; }
        set { objectDrawer.Target = value; }
    }

    GUILayoutOption largeButtonHeight = GUILayout.Height (25f);
    TextureLoader textureLoader;
    Vector2 scrollPos;
    GUILayoutOption[] options;

	[MenuItem ("Window/Parallax Designer")]
	static void Init () {
        EditorWindow editorWindow = GetWindow<ParallaxDesigner> ();
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show ();
    }

    void OnEnable () {
        if (layerDesigner == null) {
            layerDesigner = CreateInstance ("ParallaxLayerDesigner") as ParallaxLayerDesigner;
        }
        if (textureLoader == null) {
            textureLoader = new TextureLoader (TEXTURES_PATH);
        }
        SetTarget ();
    }

    void OnGUI () {
        
        if (!EditorState.InEditMode) {
            GUILayout.Label ("Editor disabled in play mode");
            return;
        }
        
        SetTarget ();
        DrawSaveLoad ();
        if (GUILayout.Button ("Load city textures from directory")) {
            // textureLoader.LoadCityTextures (Target);
            Dictionary<int, List<string>> texturePaths = textureLoader.GetTextureDirectories ();
            if (texturePaths != null)
                Target.Create (texturePaths);
        }

        options = new GUILayoutOption[] { GUILayout.MaxWidth (position.width), GUILayout.MinWidth (20f) };
        scrollPos = GUILayout.BeginScrollView (scrollPos, false, false, GUILayout.Width (position.width), GUILayout.Height (position.height - 90));
        objectDrawer.DrawObjectProperties (options);
        DrawLayerSelection ();
        layerDesigner.Options = options;
        layerDesigner.OnGUI ();
        GUILayout.EndScrollView ();
    }

    void DrawSaveLoad () {
        if (fileName != "") {
            GUILayout.Label ("Editing " + fileName);
        }
        EditorGUILayout.BeginHorizontal ();
        if (GUILayout.Button ("New")) {
            if (fileName == "") {
                NewDialog ();
            } else {
                New ();
            }
        }
        if (GUILayout.Button ("Save")) {
            if (savePath == "") {
                SaveAs ();
            } else {
                Save ();
            }
        }
        if (GUILayout.Button ("Save As")) {
            SaveAs ();
        }
        if (GUILayout.Button ("Load")) {
            Load ();
        }
        EditorGUILayout.EndHorizontal ();
    }

    void DrawLayerSelection () {
        if (LayerCount > 0) {

            GUILayout.Label ("Select a layer to edit:");
            EditorGUILayout.BeginHorizontal (options);
            if (GUILayout.Button ("<-")) {
                SelectedLayer -= 1;
                SetSelection ();
            }
            if (GUILayout.Button ("->")) {
                SelectedLayer += 1;
                SetSelection ();
            }
            int prevLayer = SelectedLayer;
            SelectedLayer = EditorGUILayout.IntField (SelectedLayer);
            if (prevLayer != SelectedLayer) {
                SetSelection ();
            }
            GUILayout.Label (" / " + LayerCount + " Layers");
            ParallaxLayer layer = Target.layers[selectedLayer-1];
            if (EditorWindow.focusedWindow == this) {
                layerDesigner.objectDrawer.Target = layer;
            }
            EditorGUILayout.EndHorizontal ();
        }
    }

    void SetTarget () {
        if (Target == null) {
            Target = ParallaxLayerManager.Instance;
        }
    }

    void SetSelection () {
        Selection.activeGameObject = Target.layers[selectedLayer-1].gameObject;
    }

    void New () {
        EditorObjectPool.Clear ();
        DestroyLayers ();
        Target.Reset ();
        fileName = "";
        savePath = "";
    }

    void NewDialog () {
        int option = EditorUtility.DisplayDialogComplex (
            "You have unsaved changes",
            "Save your changes or discard them",
            "Save City",
            "Cancel",
            "New without saving");
        switch (option) {
            case 0:
                SaveAs ();
                New ();
                break;
            case 1:
                break;
            case 2:
                New ();
                break;
        }
    }

    void Save () {
        objectDrawer.Save (savePath);
        EditorUtility.DisplayDialog ("Save successful", "The parallax scene '" + fileName + "' was successfully saved.", "OK");
    }

    void SaveAs () {
        string file = (fileName == "")
            ? Target.cityName + ".json"
            : fileName;
        savePath = EditorUtility.SaveFilePanel ("Save city", PATH, file, "json");
        if (savePath != "") {
            Save ();
            fileName = Path.GetFileName (savePath);
        }
    }

    void Load () {
        string loadPath = EditorUtility.OpenFilePanel ("Load a city", PATH, "json");
        if (loadPath != "") {
            New ();
            string path = loadPath.Replace (Application.dataPath + "/Resources/", "").Replace (".json", "");
            objectDrawer.Load (path);
            fileName = Path.GetFileName (loadPath);
            savePath = loadPath;
        }
    }
    
    void DestroyLayers () {
        DestroyImmediate (GameObject.Find ("ParallaxLayerPool"));
        DestroyImmediate (GameObject.Find ("ParallaxImagePool"));
        DestroyImmediate (GameObject.Find ("ParallaxNpcPool"));
        ParallaxLayer[] layers = FindObjectsOfType (typeof (ParallaxLayer)) as ParallaxLayer[];
        int layerCount = layers.Length;
        for (int i = 0; i < layerCount; i ++) {
            DestroyImmediate (layers[i]);
        }   
    }
}
#endif
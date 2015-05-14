using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class ParallaxDesigner : EditorWindow {

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

    GUILayoutOption largeButtonHeight = GUILayout.Height (25f);

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
        SetTarget ();
    }

    void OnGUI () {
        if (!EditorState.InEditMode) {
            GUILayout.Label ("Editor disabled in play mode");
            return;
        }
        SetTarget ();
        DrawPoolCommands ();
        DrawSaveLoad ();
        objectDrawer.DrawObjectProperties ();
        DrawLayerSelection ();
        layerDesigner.OnGUI ();
    }

    void DrawPoolCommands () {
        EditorGUILayout.BeginHorizontal ();
        GUI.color = Color.yellow;
        if (GUILayout.Button ("Clean up", largeButtonHeight)) {
            EditorObjectPool.CleanUp ();
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal ();
    }

    void DrawSaveLoad () {
        if (fileName != "") {
            GUILayout.Label ("Editing " + fileName);
        }
        EditorGUILayout.BeginHorizontal ();
        if (GUILayout.Button ("New")) {
            if (fileName == "") {

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
            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button ("<-")) {
                SelectedLayer -= 1;
            }
            if (GUILayout.Button ("->")) {
                SelectedLayer += 1;
            }
            SelectedLayer = EditorGUILayout.IntField (SelectedLayer, new GUILayoutOption[0]);
            ParallaxLayer layer = objectDrawer.Target.layers[selectedLayer-1];
            if (EditorWindow.focusedWindow == this) {
                layerDesigner.objectDrawer.Target = layer;
                Selection.activeGameObject = layer.gameObject;
            }
            EditorGUILayout.EndHorizontal ();
        }
    }

    void SetTarget () {
        if (objectDrawer.Target == null) {
            objectDrawer.Target = ParallaxLayerManager.Instance;
        }
    }

    void New () {
        EditorObjectPool.Clear ();
        fileName = "";
        savePath = "";
    }

    void Save () {
        objectDrawer.Save (savePath);
    }

    void SaveAs () {
        string file = (fileName == "")
            ? objectDrawer.Target.cityName + ".json"
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
            objectDrawer.Load (loadPath);
            fileName = Path.GetFileName (loadPath);
        }
    }
}


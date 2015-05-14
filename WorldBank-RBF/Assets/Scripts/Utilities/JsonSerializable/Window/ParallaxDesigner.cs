using UnityEngine;
using UnityEditor;
using System;
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
        if (GUILayout.Button ("Save")) {
            if (savePath == "") {
                SaveAs ();
            } else {
                objectDrawer.Save (savePath);
            }
        }
        if (GUILayout.Button ("Save As")) {
            SaveAs ();
        }
        if (GUILayout.Button ("Load")) {
            string loadPath = EditorUtility.OpenFilePanel ("Load a city", PATH, "json");
            objectDrawer.Load (loadPath);
        }
        EditorGUILayout.EndHorizontal ();
    }

    void SaveAs () {
        string file = (fileName == "")
            ? objectDrawer.Target.cityName + ".json"
            : fileName;
        savePath = EditorUtility.SaveFilePanel ("Save city", PATH, file, "json");
        objectDrawer.Save (savePath);
        fileName = System.IO.Path.GetFileName (savePath);
    }

    void DrawLayerSelection () {
        int layerCount = objectDrawer.Target.layers.Count;
        if (layerCount > 0) {
            selectedLayer = Mathf.Clamp (
                EditorGUILayout.IntField ("Select a layer to edit", selectedLayer, new GUILayoutOption[0]),
                1, layerCount);

            ParallaxLayer layer = objectDrawer.Target.layers[selectedLayer-1];
            layerDesigner.objectDrawer.Target = layer;
            Selection.activeGameObject = layer.gameObject;
        }
    }

    void SetTarget () {
        if (objectDrawer.Target == null) {
            objectDrawer.Target = ParallaxLayerManager.Instance;
        }
    }
}


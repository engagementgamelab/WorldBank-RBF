using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class ParallaxDesigner : EditorWindow {

    EditorObjectDrawer<ParallaxLayerManager> objectDrawer = 
        new EditorObjectDrawer<ParallaxLayerManager> ();

    ParallaxLayerDesigner layerDesigner;
    int selectedLayer = 1;

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
    }

    void OnSelectionChange () {
        objectDrawer.OnSelectionChange ();
        Repaint ();
    }

    void OnGUI () {
        if (objectDrawer.Selected) {
            GUILayout.Label (objectDrawer.Target.name);
            if (GUILayout.Button ("Save")) {
                objectDrawer.Save ();
            }
            if (GUILayout.Button ("Load")) {
                objectDrawer.Load ();
            }
            objectDrawer.DrawObjectProperties ();
            DrawLayerSelection ();
        }
        layerDesigner.OnGUI ();
    }

    void DrawLayerSelection () {
        int layerCount = objectDrawer.Target.layers.Count;
        if (layerCount > 0) {
            selectedLayer = Mathf.Clamp (
                EditorGUILayout.IntField ("Select a layer to edit", selectedLayer, new GUILayoutOption[0]),
                1, layerCount);
            layerDesigner.objectDrawer.Target = objectDrawer.Target.layers[selectedLayer-1];
        }
    }
}


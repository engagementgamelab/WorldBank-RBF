using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ParallaxLayerDesigner : ScriptableObject {

	string PATH { get { return Application.dataPath + "/Textures/Cities/"; } }

	public EditorObjectDrawer<ParallaxLayer> objectDrawer = 
        new EditorObjectDrawer<ParallaxLayer> ();

    public GUILayoutOption[] Options { get; set; }

    TextureLoader textureLoader;
    ParallaxNpcDesigner npcDesigner;
    ParallaxZoomTriggerDesigner zoomTriggerDesigner;
    
    int selectedNpcIndex = 0;
    int SelectedNpcIndex {
        get { return selectedNpcIndex; }
        set { selectedNpcIndex = Mathf.Clamp (value, 1, NpcCount); }
    }

    ParallaxNpc SelectedNpc {
    	get { 
    		if (SelectedNpcIndex > NpcCount) {
    			SelectedNpcIndex --;
    		}
    		return objectDrawer.Target.npcs[SelectedNpcIndex-1]; 
    	}
    }

    int NpcCount {
        get { return objectDrawer.Target.npcs.Count; }
    }

    int selectedZoomTriggerIndex = 0;
    int SelectedZoomTriggerIndex {
        get { return selectedZoomTriggerIndex; }
        set { selectedZoomTriggerIndex = Mathf.Clamp (value, 1, ZoomTriggerCount); }
    }

    ParallaxZoomTrigger SelectedZoomTrigger {
        get {
            if (SelectedZoomTriggerIndex > ZoomTriggerCount) {
                SelectedZoomTriggerIndex --;
            }
            return objectDrawer.Target.zoomTriggers[SelectedZoomTriggerIndex-1];
        }
    }

    int ZoomTriggerCount {
        get { return objectDrawer.Target.zoomTriggers.Count; }
    }

    bool showNpcDesigner = false;
    bool showZoomTriggerDesigner = false;

    void OnEnable () {
    	if (textureLoader == null) {
    		textureLoader = new TextureLoader (PATH);
    	}
    	if (npcDesigner == null) {
            npcDesigner = CreateInstance ("ParallaxNpcDesigner") as ParallaxNpcDesigner;
        }
        if (zoomTriggerDesigner == null) {
            zoomTriggerDesigner = CreateInstance ("ParallaxZoomTriggerDesigner") as ParallaxZoomTriggerDesigner;
        }
    }

	public void OnGUI () {
		if (objectDrawer.Target == null) return;

		int layerIndex = objectDrawer.Target.Index + 1;
		GUILayout.Label ("Layer " + layerIndex, Options);
		if (GUILayout.Button ("Load layer textures from directory", Options)) {
			textureLoader.LoadTexturesDirectory (objectDrawer.Target);
		}
		objectDrawer.DrawObjectProperties (Options);
		DrawNpcDesigner ();
        DrawZoomTriggerDesigner ();
	}

	void DrawNpcDesigner () {
		
        EditorGUILayout.BeginVertical (Options);
        showNpcDesigner = EditorGUILayout.Foldout (showNpcDesigner, "Edit NPCs");
        EditorGUILayout.EndVertical ();
        if (!showNpcDesigner) return;
        
        EditorGUILayout.BeginVertical (Options);
        if (GUILayout.Button ("Add NPC")) {
            objectDrawer.Target.AddNpc (
                EditorObjectPool.Create<ParallaxNpc> ());
        }

		if (NpcCount > 0) {
			GUILayout.Label ("Select an NPC to edit:");
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("<-")) {
                SelectedNpcIndex -= 1;
                SetNpcSelection ();
            }
            if (GUILayout.Button ("->")) {
                SelectedNpcIndex += 1;
                SetNpcSelection ();
            }
            int prevIndex = SelectedNpcIndex;
            SelectedNpcIndex = EditorGUILayout.IntField (SelectedNpcIndex, new GUILayoutOption[0]);
            if (prevIndex != SelectedNpcIndex) {
            	SetNpcSelection ();
            }
            GUILayout.Label (" / " + NpcCount + " NPCs");
            EditorGUILayout.EndHorizontal ();
            npcDesigner.OnGUI ();
            GUI.color = Color.red;
            if (GUILayout.Button ("Delete")) {
            	objectDrawer.Target.RemoveNpc (SelectedNpc);
            }
            GUI.color = Color.white;
		}
        EditorGUILayout.EndVertical ();
	}

	void SetNpcSelection () {
		npcDesigner.objectDrawer.Target = SelectedNpc;
		Selection.activeGameObject = SelectedNpc.gameObject;
	}

    void DrawZoomTriggerDesigner () {
        
        EditorGUILayout.BeginVertical (Options);     
        showZoomTriggerDesigner = EditorGUILayout.Foldout (showZoomTriggerDesigner, "Edit zoom triggers");
        EditorGUILayout.EndVertical ();
        if (!showZoomTriggerDesigner) return;

        EditorGUILayout.BeginVertical (Options);
        if (GUILayout.Button ("Add zoom trigger")) {
            objectDrawer.Target.AddZoomTrigger (
                EditorObjectPool.Create<ParallaxZoomTrigger> ());
        }

        if (ZoomTriggerCount > 0) {
            GUILayout.Label ("Select a zoom trigger to edit:");
            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button ("<-")) {
                SelectedZoomTriggerIndex -= 1;
                SetZoomTriggerSelection ();
            }
            if (GUILayout.Button ("->")) {
                SelectedZoomTriggerIndex += 1;
                SetZoomTriggerSelection ();
            }
            int prevIndex = SelectedZoomTriggerIndex;
            SelectedZoomTriggerIndex = EditorGUILayout.IntField (SelectedZoomTriggerIndex, new GUILayoutOption[0]);
            if (prevIndex != SelectedZoomTriggerIndex) {
                SetZoomTriggerSelection ();
            }
            GUILayout.Label (" / " + ZoomTriggerCount + " zoom triggers");
            EditorGUILayout.EndHorizontal ();
            zoomTriggerDesigner.OnGUI ();
            GUI.color = Color.red;
            if (GUILayout.Button ("Delete")) {
                objectDrawer.Target.RemoveZoomTrigger (SelectedZoomTrigger);
            }
            GUI.color = Color.white;
        }
        EditorGUILayout.EndVertical ();
    }

    void SetZoomTriggerSelection () {
        zoomTriggerDesigner.objectDrawer.Target = SelectedZoomTrigger;
        Selection.activeGameObject = SelectedZoomTrigger.gameObject;
    }
}

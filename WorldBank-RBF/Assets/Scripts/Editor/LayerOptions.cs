using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class LayerOptions : ScriptableObject {

	DepthLayer depthLayer = null;
	LayerImageOptions layerImageOptions;
	int selectedImage = -1;

	public void SetDepthLayer (DepthLayer depthLayer) {
		this.depthLayer = depthLayer;
		UnselectImage ();
	}

	public void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
        if (layerImageOptions == null) {
        	layerImageOptions = CreateInstance<LayerImageOptions> () as LayerImageOptions;
        }
    }

	public void OnGUI () {
		if (depthLayer == null) {
			GUILayout.Label ("No Layer Selected");
			return;
		}

		GUI.color = Color.white;
		depthLayer.LocalSeparation = EditorGUILayout.Slider ("Relative Distance", depthLayer.LocalSeparation, 0, DepthLayer.layerSeparation-1);

		GUILayout.Label ("Add or remove images", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("+")) {
			depthLayer.AddImage ();
		}
		if (GUILayout.Button ("-")) {
			depthLayer.RemoveImage ();
		}
		EditorGUILayout.EndHorizontal ();

		GUILayout.Label ("Select an image to edit", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal ();
		for (int i = 0; i < depthLayer.Images.Count; i ++) {
			LayerImage image = depthLayer.Images[i];
			if (image == null) continue;
			if (selectedImage == i) {
				GUI.color = Color.gray;
			} else {
				GUI.color = Color.white;
			}
			int imageIndex = i+1;
			if (GUILayout.Button ("Image " + imageIndex)) {
				selectedImage = i;
				layerImageOptions.SetLayerImage (image);
				Selection.activeGameObject = image.gameObject;
			}
		}
		EditorGUILayout.EndHorizontal ();
		layerImageOptions.OnGUI ();
	}

	void UnselectImage () {
		layerImageOptions.Unselect ();
		selectedImage = -1;
	}
}

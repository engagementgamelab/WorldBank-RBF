using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class LayerOptions : ScriptableObject {

	LayerSettings layerSettings = null;
	LayerImageOptions layerImageOptions;
	int selectedImage = -1;

	// TextureField textureField;
    // SerializedObject serializedTextureField;

	public void SetLayerSettings (LayerSettings layerSettings) {
		this.layerSettings = layerSettings;
		// textureField = CreateInstance ("TextureField") as TextureField;
		// serializedTextureField = new UnityEditor.SerializedObject (textureField);
		UnselectImage ();
	}

	public void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
        if (layerImageOptions == null) {
        	layerImageOptions = CreateInstance<LayerImageOptions> () as LayerImageOptions;
        }
    }

	public void OnGUI () {
		if (layerSettings == null) {
			GUILayout.Label ("No Layer Selected");
			return;
		}

		GUI.color = Color.white;
		layerSettings.LocalSeparation = EditorGUILayout.Slider ("Relative Distance", layerSettings.LocalSeparation, 0, DepthLayer.layerSeparation-1);

		GUILayout.Label ("Add or remove images", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("+")) {
			layerSettings.AddImage ();
		}
		if (GUILayout.Button ("-")) {
			layerSettings.RemoveImage ();
		}
		EditorGUILayout.EndHorizontal ();

		GUILayout.Label ("Select an image to edit", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal ();
		for (int i = 0; i < layerSettings.Images.Count; i ++) {
			LayerImage image = layerSettings.Images[i];
			if (image == null) continue;
			if (selectedImage == i) {
				GUI.color = Color.gray;
			} else {
				GUI.color = Color.white;
			}
			int imageIndex = i + 1;
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

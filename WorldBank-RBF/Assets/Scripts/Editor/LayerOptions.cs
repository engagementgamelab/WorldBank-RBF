using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class LayerOptions : ScriptableObject {

	LayerSettings layerSettings = null;
	TextureField textureField;
    SerializedObject serializedTextureField;

	public void SetLayerSettings (LayerSettings layerSettings) {
		this.layerSettings = layerSettings;
		textureField = CreateInstance ("TextureField") as TextureField;
		serializedTextureField = new UnityEditor.SerializedObject (textureField);
		textureField.textures = layerSettings.BackgroundTextures;
	}

	public void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
    }

	public void OnGUI () {
		if (layerSettings == null) {
			GUILayout.Label ("No Layer Selected");
			return;
		}

		GUI.color = Color.white;
		layerSettings.LocalSeparation = EditorGUILayout.Slider ("Relative Distance", layerSettings.LocalSeparation, 0, 19);

		if (serializedTextureField == null) return;
		serializedTextureField.Update ();
		EditorGUILayout.PropertyField (serializedTextureField.FindProperty ("textures"), new GUIContent ("Textures"), true);
		serializedTextureField.ApplyModifiedProperties ();
		layerSettings.BackgroundTextures = textureField.textures;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class LayerOptions : ScriptableObject {

	DepthLayer layer = null;

	float localSeparation = 0;
	float prevLocalSeparation = 0;
	float distanceConstraint = -1;

    List<Texture2D> textures;
    TextureField textureField;
    SerializedObject serializedTextureField;
    SerializedProperty textureProp;

	public void SetLayer (DepthLayer layer) {
		this.layer = layer;
		
		textureField = CreateInstance ("TextureField") as TextureField;
		serializedTextureField = new UnityEditor.SerializedObject (textureField);
		textureProp = serializedTextureField.FindProperty ("textures");
		
		localSeparation = layer.LocalSeparation;
		textureField.textures = layer.BackgroundTextures;
	}

	public void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
        if (distanceConstraint == -1)
        	distanceConstraint = DepthLayer.LayerSeparation-1;
        if (textureField == null) {
        	textureField = CreateInstance ("TextureField") as TextureField;
        }
        if (serializedTextureField == null) {
        	serializedTextureField = new UnityEditor.SerializedObject (textureField);
        }
        textureProp = serializedTextureField.FindProperty ("textures");
    }

    public void OnGUI () {
    	if (layer == null) {
    		GUILayout.Label ("No Layer Selected");
    		return;
    	}

    	GUI.color = Color.white;

		serializedTextureField.Update ();
		//EditorGUILayout.PropertyField (serializedTextureField.FindProperty ("textures"), new GUIContent ("Textures"), true);
		EditorGUILayout.PropertyField (textureProp, new GUIContent ("Textures"), true);
		serializedTextureField.ApplyModifiedProperties ();
		layer.BackgroundTextures = textureField.textures;

    	localSeparation = EditorGUILayout.Slider ("Relative Distance", localSeparation, -distanceConstraint, distanceConstraint);
    	if (localSeparation != prevLocalSeparation) {
    		layer.LocalSeparation = localSeparation;
    		prevLocalSeparation = localSeparation;
    	}
    }
}

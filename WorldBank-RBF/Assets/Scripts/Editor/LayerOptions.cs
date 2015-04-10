using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class LayerOptions : ScriptableObject {

	DepthLayer layer = null;
	int layerIndex = 0;

	float localSeparation = 0;
	float prevLocalSeparation = 0;
	float distanceConstraint = -1;

	Texture2D texture;
	TextureField textureField;
    SerializedObject serializedTextureField;

	public void SetLayer (DepthLayer layer) {
		this.layer = layer;
		layerIndex = layer.Index + 1;
		localSeparation = layer.LocalSeparation;
		texture = layer.BackgroundTexture;
		textureField.texture = texture;
		/*serializedTextureField.FindProperty ("texture");
		serializedTextureField.ApplyModifiedProperties ();*/
	}

	public void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
        if (distanceConstraint == -1)
        	distanceConstraint = DepthLayer.LayerSeparation-1;
        if (textureField == null) {
            textureField = ScriptableObject.CreateInstance<TextureField> ();
        }
        if (serializedTextureField == null) {
            serializedTextureField = new UnityEditor.SerializedObject (textureField);
        }
    }

    public void OnGUI () {
    	if (layer == null) {
    		GUILayout.Label ("No Layer Selected");
    		return;
    	}

    	GUI.color = Color.white;
    	/*EditorGUILayout.PropertyField (serializedTextureField.FindProperty ("texture"), new GUIContent("Background Texture"));
    	serializedTextureField.ApplyModifiedProperties ();
    	layer.BackgroundTexture = textureField.texture;*/

    	localSeparation = EditorGUILayout.Slider ("Relative Distance", localSeparation, -distanceConstraint, distanceConstraint);
    	if (localSeparation != prevLocalSeparation) {
    		layer.LocalSeparation = localSeparation;
    		prevLocalSeparation = localSeparation;
    	}
    }
}

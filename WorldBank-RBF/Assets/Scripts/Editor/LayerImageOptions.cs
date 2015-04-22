using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class LayerImageOptions : ScriptableObject {

	LayerImage layerImage;
	ImageSettings imageSettings;
	SerializedObject serializedImageSettings;

	public void SetLayerImage (LayerImage layerImage) {
		this.layerImage = layerImage;
		imageSettings = CreateInstance ("ImageSettings") as ImageSettings;
		imageSettings.Init (layerImage.Texture, layerImage.ColliderEnabled, layerImage.ColliderWidth, layerImage.ColliderCenter);
		serializedImageSettings = new UnityEditor.SerializedObject (imageSettings);
	}

	void OnEnable () {
		hideFlags = HideFlags.HideAndDontSave;
	}

	public void OnGUI () {
		if (serializedImageSettings == null) {
			GUILayout.Label ("No Image Selected");
			return;
		}

		GUI.color = Color.white;
		
		serializedImageSettings.Update ();
		EditorGUILayout.PropertyField (serializedImageSettings.FindProperty ("texture"), new GUIContent ("Texture"), true);
		serializedImageSettings.ApplyModifiedProperties ();

		imageSettings.colliderEnabled 	= EditorGUILayout.Toggle ("Enable Collider", imageSettings.colliderEnabled);
		if (imageSettings.colliderEnabled) {
			imageSettings.width 			= EditorGUILayout.Slider ("Collider Width", imageSettings.width, 0, 1);
			imageSettings.center 			= EditorGUILayout.Slider ("Collider Center", imageSettings.center, -0.5f, 0.5f);
		}
		layerImage.Texture 				= imageSettings.texture;
		layerImage.ColliderEnabled		= imageSettings.colliderEnabled;
		layerImage.ColliderWidth 		= imageSettings.width;
		layerImage.ColliderCenter 		= imageSettings.center;
	}

	public void Unselect () {
		serializedImageSettings = null;
	}
}

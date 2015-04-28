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
		imageSettings.Init (layerImage.NPCSymbol, layerImage.FacingLeft, layerImage.Texture, layerImage.ColliderEnabled, layerImage.ColliderWidth, layerImage.ColliderHeight, layerImage.ColliderCenterX, layerImage.ColliderCenterY);
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
		
		layerImage.NPCSymbol = EditorGUILayout.TextField ("NPC Symbol", layerImage.NPCSymbol);
		if (layerImage.NPCSymbol != "") {
			imageSettings.facingLeft 	= EditorGUILayout.Toggle ("Facing left", imageSettings.facingLeft);
		}

		serializedImageSettings.Update ();
		EditorGUILayout.PropertyField (serializedImageSettings.FindProperty ("texture"), new GUIContent ("Texture"), true);
		serializedImageSettings.ApplyModifiedProperties ();

		imageSettings.colliderEnabled 	= EditorGUILayout.Toggle ("Enable Collider", imageSettings.colliderEnabled);
		if (imageSettings.colliderEnabled) {
			imageSettings.width 		= EditorGUILayout.Slider ("Collider Width", imageSettings.width, 0, 1);
			imageSettings.height 		= EditorGUILayout.Slider ("Collider Height", imageSettings.height, 0, 1);
			imageSettings.centerX 		= EditorGUILayout.Slider ("Collider X", imageSettings.centerX, -0.5f, 0.5f);
			imageSettings.centerY 		= EditorGUILayout.Slider ("Collider Y", imageSettings.centerY, -0.5f, 0.5f);
		}
		layerImage.FacingLeft 			= imageSettings.facingLeft;
		layerImage.Texture 				= imageSettings.texture;
		layerImage.ColliderEnabled		= imageSettings.colliderEnabled;
		layerImage.ColliderWidth 		= imageSettings.width;
		layerImage.ColliderHeight		= imageSettings.height;
		layerImage.ColliderCenterX 		= imageSettings.centerX;
		layerImage.ColliderCenterY		= imageSettings.centerY;
	}

	public void Unselect () {
		serializedImageSettings = null;
	}
}

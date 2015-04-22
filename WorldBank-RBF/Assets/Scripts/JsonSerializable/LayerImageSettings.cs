#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LayerImageSettings {

	int index;
	string texture;
	bool collider_enabled;
	float collider_width;
	float collider_center;

	public void SetIndex (int index) {
		this.index = index;
	}

	public void SetTexture (Texture2D texture) {
		this.texture = AssetDatabase.GetAssetPath (texture);
	}

	public void SetColliderEnabled (bool collider_enabled) {
		this.collider_enabled = collider_enabled;
	}

	public void SetColliderWidth (float width) {
		collider_width = width;
	}

	public void SetColliderCenter (float center) {
		collider_center = center;
	}

	public int GetIndex () {
		return index;
	}

	public Texture2D GetTexture2D () {
		return AssetDatabase.LoadAssetAtPath (texture, typeof (Texture2D)) as Texture2D;
	}

	public string GetTexture () {
		return texture;
	}

	public bool GetColliderEnabled () {
		return collider_enabled;
	}

	public float GetColliderWidth () {
		return collider_width;
	}

	public float GetColliderCenter () {
		return collider_center;
	}
}
#endif
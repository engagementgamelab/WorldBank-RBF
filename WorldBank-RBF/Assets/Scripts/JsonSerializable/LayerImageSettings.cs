#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LayerImageSettings {

	int index;
	string npc_symbol;
	string texture;
	bool collider_enabled;
	float collider_width;
	float collider_height;
	float collider_center_x;
	float collider_center_y;

	public void SetIndex (int index) {
		this.index = index;
	}

	public void SetNPCSymbol (string npc_symbol) {
		this.npc_symbol = npc_symbol;
	}

	public void SetTexture (Texture2D texture) {
		this.texture = AssetDatabase.GetAssetPath (texture);
	}

	public void SetColliderEnabled (bool collider_enabled) {
		this.collider_enabled = collider_enabled;
	}

	public void SetColliderWidth (float collider_width) {
		this.collider_width = collider_width;
	}

	public void SetColliderHeight (float collider_height) {
		this.collider_height = collider_height;
	}

	public void SetColliderCenterX (float collider_center_x) {
		this.collider_center_x = collider_center_x;
	}

	public void SetColliderCenterY (float collider_center_y) {
		this.collider_center_y = collider_center_y;
	}

	public int GetIndex () {
		return index;
	}

	public string GetNPCSymbol () {
		return npc_symbol;
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

	public float GetColliderHeight () {
		return collider_height;
	}

	public float GetColliderCenterX () {
		return collider_center_x;
	}

	public float GetColliderCenterY () {
		return collider_center_y;
	}
}
#endif
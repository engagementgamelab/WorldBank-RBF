#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LayerImageSettings {

	public int index { get; set; }
	public string npc_symbol { get; set; }
	public string texture { get; set; }
	public bool collider_enabled { get; set; }
	public float collider_width { get; set; }
	public float collider_height { get; set; }
	public float collider_center_x { get; set; }
	public float collider_center_y { get; set; }

	public void SetTexture (Texture2D texture) {
		this.texture = AssetDatabase.GetAssetPath (texture);
	}

	public Texture2D GetTexture () {
		return AssetDatabase.LoadAssetAtPath (texture, typeof (Texture2D)) as Texture2D;
	}
}
#endif
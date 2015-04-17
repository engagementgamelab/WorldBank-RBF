using UnityEngine;
using UnityEditor;
using System.Collections;

public class ImageSettings : ScriptableObject {

	public Texture2D texture;
	public float width;
	public float center;

	public void Init (Texture2D texture, float width, float center) {
		this.texture = texture;
		this.width = width;
		this.center = center;
	}
}

using UnityEngine;
using UnityEditor;
using System.Collections;

public class ImageSettings : ScriptableObject {

	public Texture2D texture;
	public bool colliderEnabled;
	public float width;
	public float center;

	public void Init (Texture2D texture, bool colliderEnabled, float width, float center) {
		this.texture = texture;
		this.colliderEnabled = colliderEnabled;
		this.width = width;
		this.center = center;
	}
}

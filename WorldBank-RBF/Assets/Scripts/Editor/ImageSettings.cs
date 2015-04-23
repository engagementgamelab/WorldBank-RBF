using UnityEngine;
using UnityEditor;
using System.Collections;

public class ImageSettings : ScriptableObject {

	public string npcSymbol;
	public Texture2D texture;
	public bool colliderEnabled;
	public float width;
	public float height;
	public float centerX;
	public float centerY;

	public void Init (string npcSymbol, Texture2D texture, bool colliderEnabled, float width, float height, float centerX, float centerY) {
		this.npcSymbol = npcSymbol;
		this.texture = texture;
		this.colliderEnabled = colliderEnabled;
		this.width = width;
		this.height = height;
		this.centerX = centerX;
		this.centerY = centerY;
	}
}

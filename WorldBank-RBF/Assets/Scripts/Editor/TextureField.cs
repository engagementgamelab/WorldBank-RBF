using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// get rid of this ??
public class TextureField : ScriptableObject {
	public List<Texture2D> textures;
	public List<float> colliderWidths;
	public List<float> colliderCenters;
}

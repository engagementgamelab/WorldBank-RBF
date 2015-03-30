using UnityEngine;
using System.Collections;

public class MaterialsManager {

	public static Material CreateMaterialFromTexture (Texture2D texture) {
		Material m = new Material (Shader.Find ("Mobile/Diffuse"));
		m.mainTexture = texture;
		return m;
	}
}

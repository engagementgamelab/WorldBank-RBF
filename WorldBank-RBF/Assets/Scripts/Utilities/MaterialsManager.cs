using UnityEngine;
using System.Collections;

public class MaterialsManager {

	public static Material CreateMaterialFromTexture (Texture2D texture, bool transparent=true) {
		Shader shader = Shader.Find ("Standard");
		Material m = new Material (shader);
		if (transparent) {
			m.SetFloat ("_Mode", 2);
	        m.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
	        m.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
	        m.SetInt ("_ZWrite", 0);
	        m.DisableKeyword ("_ALPHATEST_ON");
	        m.EnableKeyword ("_ALPHABLEND_ON");
	        m.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
	        m.renderQueue = 3000;
		} else {
			m.SetFloat ("_Mode", 0);
		}
		m.SetFloat ("_Glossiness", 0);
		m.mainTexture = texture;
		return m;
	}
}

using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class MaterialsManager {

	public static Material GetMaterialAtPath (string path) {
		path = path.Replace ("Assets/Textures/", "Materials/").Replace (".png", "");
		Material material = Resources.Load (path) as Material;
		return material;
	}

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

		#if UNITY_EDITOR
		string path = AssetDatabase.GetAssetPath (texture).Replace ("/Textures/", "/Resources/Materials/").Replace (texture.name + ".png", "");
		string fileName = texture.name + ".mat";
		if (fileName != ".mat" && !CreateDirectory (path, fileName)) {
			AssetDatabase.CreateAsset (m, path + fileName);
		}
		#endif

		return m;
	}

	#if UNITY_EDITOR
	static bool CreateDirectory (string path, string fileName) {
		string dataPath = Application.dataPath.Replace ("Assets", "");
		string path2 = dataPath + path;
		if (!Directory.Exists (path2)) {
            Directory.CreateDirectory (path2);
        }
        return File.Exists (path2 + fileName);
	}

	public static void PrepareMaterialsFromTextures () {
		string[] textureFiles = Directory.GetFiles (Application.dataPath + "/Textures", "*.png", SearchOption.AllDirectories);
		for (int i = 0; i < textureFiles.Length; i ++) {
			string path = textureFiles[i].Replace (Application.dataPath, "").Replace ("/Textures/", "Assets/Textures/");
			CreateMaterialFromTexture ((Texture2D)AssetDatabase.LoadAssetAtPath (path, typeof (Texture2D)));
		}
	}
	#endif

	public static Material Blank {
		get { return Resources.Load ("Materials/Blank.mat") as Material; }
	}

	public static bool TextureIsBlank (Texture2D tex) {
		
		if (!tex.format.HasAlpha ()) return false;
		
		try {
			Color c = tex.GetPixel (0, 0);
		} catch (UnityException e) {
			Debug.LogError (e);
			return false;
		}
		int w = tex.width;
		int h = tex.height;
		int resolution = 16;
		for (int i = 0; i < w; i += resolution) {
			for (int j = 0; j < h; j += resolution) {
				if (tex.GetPixel (i, j).a > 0f) {
					return false;
				}
			}
		}
		return true;
	}
}

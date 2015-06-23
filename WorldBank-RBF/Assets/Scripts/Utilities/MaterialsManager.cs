 	using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class MaterialsManager {

	public static Material CreateMaterialFromTexture (Texture2D texture, bool transparent=true) {
		
		#if !UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX
		#if UNITY_EDITOR
		string path = "Assets/Materials/";
		Material material = AssetDatabase.LoadAssetAtPath (path + texture.name + ".mat", typeof (Material)) as Material;
		if (material != null) {
			return material;
		}
		#elif !UNITY_WEBPLAYER
		Material material = Resources.Load ("Materials/" + texture.name + ".mat") as Material;
		if (material != null) {
			return material;
		}
		#endif
		#endif

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
		
		#if UNITY_EDITOR && !UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX
		AssetDatabase.CreateAsset (m, path + texture.name + ".mat");
		#endif

		return m;
	}

	/*public static void PrepareMaterialsForBuild () {
		#if UNITY_EDITOR && !UNITY_WEBPLAYER
		string path = Application.dataPath + "/Resources/Materials";
		if (!Directory.Exists (path)) {
            Directory.CreateDirectory (path);
        }
		string[] materialFiles = Directory.GetFiles (Application.dataPath + "/Materials", "*.mat", SearchOption.AllDirectories);
		for (int i = 0; i < materialFiles.Length; i ++) {
			string fromPath = "Assets" + materialFiles[i].Replace (Application.dataPath, "").Replace ('\\', '/');
			string fileName = fromPath.Replace ("Assets/Materials/", "");
			string resourcesPath = "/Resources/Materials/" + fileName;
			string toPath = "Assets" + resourcesPath;
			if (File.Exists (Application.dataPath + resourcesPath)) {
				continue;
			}
			AssetDatabase.CopyAsset (fromPath, toPath);
			AssetDatabase.Refresh ();
		}
		#endif
	}*/

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

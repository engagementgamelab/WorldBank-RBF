﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;
#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEditor;
#endif

/// <summary>
/// Handles material creation, saving, and loading.
/// </summary>
public static class MaterialsManager {

	static Models.MaterialData _materialData;

	public static Material GetMaterialAtPath (string path, AnimatedQuadTexture quadTex) {

		bool textureInProject = path.Contains ("Assets/Textures/");

		if (textureInProject) {
			
			string materialPath = path.Replace (Application.dataPath, "")
				.Replace ("Assets", "")
				.Replace ("/Textures", "Materials")
				.Replace (".png", "");

			return Resources.Load (materialPath) as Material;
		} 

		if (path != "") {
			path = "file://" + path;
            Coroutine.LoadTexture (path, quadTex);
            return Blank;
		}

		return null;
	}

	public static Material CreateMaterialFromTexture (Texture2D texture) {

		bool transparent = texture == null
			? false
			: texture.format.HasAlpha ();

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

	#if UNITY_EDITOR && !UNITY_WEBPLAYER
	public static string GetTexturePath (Texture2D texture) {
		return AssetDatabase.GetAssetPath (texture);
	}

	static Material CreateMaterialAndAddToDatabase (string texturePath) {

		Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath (texturePath, typeof (Texture2D));
		Material m = CreateMaterialFromTexture (texture);
		
		if (texture != null) AddMaterialToDatabase (texture, m);

		SetTextureIsBlank(texture, texturePath);

		return m;

	}

	static void AddMaterialToDatabase (Texture2D texture, Material m) {
		string path = AssetDatabase.GetAssetPath (texture)
			.Replace ("/Textures/", "/Resources/Materials/")
			.Replace (texture.name + ".png", "");
		string fileName = texture.name + ".mat";
		if (fileName != ".mat" && !CreateDirectory (path, fileName)) {
			AssetDatabase.CreateAsset (m, path + fileName);
		}
	}

	static bool CreateDirectory (string path, string fileName) {
		string dataPath = Application.dataPath.Replace ("Assets", "");
		string path2 = dataPath + path;
		if (!Directory.Exists (path2)) {
            Directory.CreateDirectory (path2);
        }
        return File.Exists (path2 + fileName);
	}

	public static void PrepareMaterialsFromTextures () {

		_materialData = new Models.MaterialData();
		_materialData.blank_textures = new Dictionary<string, bool>();

		string[] textureFiles = Directory.GetFiles (Application.dataPath + "/Textures", "*.png", SearchOption.AllDirectories);
		for (int i = 0; i < textureFiles.Length; i ++) {
			string path = textureFiles[i].Replace (Application.dataPath, "").Replace ("/Textures/", "Assets/Textures/");
			CreateMaterialAndAddToDatabase (path);
		}


    System.Text.StringBuilder material_output = new System.Text.StringBuilder();
    
    JsonWriter writer = new JsonWriter (material_output);
    writer.Write(_materialData);
    
    DataManager.SaveDataToJson("material_data", material_output.ToString(), false);

    Debug.Log("*** MATERIALS CREATED ***");

	}
	#endif

	public static Material Blank {
		get { return Resources.Load ("Materials/blank") as Material; }
	}

	public static bool GetTextureIsBlank(string texturePath) {

		string textureKey = texturePath.Replace("Assets/Textures/Cities/", "").Replace(".png", "");

		if(_materialData == null) {
		    TextAsset dataJson = (TextAsset)Resources.Load("material_data", typeof(TextAsset));
				StringReader strData = new StringReader(dataJson.text);

		    JsonReader reader = new JsonReader(strData.ReadToEnd());
				_materialData = reader.Deserialize<Models.MaterialData>();

				strData.Close();
		}

    return false;

	}

	static void SetTextureIsBlank (Texture2D tex, string texturePath) {

		Debug.Log(texturePath);

		bool isBlank = false;
		string textureKey = texturePath.Replace("Assets/Textures/Cities/", "").Replace(".png", "");
		
		#if UNITY_IOS || UNITY_ANDROID
			isBlank = false;
		#endif
	
		if (tex == null) isBlank = true;
		if (!tex.format.HasAlpha ()) isBlank = false;
		
		try {
			tex.GetPixel (0, 0);
		} catch (UnityException e) {
			Debug.LogError (e);
			isBlank = false;
		}
		int w = tex.width;
		int h = tex.height;
		int resolution = 16;

		for (int i = 0; i < w; i += resolution) {
			for (int j = 0; j < h; j += resolution) {
				if (tex.GetPixel (i, j).a > 0f) {
					isBlank = false;
				}
			}
		}

		_materialData.blank_textures.Add(textureKey, isBlank);

	}
}

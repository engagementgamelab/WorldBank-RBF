/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 ImportTextures.cs
 Unity texture asset import settings.

 Created by Johnny Richardson on 3/31/15.
==============
*/

// Run only if inside editor
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TexturePostProcessor : AssetPostprocessor {
	// Texture import setting vars
	int _qualityBest = (int)TextureCompressionQuality.Best;
	int _qualityGood = (int)TextureCompressionQuality.Normal;
	int _qualityFast = (int)TextureCompressionQuality.Fast;
	
	// Set per-platform texture settings
	void OnPostprocessTexture(Texture2D texture) {

		Debug.Log("OnPostprocessTexture");
	
		TextureImporter importer = assetImporter as TextureImporter;

		importer.anisoLevel = 0;
		importer.filterMode = FilterMode.Bilinear;
		importer.isReadable = true;
		importer.mipmapEnabled = false;
		importer.wrapMode = TextureWrapMode.Clamp;

		// Set texture settings
		importer.SetPlatformTextureSettings( "iPhone", 1024, TextureImporterFormat.PVRTC_RGBA4, _qualityGood, false );
		importer.SetPlatformTextureSettings( "Android", 512, TextureImporterFormat.ETC2_RGBA8, _qualityFast, false ); //ATC_RGBA8 \\ ETC2_RGBA8 || PVRTC_RGBA4
	
		importer.SetPlatformTextureSettings( "Standalone", 512, TextureImporterFormat.PVRTC_RGBA4, _qualityFast, false );
		importer.SetPlatformTextureSettings( "WebPlayer", 1024, TextureImporterFormat.PVRTC_RGBA4, _qualityBest, false );
		importer.SetPlatformTextureSettings( "WebGL", 512, TextureImporterFormat.PVRTC_RGBA4, _qualityFast, false );

		Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
		if (asset) {
			EditorUtility.SetDirty(asset);
		} else {
			texture.anisoLevel = 0;
			texture.filterMode = FilterMode.Bilinear;
			texture.wrapMode = TextureWrapMode.Clamp;
		}

	}

}
#endif
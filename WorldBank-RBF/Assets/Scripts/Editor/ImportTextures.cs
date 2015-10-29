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
	// TextureCompressionQuality _qualityBest = TextureCompressionQuality.Best;
	// TextureCompressionQuality _qualityGood = TextureCompressionQuality.Normal;
	// TextureCompressionQuality _qualityFast = TextureCompressionQuality.Fast;
	
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
		importer.SetPlatformTextureSettings( "iPhone", 1024, TextureImporterFormat.PVRTC_RGBA4, (int)TextureCompressionQuality.Normal, false );

		/*if(assetPath.ToLower().IndexOf("/layer4") != -1)
			importer.SetPlatformTextureSettings( "Android", 1024, TextureImporterFormat.ETC_RGB4, (int)TextureCompressionQuality.Fast, false ); //ATC_RGBA8 \\ ETC2_RGBA8 || PVRTC_RGBA4
		else*/
		importer.SetPlatformTextureSettings( "Android", 1024, TextureImporterFormat.PVRTC_RGBA4, (int)TextureCompressionQuality.Fast, false ); //ATC_RGBA8 \\ ETC2_RGBA8 || PVRTC_RGBA4
	
		importer.SetPlatformTextureSettings( "Standalone", 4096, TextureImporterFormat.PVRTC_RGBA4, 100, false );
		importer.SetPlatformTextureSettings( "WebPlayer", 1024, TextureImporterFormat.PVRTC_RGBA4, 100, false );
		importer.SetPlatformTextureSettings( "WebGL", 1024, TextureImporterFormat.PVRTC_RGBA4, 100, false );

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
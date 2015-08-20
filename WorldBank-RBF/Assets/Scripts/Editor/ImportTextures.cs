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
	
		TextureImporter importer = assetImporter as TextureImporter;

		importer.anisoLevel = 0;
		importer.filterMode = FilterMode.Bilinear;
		importer.isReadable = true;
		importer.mipmapEnabled = false;
		importer.wrapMode = TextureWrapMode.Clamp;

		// TODO: Find most optimized settings
		importer.SetPlatformTextureSettings( "iPhone", 4096, TextureImporterFormat.PVRTC_RGBA4, 100 );
		importer.SetPlatformTextureSettings( "Android", 2048, TextureImporterFormat.RGBA16, 100 );
		importer.SetPlatformTextureSettings( "Standalone", 4096, TextureImporterFormat.PVRTC_RGBA4, 100 );
		importer.SetPlatformTextureSettings( "WebPlayer", 1024, TextureImporterFormat.PVRTC_RGBA4, 100 );
		importer.SetPlatformTextureSettings( "WebGL", 1024, TextureImporterFormat.PVRTC_RGBA4, 100 );

		Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
		if (asset) {
			EditorUtility.SetDirty(asset);
		} else {
			texture.anisoLevel = 0;
			texture.filterMode = FilterMode.Bilinear;          
		}
	}
}
#endif
/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 ImportAudio.cs
 Unity audio asset import settings.

 Created by Johnny Richardson on 11/09/15.
==============
*/

// Run only if inside editor
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AudioPostProcessor : AssetPostprocessor {
	
	// Set per-platform audio settings
	void OnPostprocessAudio(AudioClip clip) {
	
		AudioImporter importer = assetImporter as AudioImporter;

		AudioImporterSampleSettings settings = new AudioImporterSampleSettings();
		settings.compressionFormat = AudioCompressionFormat.AAC;
		settings.loadType = AudioClipLoadType.DecompressOnLoad;
		settings.sampleRateOverride = 11025;
		settings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;

		importer.SetOverrideSampleSettings( "WebGL", settings );

	}

}
#endif
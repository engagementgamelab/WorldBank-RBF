/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 ImportAudio.cs
 Unity audio asset import settingsWebGL.

 Created by Johnny Richardson on 11/09/15.
==============
*/

// Run only if inside editor
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AudioPostProcessor : AssetPostprocessor {
	
	// Set per-platform audio settingsWebGL
	void OnPostprocessAudio(AudioClip clip) {
	
		AudioImporter importer = assetImporter as AudioImporter;

		AudioImporterSampleSettings settingsWebGL = new AudioImporterSampleSettings();
		settingsWebGL.compressionFormat = AudioCompressionFormat.AAC;
		settingsWebGL.loadType = AudioClipLoadType.DecompressOnLoad;
		settingsWebGL.sampleRateOverride = 11025;
		settingsWebGL.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;

		AudioImporterSampleSettings settingsStandalone = new AudioImporterSampleSettings();
		settingsStandalone.compressionFormat = AudioCompressionFormat.AAC;
		settingsStandalone.loadType = AudioClipLoadType.DecompressOnLoad;
		settingsStandalone.quality = 0.1f;
		settingsStandalone.sampleRateOverride = 11025;
		settingsStandalone.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;

		importer.SetOverrideSampleSettings( "WebGL", settingsWebGL );
		importer.SetOverrideSampleSettings( "Standalone", settingsStandalone );

	}

}
#endif
/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 GenerateBuilds.cs
 Unity build script.

 Created by Johnny Richardson on 4/6/15.
==============
*/

#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

class GenerateBuilds {

    // These are the scenes that the build server is going to use
    static string[] SCENES = new string[] {"PhaseOne", "PhaseTwo"};

    static string SCENE_PREFIX = "Assets/Scenes/";
    static string SCENE_AFFIX = ".unity";

    static string APP_NAME = "WorldBank";
    static string TARGET_DIR = "Output";

    [MenuItem ("Build/Prepare Materials")]
    static void PrepareMaterials () {
        MaterialsManager.PrepareMaterialsFromTextures();
    }

    [MenuItem ("Build/Build Mac OS X Universal")]
    static void PerformMacOSXBuild ()
    {
        string target_dir = APP_NAME + ".app";
        GenericBuild("Mac", target_dir, BuildTarget.StandaloneOSXUniversal, BuildOptions.None);
    }
    [MenuItem ("Build/Build PC")]
    static void PerformPCBuild ()
    {
        GenericBuild("PC", APP_NAME, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem ("Build/Build WebGL")]
    static void PerformWebGLBuild ()
    {
        string target_dir = APP_NAME;
        GenericBuild("WebGL", target_dir, BuildTarget.WebGL, BuildOptions.None);
    }

    [MenuItem ("Build/Build Web")]
    static void PerformWebBuild ()
    {
         string target_dir = APP_NAME;
         GenericBuild("Web", target_dir, BuildTarget.WebPlayer, BuildOptions.None);
    }

    [MenuItem ("Build/Build All")]
    static void MakeAllBuilds()
    {
        PerformMacOSXBuild();
        PerformPCBuild();
        PerformWebGLBuild();
        PerformWebBuild();
    }

    static string[] FindEnabledScenes() {
        
        List<string> EditorScenes = new List<string>();
        
        foreach(string sceneName in SCENES)
            EditorScenes.Add(SCENE_PREFIX + sceneName + SCENE_AFFIX);

        return EditorScenes.ToArray();

    }

    static void GenericBuild(string platform, string buildDir, BuildTarget build_target, BuildOptions build_options)
    {

        EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
        PrepareMaterials ();
        
        string res = BuildPipeline.BuildPlayer(FindEnabledScenes(), TARGET_DIR + "/" + platform + "/" + buildDir, build_target, build_options);
        
        if (res.Length > 0)
            throw new Exception("BuildPlayer failure: " + res);

    }

}
#endif
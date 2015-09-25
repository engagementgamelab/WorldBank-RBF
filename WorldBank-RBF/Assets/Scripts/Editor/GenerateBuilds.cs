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
    static string[] SCENES = new string[] {"PhaseOne", "Menus", "PhaseTwo"};

    // Options for all builds
    static BuildOptions BUILD_OPTIONS = BuildOptions.Development | BuildOptions.AllowDebugging;

    // Scene directory
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
        GenericBuild("Mac", BuildTarget.StandaloneOSXUniversal);
    }
    [MenuItem ("Build/Build PC")]
    static void PerformPCBuild ()
    {
        GenericBuild("PC", BuildTarget.StandaloneWindows);
    }

    [MenuItem("File/AutoBuilder/Android")]
    static void PerformAndroidBuild ()
    {
        GenericBuild("Android", BuildTarget.Android);
    }

    [MenuItem ("Build/Build WebGL")]
    static void PerformWebGLBuild ()
    {
        GenericBuild("WebGL", BuildTarget.WebGL);
    }

    [MenuItem ("Build/Build Web")]
    static void PerformWebBuild ()
    {
        GenericBuild("Web", BuildTarget.WebPlayer);
    }

    [MenuItem ("Build/Build All")]
    static void MakeAllBuilds()
    {
        PerformMacOSXBuild();
        PerformPCBuild();
        PerformWebGLBuild();
        PerformWebBuild();
    }

    [MenuItem ("Build/Build Staging")]
    static void MakeStagingBuilds()
    {
        BUILD_OPTIONS = BuildOptions.None;

        PerformMacOSXBuild();
        PerformPCBuild();
    }

    static string[] FindEnabledScenes() {

        List<string> EditorScenes = new List<string>();

        foreach (string sceneName in SCENES)
            EditorScenes.Add(SCENE_PREFIX + sceneName + SCENE_AFFIX);

        return EditorScenes.ToArray();

    }

    /// <summary>
    /// Generate a game binary using the given options.
    /// </summary>
    /// <param name="platform">The platform name, used as the name for the folder to contain the platform's binary.</param>
    /// <param name="buildTarget">The Unity build target.</param>
    static void GenericBuild(string platform, BuildTarget buildTarget)
    {

        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);
        PrepareMaterials();

        string name = APP_NAME;

        if(platform == "Mac")
            name = APP_NAME + ".app";
        else if(platform == "PC") 
            name = APP_NAME + ".exe";         

        string res = BuildPipeline.BuildPlayer(FindEnabledScenes(), TARGET_DIR + "/" + platform + "/" + name, buildTarget, BUILD_OPTIONS);

        if (res.Length > 0)
            throw new Exception("BuildPlayer failure: " + res);

    }

}
#endif
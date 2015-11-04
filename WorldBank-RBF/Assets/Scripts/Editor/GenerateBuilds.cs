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
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

class GenerateBuilds {

    // These are the scenes that the build server is going to use
    static string[] SCENES = new string[] { "ScreenSizeSetup", "Menus", "PhaseOne", "PhaseTwo"};
    static string[] SCENES_MOBILE = new string[] { "Menus", "PhaseOne", "PhaseTwo"};

    // Options for all builds
    static BuildOptions BUILD_OPTIONS = BuildOptions.Development | BuildOptions.AllowDebugging;

    // Scene directory
    static string SCENE_PREFIX = "Assets/Scenes/";
    static string SCENE_AFFIX = ".unity";

    static string APP_NAME = "Unlocking Health";
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

    static void PerformAndroidBuild ()
    {
     
        GenericBuild("Android", BuildTarget.Android);
    }

    static void PerformiOSBuild ()
    {
        
        EditorUserBuildSettings.symlinkLibraries = false;
     
        GenericBuild("iOS", BuildTarget.iOS);
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
        
        // PerformWebBuild();

        PerformiOSBuild();
        PerformAndroidBuild();
        // PerformWebGLBuild();
    }

    [MenuItem ("Build/Build Desktop")]
    static void MakeDesktopBuilds()
    {

        PerformMacOSXBuild();
        PerformPCBuild();
    }

    [MenuItem ("Build/Build Mobile")]
    static void MakeMobileBuilds()
    {

        PerformiOSBuild();
        PerformAndroidBuild();
    }

    [MenuItem ("Build/Build Desktop for Production")]
    static void MakeProductionBuilds()
    {
        BUILD_OPTIONS = BuildOptions.None;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "IS_PRODUCTION");

        PerformMacOSXBuild();
        PerformPCBuild();
    }

    [MenuItem ("Build/Build Staging")]
    static void MakeStagingBuilds()
    {
        BUILD_OPTIONS = BuildOptions.None;

        PerformMacOSXBuild();
        PerformPCBuild();
    }

    [MenuItem ("Build/Set Icons")]
    static void SetIcons ()
    {
        SetIcons (BuildTarget.StandaloneOSXUniversal);
    }

    static void SetIcons(BuildTarget buildTarget)
    {
        if (buildTarget.GetBuildTargetGroup () == BuildTargetGroup.Standalone) {
            Texture2D[] textures = new [] {
                (Texture2D)Resources.Load ("AppIcons/icon_logo_1024"),
                (Texture2D)Resources.Load ("AppIcons/icon_logo_512"),
                (Texture2D)Resources.Load ("AppIcons/icon_logo_256"),
                (Texture2D)Resources.Load ("AppIcons/icon_logo_128"),
                (Texture2D)Resources.Load ("AppIcons/icon_logo_48"),
                (Texture2D)Resources.Load ("AppIcons/icon_logo_32"),
                (Texture2D)Resources.Load ("AppIcons/icon_logo_16")
            };

            PlayerSettings.SetIconsForTargetGroup (BuildTargetGroup.Standalone, textures);
        }
    }

    static string[] FindEnabledScenes(string platform) {

        string[] sceneList = (platform == "iOS" || platform == "Android") ? SCENES_MOBILE : SCENES;

        List<string> EditorScenes = new List<string>();

        foreach (string sceneName in sceneList)
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

        // Disable all but 4x3
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect4by3, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by10, false);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by9, false);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect5by4, false);
        PlayerSettings.SetAspectRatio(AspectRatio.AspectOthers, false);

        // Disable splash and res dialog
        PlayerSettings.showUnitySplashScreen = false;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;

        PlayerSettings.productName = "Unlocking Health";
        PlayerSettings.companyName = "Engagement Lab";

        PrepareMaterials();

        string name = APP_NAME;

        if(platform == "Mac")
            name = APP_NAME + ".app";
        else if(platform == "PC") 
            name = APP_NAME + ".exe";         

        SetIcons (buildTarget);

        string res = BuildPipeline.BuildPlayer(FindEnabledScenes(platform), TARGET_DIR + "/" + platform + "/" + name, buildTarget, BUILD_OPTIONS);

        if (res.Length > 0)
            throw new Exception("BuildPlayer failure: " + res);

        // Reset define symbols for all groups
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, null);

    }

}
#endif
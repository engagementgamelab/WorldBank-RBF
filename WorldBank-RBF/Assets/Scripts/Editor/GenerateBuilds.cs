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

    static string[] SCENES = FindEnabledEditorScenes();

    static string APP_NAME = "WorldBank";
    static string TARGET_DIR = "Output";
    [MenuItem ("Build/Build Mac OS X")]
    static void PerformMacOSXBuild ()
    {
             string target_dir = APP_NAME + ".app";
             GenericBuild(SCENES, TARGET_DIR + "/Mac/" + target_dir, BuildTarget.StandaloneOSXIntel, BuildOptions.None);
    }

    [MenuItem ("Build/Build WebGL")]
    static void PerformWebGLBuild ()
    {
             string target_dir = APP_NAME;
             GenericBuild(SCENES, TARGET_DIR + "/WebGL/" + target_dir, BuildTarget.WebGL, BuildOptions.None);
    }

    [MenuItem ("Build/Build Web")]
    static void PerformWebBuild ()
    {
             string target_dir = APP_NAME;
             GenericBuild(SCENES, TARGET_DIR + "/Web/" + target_dir, BuildTarget.WebPlayer, BuildOptions.None);
    }

    [MenuItem ("Build/Build All")]
    static void MakeAllBuilds()
    {
        PerformMacOSXBuild();
        PerformWebGLBuild();
        PerformWebBuild();
    }


    private static string[] FindEnabledEditorScenes() {
        List<string> EditorScenes = new List<string>();
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
            EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
            string res = BuildPipeline.BuildPlayer(scenes,target_dir,build_target,build_options);
            if (res.Length > 0) {
                    throw new Exception("BuildPlayer failure: " + res);
            }
    }

}
#endif
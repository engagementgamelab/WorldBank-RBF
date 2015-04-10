using UnityEngine;
using UnityEditor;
 
public class SceneGenerator : EditorWindow {

    SceneGeneratorOptions options;
 
    [MenuItem ("Window/SceneGenerator")]
    static void Init () {
        GetWindow<SceneGenerator> ();
    }
 
    void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
        if (options == null)
            options = CreateInstance ("SceneGeneratorOptions") as SceneGeneratorOptions;
    }
 
    void OnGUI () {
        GUILayout.Label ("Options", EditorStyles.boldLabel);
        options.OnGUI ();
    }
}
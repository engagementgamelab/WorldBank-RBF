using UnityEngine;
using UnityEditor;
 
public class SceneGenerator : EditorWindow {

    SceneGeneratorOptions options;
    bool refresh = false;
 
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
        if (!EditorState.InEditMode) {
            GUILayout.Label ("Window disabled in Play Mode");
            refresh = true;
            return;
        }

        GUILayout.Label ("Options", EditorStyles.boldLabel);
        if (refresh) {
            options.Refresh ();
            refresh = false;
        }
        options.OnGUI ();
    }
}
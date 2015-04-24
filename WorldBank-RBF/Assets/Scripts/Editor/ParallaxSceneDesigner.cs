using UnityEngine;
using UnityEditor;
 
public class ParallaxSceneDesigner : EditorWindow {

    ParallaxSceneDesignerOptions options;
    bool refresh = false;
 
    [MenuItem ("Window/Parallax Scene Designer")]
    static void Init () {
        GetWindow<ParallaxSceneDesigner> ();
    }
 
    void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
        if (options == null) {
            options = CreateInstance ("ParallaxSceneDesignerOptions") as ParallaxSceneDesignerOptions;
        }
    }

    void OnGUI () {
        if (!EditorState.InEditMode) {
            GUILayout.Label ("Window disabled in Play Mode", EditorStyles.boldLabel);
            refresh = true;
            return;
        }

        GUILayout.Label ("Parallax Scene Designer", EditorStyles.largeLabel);
        if (refresh) {
            options.Refresh ();
            refresh = false;
        }
        options.OnGUI ();
    }
}
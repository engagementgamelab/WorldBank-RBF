using UnityEngine;
using UnityEditor;
using System.Collections;

public class ParallaxPreview : EditorWindow {

	Camera previewCamera;
	RenderTexture renderTexture;

	bool leftPressed = false;
	bool rightPressed = false;

	[MenuItem ("Window/Parallax Preview")]
	static void Init () {
        EditorWindow editorWindow = GetWindow<ParallaxPreview> ();
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show ();
    }

    void OnEnable () {
    	FindCamera ();	
    }

    void Update () {
    	
    	FindCamera ();
    	if (previewCamera != null) {
    		previewCamera.targetTexture = renderTexture;
    		previewCamera.Render ();
    		previewCamera.targetTexture = null;
    	}

    	if (renderTexture != null && renderTexture.width != position.width || renderTexture.height != position.height) {
	        UpdateRenderTexture ();
        }
    }

    void OnGUI () {
        if (!EditorState.InEditMode) {
            GUILayout.Label ("Editor disabled in play mode");
            return;
        }
        GUI.DrawTexture (new Rect (0f, 0f, position.width, position.height), renderTexture);
        EditorGUILayout.BeginHorizontal ();
        // TODO: Replace with scrollbar
        if (GUILayout.RepeatButton ("<-")) {
        	previewCamera.transform.SetPositionX (Mathf.Max (0, previewCamera.transform.position.x-1));
        } 
        if (GUILayout.RepeatButton ("->")) {
        	previewCamera.transform.SetPositionX (previewCamera.transform.position.x+1);
        } 
        EditorGUILayout.EndHorizontal ();
    }

    void UpdateRenderTexture () {
    	renderTexture = new RenderTexture (
			(int)position.width,
			(int)position.height,
			(int)RenderTextureFormat.ARGB32);
    }

    void FindCamera () {
    	if (previewCamera == null) {
    		GameObject cam = GameObject.Find ("PreviewCamera");
    		if (cam != null) {
	    		previewCamera = cam.GetComponent<Camera> ();
	    		UpdateRenderTexture ();
    		}
    	}
        if (renderTexture == null) {
            UpdateRenderTexture ();
        }
    }
}

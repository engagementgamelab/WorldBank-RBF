using UnityEngine;
using UnityEditor;
using System.Collections;

public class ParallaxPreview : EditorWindow {

	Camera previewCamera;
	RenderTexture renderTexture;

    float xMin = 0f;
    float xMax;
    float xPos = 0f;

    float zMin = 0f;
    float zMax = 9.5f;
    float zPos = 0f;

	[MenuItem ("Window/Parallax Preview")]
	static void Init () {
        EditorWindow editorWindow = GetWindow<ParallaxPreview> ();
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show ();
    }

    void OnEnable () {
    	FindCamera ();
        xMax = ParallaxLayerManager.Instance.FurthestLayer.RightMax;
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
            GUILayout.Label ("Preview disabled in play mode");
            return;
        }

        previewCamera.transform.SetPositionX (xPos);
        previewCamera.transform.SetPositionZ (zPos);

        GUI.DrawTexture (new Rect (0f, 0f, position.width, position.height), renderTexture);
        xPos = GUILayout.HorizontalScrollbar (xPos, 1f, xMin, xMax, new GUILayoutOption[0]);

        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height (position.height-10f) };
        zPos = GUILayout.VerticalScrollbar (zPos, 1f, zMax, zMin, options);
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

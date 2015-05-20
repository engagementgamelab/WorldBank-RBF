using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PreviewCamera : MonoBehaviour {

	public RenderTexture RenderTexture {
		get { return Camera.targetTexture; }
	}

	Camera Camera {
		get { return GetComponent<Camera> (); }
	}
	
	void Awake () {
		#if !UNITY_EDITOR
		gameObject.SetActive (false);
		#endif
	}

	#if UNITY_EDITOR
	void Update () {
		ToggleActive ();
	}

	void ToggleActive () {
		Camera.enabled = EditorState.InEditMode;
	}
	#endif
}

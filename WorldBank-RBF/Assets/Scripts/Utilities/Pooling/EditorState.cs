using UnityEngine;
using System;
using System.Collections;

// from: http://answers.unity3d.com/questions/295649/callback-for-when-an-object-is-deleted-from-the-sc.html

[ExecuteInEditMode]
public class EditorState : MonoBehaviour {
#if UNITY_EDITOR

	// Modified singleton implementation; will not spawn instances of itself, user _must_ create one manually in the scene.
	// This is because the code will try to find an instance between edit and play mode when objects are deleted, and it
	// creates an unwanted clone which persists when exiting play mode.
	private static EditorState _instance;
	private static object _lock = new object ();
	private static EditorState Instance {
		get { 
			lock (_lock) {
				if (_instance == null) {
					_instance = (EditorState)FindObjectOfType (typeof (EditorState));
				}
		 		return _instance;
			}
		}
	}
	 
	/// <summary>
	/// Returns whether or not the editor is in edit mode.
	/// </summary>
	public static bool InEditMode {
		get {
			if (Instance == null) {
				Debug.LogWarning ("An object is referencing EditorState but it does not exist in the scene. Please create an instance of it.");
				return false;
			}
			return Instance.inEditMode;
		}
	}

	[SerializeField]
	private bool inEditMode = false;
	[NonSerialized]
	private bool enteringPlayMode = false;
	[NonSerialized]
	private bool editModeCallbackAdded = false;
	[NonSerialized]
	private bool updateCallbackAdded = false;
	
	void OnEnable () {
		if (!editModeCallbackAdded) {
			editModeCallbackAdded = true;
			UnityEditor.EditorApplication.playmodeStateChanged += EditModeCallback;
		}
		if (!updateCallbackAdded) {
			updateCallbackAdded = true;
			UnityEditor.EditorApplication.update += UpdateCallback;
		}
	}

	private void EditModeCallback () {
		if (!Application.isPlaying && inEditMode)
			enteringPlayMode = true;
	}

	private void UpdateCallback () {
		if (inEditMode == Application.isPlaying && !enteringPlayMode)
			inEditMode = !inEditMode;
		else if (enteringPlayMode)
			inEditMode = false;
	}
#endif
}
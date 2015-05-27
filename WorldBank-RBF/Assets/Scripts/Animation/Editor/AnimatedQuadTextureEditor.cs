using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (AnimatedQuadTexture))]
public class AnimatedQuadTextureEditor : Editor {

	AnimatedQuadTexture _target = null;
	AnimatedQuadTexture Target {
		get { 
			if (_target == null) {
				_target = (AnimatedQuadTexture)target;
			}
			return _target;
		}
	}

	SerializedObject serializedTarget = null;
	SerializedObject SerializedTarget {
		get {
			if (serializedTarget == null) {
				serializedTarget = new SerializedObject (Target);
			}
			return serializedTarget;
		}
	}

	GUILayoutOption[] EmptyOptions {
		get { return new GUILayoutOption[0]; }
	}

	bool animating = false;
	float startTime;

	void OnEnable () {
		startTime = (float)EditorApplication.timeSinceStartup;
		EditorApplication.update += Update;
	}

	void OnDisable () {
		EditorApplication.update -= Update;
	}

	public override void OnInspectorGUI () {

		SerializedTarget.Update ();

		bool textureUpdated = DrawTextureProperty ();
		bool frameCountUpdated = DrawFrameCountProperty ();
		bool frameUpdated = DrawFrameProperty (
			SerializedTarget.FindProperty ("frameCount"));

		DrawRunStop ();
		bool speedUpdated = DrawSpeedProperty ();

		serializedTarget.ApplyModifiedProperties ();

		if (textureUpdated) {
			Target.Refresh ();
			Target.SetScale ();
		}
		if (frameCountUpdated) Target.SetScale ();
		if (frameUpdated) Target.SetOffset ();
	}

	void Update () {
		float deltaTime = (float)EditorApplication.timeSinceStartup - startTime;
		startTime = (float)EditorApplication.timeSinceStartup;
		if (animating) Target.Animate (deltaTime);
	}

	void DrawRunStop () {
		animating = SerializedTarget.FindProperty ("animating").boolValue;
		if (animating) {
			if (GUILayout.Button ("Stop")) {
				Target.StopAnimating ();
			}
		} else {
			if (GUILayout.Button ("Run")) {
				Target.StartAnimating ();
			}
		}
	}

	bool DrawTextureProperty () {
		SerializedProperty tex = SerializedTarget.FindProperty ("texture");
		string initialName = (tex.objectReferenceValue == null)
			? ""
			: tex.objectReferenceValue.name;

		EditorGUILayout.PropertyField (tex, EmptyOptions);
		if (tex.objectReferenceValue == null) {
			return true;
		}

		string editName = tex.objectReferenceValue.name;
		return initialName != editName;
	}

	bool DrawFrameCountProperty () {
		SerializedProperty frameCount = SerializedTarget.FindProperty ("frameCount");
		int initialFrameCount = frameCount.intValue;
		EditorGUILayout.PropertyField (frameCount, EmptyOptions);
		int editFrameCount = frameCount.intValue;
		return initialFrameCount != editFrameCount;
	}

	bool DrawFrameProperty (SerializedProperty frameCount) {
		SerializedProperty frame = SerializedTarget.FindProperty ("frame");
		int initialFrame = frame.intValue;
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("<<")) {
			if (frame.intValue <= 0) {
				frame.intValue = frameCount.intValue-1;
			} else {
				frame.intValue --;
			}
		}
		if (GUILayout.Button (">>")) {
			if (frame.intValue >= frameCount.intValue-1) {
				frame.intValue = 0;
			} else {
				frame.intValue ++;
			}
		}
		EditorGUILayout.PropertyField (frame, EmptyOptions);
		int editFrame = frame.intValue;
		EditorGUILayout.EndHorizontal ();
		return initialFrame != editFrame;
	}

	bool DrawSpeedProperty () {
		SerializedProperty speed = SerializedTarget.FindProperty ("speed");
		float initialSpeed = speed.floatValue;
		EditorGUILayout.PropertyField (speed, EmptyOptions);
		float editSpeed = speed.floatValue;
		return initialSpeed != editSpeed;
	}
}

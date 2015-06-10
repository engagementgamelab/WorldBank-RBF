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
	[SerializeField] float minLim = 0f;
	[SerializeField] float maxLim = 20f;

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
		DrawFloatProperty ("speed");
		DrawIntervalRange ();

		serializedTarget.ApplyModifiedProperties ();

		if (textureUpdated) {
			Target.Refresh ();
			Target.SetScale ();
		}

		if (frameCountUpdated) Target.SetScale ();
		if (frameUpdated) Target.SetOffset ();
	}

	void Update () {
		#if PREVIEW_ANIMATIONS && UNITY_EDITOR
		float deltaTime = (float)EditorApplication.timeSinceStartup - startTime;
		startTime = (float)EditorApplication.timeSinceStartup;
		if (animating) Target.Animate (deltaTime);
		#endif
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

	void DrawIntervalRange () {
		SerializedProperty useInterval = SerializedTarget.FindProperty ("useInterval");
		useInterval.boolValue = EditorGUILayout.Toggle ("Random interval", useInterval.boolValue);
		if (!useInterval.boolValue) return;
		EditorGUILayout.BeginHorizontal ();
		SerializedProperty min = SerializedTarget.FindProperty ("intervalMin");
		SerializedProperty max = SerializedTarget.FindProperty ("intervalMax");
		float minv = min.floatValue;
		float maxv = max.floatValue;
		GUILayout.Label (min.floatValue.RoundToDecimal (1).ToString (), new GUILayoutOption[] { GUILayout.Width (20) });
		EditorGUILayout.MinMaxSlider (ref minv, ref maxv, minLim, maxLim);
		GUILayout.Label (max.floatValue.RoundToDecimal (1).ToString (), new GUILayoutOption[] { GUILayout.Width (20) });
		EditorGUILayout.EndHorizontal ();
		min.floatValue = minv;
		max.floatValue = maxv;
		EditorGUILayout.HelpBox ("When 'random interval' is enabled, the animation pauses for some amount of time between animation cycles. The number on the left is the minimum amount of time (in seconds) that the animation will pause. The number on the right is the maximum amount of time it will pause.", MessageType.Info);
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

	bool DrawFloatProperty (string name, float min=-1, float max=-1) {
		SerializedProperty val = SerializedTarget.FindProperty (name);
		float initialValue = val.floatValue;
		EditorGUILayout.PropertyField (val, EmptyOptions);
		float editValue = val.floatValue;
		return initialValue != editValue;
	}
}

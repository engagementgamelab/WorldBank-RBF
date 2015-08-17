using UnityEngine;
using UnityEditor;
using System.Collections;

public class MyCustomEditor<T> : Editor where T : MonoBehaviour {
	
	T _target = null;
	protected T Target {
		get { 
			if (_target == null) {
				_target = (T)target;
			}
			return _target;
		}
	}

	SerializedObject serializedTarget = null;
	protected SerializedObject SerializedTarget {
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

	protected virtual void Draw () {}

	public override void OnInspectorGUI () {
		Draw ();
		SerializedTarget.ApplyModifiedProperties ();
	}

	protected bool DrawBoolProperty (string name) {
		SerializedProperty val = SerializedTarget.FindProperty (name);
		bool initialValue = val.boolValue;
		EditorGUILayout.PropertyField (val, EmptyOptions);
		bool editValue = val.boolValue;
		return initialValue != editValue;
	}

	protected bool DrawStringProperty (string name) {
		SerializedProperty val = SerializedTarget.FindProperty (name);
		string initialValue = val.stringValue;
		EditorGUILayout.PropertyField (val, EmptyOptions);
		string editValue = val.stringValue;
		return initialValue != editValue;
	}

	protected bool DrawFloatProperty (string name) {
		SerializedProperty val = SerializedTarget.FindProperty (name);
		float initialValue = val.floatValue;
		EditorGUILayout.PropertyField (val, EmptyOptions);
		float editValue = val.floatValue;
		return initialValue != editValue;
	}

	protected string FindStringProperty (string name) {
		return SerializedTarget.FindProperty (name).stringValue;
	}
}

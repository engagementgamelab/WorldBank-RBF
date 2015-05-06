using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class ParallaxDesigner : EditorWindow, IEditorObjectDrawer<Example> {

	public Example Target { get; set; }

	[MenuItem ("Window/Parallax Designer")]
	static void Init () {
        EditorWindow editorWindow = GetWindow<ParallaxDesigner> ();
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show ();
    }

    void OnSelectionChange () {
    	SetTargetFromSelection ();
    	Repaint ();
    }

    void OnGUI () {
    	if (Target != null) {
    		GUILayout.Label (Target.name);
    		List<FieldInfo> fields = GetEditableFields ();
    		foreach (FieldInfo field in fields) {
    			EditorGUI.BeginChangeCheck ();
		        string value = EditorGUILayout.TextField (field.Name, (string)field.GetValue (Target));
		        if (EditorGUI.EndChangeCheck ())
		        	field.SetValue (Target, value);
    		}
    	}
    }

    void SetTargetFromSelection () {
    	
    	Object[] objects = Selection.objects;
    	if (objects.Length == 0) {
    		Target = null;
    		return;
    	}

    	for (int i = 0; i < objects.Length; i ++) {
    		
    		GameObject go = objects[i] as GameObject;
    		if (go == null) {
    			continue;
    		}

    		Example obj = go.GetScript<Example> ();
    		if (obj != null) {
    			Target = obj;
    			return;
    		}
    	}
    	Target = null;
    }

    List<FieldInfo> GetEditableFields () {
    	FieldInfo[] fields = Target.GetType ().GetFields ();
    	List<FieldInfo> editable = new List<FieldInfo> ();
    	for (int i = 0; i < fields.Length; i ++) {
    		object[] attributes = fields[i].GetCustomAttributes (typeof (WindowFieldAttribute), true);
    		if (attributes.Length > 0) {
    			editable.Add (fields[i]);
    		}
    	}
    	return editable;
    }
}

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class EditorObjectDrawer<T> where T : UnityEngine.Object {

	T target;
	public T Target { 
		get { return target; }
		set {
			target = value;
            if (target == null) {
                serializedTarget = null;
            } else {
                serializedTarget = new SerializedObject (target);
                properties = ExposeProperties.GetProperties (target);
            }
		}
	}

    SerializedObject serializedTarget = null;
    protected SerializedObject SerializedTarget {
    	get { return serializedTarget; }
    }

    public bool Selected {
    	get { return Target != null && serializedTarget != null; }
    }

    PropertyField[] properties;

    public void Save (string fileName="") {
	    ModelSerializer.Save (Target, fileName);
    }

    public void Load (string fileName="") {
		ModelSerializer.Load (Target, fileName);
	}

	public void DrawObjectProperties () {
		if (Selected) {
    		serializedTarget.Update ();
            Dictionary<MemberInfo, Attribute> members = GetMembersWithWindowAttribute (Target.GetType ());
            foreach (var member in members) {
                string memberName = member.Key.Name;
                SerializedProperty prop = serializedTarget.FindProperty (memberName);
                if (prop == null) {
                    ExposeProperties.Expose (memberName, properties);
                } else {
                    EditorGUILayout.PropertyField (prop, true, new GUILayoutOption[0]);
                }
            }
            serializedTarget.ApplyModifiedProperties ();
    	}
	}

    Dictionary<MemberInfo, Attribute> GetMembersWithWindowAttribute (System.Type type) {
        MemberInfo[] members = type.GetMembers ();
        Dictionary<MemberInfo, Attribute> membersWithAttribute = new Dictionary<MemberInfo, Attribute> ();
        for (int i = 0; i < members.Length; i ++) {
            object[] attributes = members[i].GetCustomAttributes (typeof (WindowExposedAttribute), true);
            if (attributes.Length > 0) {
                membersWithAttribute.Add (members[i], attributes[0] as Attribute);
            }
        }
        return membersWithAttribute;        
    }

    public void OnSelectionChange () {
    	SetTargetFromSelection ();
    }

    void SetTargetFromSelection () {
    	
    	UnityEngine.Object[] objects = Selection.objects;
    	if (objects.Length == 0) {
    		Target = null;
    		return;
    	}

    	for (int i = 0; i < objects.Length; i ++) {
    		
    		GameObject go = objects[i] as GameObject;
    		if (go == null) {
    			continue;
    		}

    		T obj = go.GetScript<T> ();
    		if (obj != null) {
    			Target = obj;
    			return;
    		}
    	}
    	Target = null;
    }
}

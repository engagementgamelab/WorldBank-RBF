using UnityEngine;

// Run only if inside editor
#if UNITY_EDITOR
using UnityEditor;
#endif

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
            #if UNITY_EDITOR
                if (target == null) {
                    serializedTarget = null;
                } else {
                    serializedTarget = new SerializedObject (target);
                    properties = ExposeProperties.GetProperties (target);
                }
            #endif
		}
	}


#if UNITY_EDITOR
    SerializedObject serializedTarget = null;
    protected SerializedObject SerializedTarget {
    	get { return serializedTarget; }
    }
#endif

    public bool Selected {
    	get { return Target != null 
                #if UNITY_EDITOR
                    && serializedTarget != null
                #endif
               ;
            }
    }

#if UNITY_EDITOR
    PropertyField[] properties;
#endif

    public void Save (string fileName="") {
	    ModelSerializer.Save (Target, fileName);
    }

    public void Load (string fileName="") {
    	ModelSerializer.Load (Target, fileName);
	}

	public void DrawObjectProperties (GUILayoutOption[] options) {
        #if UNITY_EDITOR
		if (Selected) {
    		serializedTarget.Update ();
            Dictionary<MemberInfo, Attribute> members = GetMembersWithWindowAttribute (Target.GetType ());
            foreach (var member in members) {
                string memberName = member.Key.Name;
                SerializedProperty prop = serializedTarget.FindProperty (memberName);
                if (prop == null) {
                    ExposeProperties.Expose (memberName, properties, options);
                } else {
                    EditorGUILayout.PropertyField (prop, true, options);
                }
            }
            serializedTarget.ApplyModifiedProperties ();
    	}
        #else
            return;
        #endif
	}

    Dictionary<MemberInfo, Attribute> GetMembersWithWindowAttribute (System.Type type) {
        MemberInfo[] members = type.GetMembers ();
        Dictionary<MemberInfo, Attribute> membersWithAttribute = new Dictionary<MemberInfo, Attribute> ();
        for (int i = 0; i < members.Length; i ++) {
            object[] attributes = members[i].GetCustomAttributes (typeof (ExposeInWindowAttribute), true);
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

        #if UNITY_EDITOR
          
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

        #else

            return;

        #endif
        
    }
}
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using JsonFx.Json;

public class ParallaxDesigner : EditorWindow, IEditorObjectDrawer<Example2> {

	Example2 target;
	public Example2 Target { 
		get { return target; }
		set {
			target = value;
			serializedTarget = (target == null) 
				? null 
				: new SerializedObject (target);
		}
	}

    SerializedObject serializedTarget = null;

    static int failsafe = 10;

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
    	if (Target != null && serializedTarget != null) {
    		GUILayout.Label (Target.name);
    		if (GUILayout.Button ("Save")) {
    			Save ();
    		}
    		if (GUILayout.Button ("Load")) {
    			Load ();
    		}
    		serializedTarget.Update ();
    		Dictionary<FieldInfo, Attribute> fields = GetFieldsWithAttribute (Target.GetType (), typeof (WindowFieldAttribute));
    		foreach (var field in fields) {
	    		SerializedProperty prop = serializedTarget.FindProperty (field.Key.Name);
	    		EditorGUILayout.PropertyField (prop, true, new GUILayoutOption[0]);
    		}
    		serializedTarget.ApplyModifiedProperties ();
    	}
    }

    void SetTargetFromSelection () {
    	
    	failsafe = 10;

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

    		Example2 obj = go.GetScript<Example2> ();
    		if (obj != null) {
    			Target = obj;
    			return;
    		}
    	}
    	Target = null;
    }


    void Save () {
        WriteJsonData (CreateModelFromObject (Target));
    }

    void Load () {
        Target = (Example2)SetObjectMembersFromModel (Target);
    }

    Dictionary<FieldInfo, Attribute> GetFieldsWithAttribute (System.Type type, System.Type attribute) {
    	FieldInfo[] fields = type.GetFields ();
    	Dictionary<FieldInfo, Attribute> fieldsWithAttribute = new Dictionary<FieldInfo, Attribute> ();
    	for (int i = 0; i < fields.Length; i ++) {
    		object[] attributes = fields[i].GetCustomAttributes (attribute, true);
    		if (attributes.Length > 0) {
    			fieldsWithAttribute.Add (fields[i], attributes[0] as Attribute);
    		}
    	}
    	return fieldsWithAttribute;
    }

    object CreateModelFromObject (System.Object obj) {

        if (obj == null) 
            return null;

        System.Type modelType = GetModelType (obj);
        object model = Activator.CreateInstance (modelType);
        List<PropertyInfo> modelProperties  = new List<PropertyInfo> (modelType.GetProperties ());
        model = ApplyModelProperties (obj, model, modelProperties);

        return model;
    }

    object ApplyModelProperties (object obj, object applyTo, List<PropertyInfo> modelProperties) {

        for (int i = 0; i < modelProperties.Count; i ++) {
           
            PropertyInfo p = modelProperties[i];
            object value = null;
            System.Type memberType = null;

            PropertyInfo targetProperty = obj.GetType ().GetProperty (p.Name);
            if (targetProperty != null) {
                value = targetProperty.GetValue (obj, null);
                memberType = targetProperty.PropertyType;
            }

            FieldInfo targetField = obj.GetType ().GetField (p.Name);
            if (targetField != null) {
                value = targetField.GetValue (obj);
                memberType = targetField.FieldType;
            }

            if (typeof (IEnumerable).IsAssignableFrom (memberType)) {
                
                // List
                if (value is IList 
                    && value.GetType ().IsGenericType 
                    && !IsFundamental (memberType.GetGenericArguments ()[0])) {
                    
                    IList list = (IList)value;
                    IEnumerable<object> o = list.Cast<object> ();
                    value = o.Select (x => CreateModelFromObject (x)).ToList ();

                // Array
                } else if (memberType.IsArray && !IsFundamental (memberType.GetElementType ())) {
                    IList list = (IList)value;
                    IEnumerable<object> o = list.Cast<object> ();
                    value = o.Select (x => CreateModelFromObject (x)).ToArray ();
                }

            } else {
                if (!IsFundamental (memberType)) {
                    value = CreateModelFromObject (value);
                } 
            }

            if (value == null) {
                continue;
            } else {
                memberType = value.GetType ();
            }

            if (value != null && memberType != null) {
                p.SetValue (
                    applyTo,
                    Convert.ChangeType (value, memberType),
                    null);
            } else {
                Debug.LogError (obj.GetType () + " does not include a field or property named " + p.Name);
            }
        }

        return applyTo;
    }

    object SetObjectMembersFromModel (object obj) {

        if (obj == null) 
            return null;

        System.Type modelType = GetModelType (obj);
        object model = ReadJsonData<object> ();
        Dictionary<string, object> dict = (Dictionary<string, object>)model;
        // TODO: go through dict and set Target's values to match the properties in the loaded model

        /*foreach (var d in dict) {

        }*/

        // model = dict.ToObject2 (modelType);
        // Debug.Log (model.GetType ());

        List<PropertyInfo> modelProperties = new List<PropertyInfo> (modelType.GetProperties ());
        object o = ApplyModelProperties (model, Target, modelProperties);

        return o;
    }

    System.Type GetModelType (object obj) {
        JsonSerializableAttribute j = (JsonSerializableAttribute) Attribute.GetCustomAttribute (
            obj.GetType (), 
            typeof (JsonSerializableAttribute));
        return j.modelType;
    }

    public void WriteJsonData (object obj) {
        string fileName = Target.GetType ().Name;
    	string path = Application.dataPath + "/Scripts/Utilities/JsonSerializable/Data/";
		var streamWriter = new StreamWriter (path + "" + fileName + ".json");
        streamWriter.Write(JsonWriter.Serialize (obj));
        streamWriter.Close();
    }

    public T ReadJsonData<T> () where T : class {
        string fileName = Target.GetType ().Name;
        string path = Application.dataPath + "/Scripts/Utilities/JsonSerializable/Data/";
        StreamReader streamReader = new StreamReader (path + "" + fileName + ".json");
        string data = streamReader.ReadToEnd ();
        streamReader.Close ();
        return JsonReader.Deserialize<T> (data);
    }

    bool IsFundamental (System.Type type) {
    	return type.IsPrimitive || type.Equals (typeof (string)) || type.Equals (typeof (DateTime));
    }
}

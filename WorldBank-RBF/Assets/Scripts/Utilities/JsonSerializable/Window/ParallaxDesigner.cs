using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

    void Save () {

    	/*Dictionary<FieldInfo, Attribute> fields = GetFieldsWithAttribute (typeof (JsonSerializableAttribute));
    	Dictionary<string, object> models = new Dictionary<string, object> ();
    	
    	foreach (var field in fields) {
    		
			FieldInfo info = field.Key;
    		JsonSerializableAttribute attribute = field.Value as JsonSerializableAttribute;
			System.Type modelType = attribute.modelType;
			string attributeName = modelType.Name;
			object model;

			if (models.ContainsKey (attributeName)) {
				model = models[attributeName];
			} else {
				model = Activator.CreateInstance (attribute.modelType);
				models.Add (attributeName, model);
			}

			if (typeof (IList).IsAssignableFrom (info.FieldType)) {
				foreach (var item in info.GetValue (Target) as IList) {
					System.Type itemType = item.GetType ();
					Debug.Log (itemType);
				}
			}

			PropertyInfo prop = modelType.GetProperty (info.Name);
			prop.SetValue (
				model,
				Convert.ChangeType (info.GetValue (Target), info.FieldType),
				null);
    	}

    	foreach (var model in models) {
	    	WriteJsonData (model.Value, model.Key);
    	}*/

    	WriteJsonData (GetModelFromObject (Target), "Example2");
    }

    object GetModelFromObject (System.Object obj) {

    	Dictionary<FieldInfo, Attribute> fields = GetFieldsWithAttribute (obj.GetType (), typeof (JsonSerializableAttribute));
    	if (fields.Count == 0)
    		return null;

    	JsonSerializableAttribute attribute;
    	System.Type modelType = null;
    	string attributeName;
    	object model = null;

    	foreach (var field in fields) {

    		FieldInfo info = field.Key;
			if (model == null) {
	    		attribute = field.Value as JsonSerializableAttribute;
				modelType = attribute.modelType;
				attributeName = modelType.Name;
				model = Activator.CreateInstance (attribute.modelType);
			}

            // Left off here:
            // this needs to handle lists/arrays of objects that also have json serializable fields
			/*if (typeof (IList).IsAssignableFrom (info.FieldType)) {
				IList l = info.GetValue (obj) as IList;
				IList models = CreateList (l[0].GetType ());
				foreach (var i in l) {
					System.Type itemType = i.GetType ();
					if (!IsFundamental (itemType)) {
                        System.Object o = GetModelFromObject (i);
						models.Add (o);
					}
				}
				if (models.Count > 0) {
					PropertyInfo prop2 = modelType.GetProperty (info.Name);
					prop2.SetValue (
						model,
						Convert.ChangeType (models, info.FieldType),
						null);
				}
			}*/

			PropertyInfo prop = modelType.GetProperty (info.Name);
			prop.SetValue (
				model,
				Convert.ChangeType (info.GetValue (obj), info.FieldType),
				null);
    	}

    	return model;
    }

    public IList CreateList(Type myType) {
	    Type genericListType = typeof(List<>).MakeGenericType(myType);
	    return (IList)Activator.CreateInstance(genericListType);
	}

    /*List<object> GetModelsFromObject (System.Object obj) {

    	Dictionary<FieldInfo, Attribute> fields = GetFieldsWithAttribute (typeof (JsonSerializableAttribute));
    	Dictionary<string, object> models = new Dictionary<string, object> ();

    	foreach (var field in fields) {
    		
			FieldInfo info = field.Key;
    		JsonSerializableAttribute attribute = field.Value as JsonSerializableAttribute;
			System.Type modelType = attribute.modelType;
			string attributeName = modelType.Name;
			object model;

			if (models.ContainsKey (attributeName)) {
				model = models[attributeName];
			} else {
				model = Activator.CreateInstance (attribute.modelType);
				models.Add (attributeName, model);
			}

			if (typeof (IList).IsAssignableFrom (info.FieldType)) {
				List<List<object>> customTypes = new List<List<object>> ();
				foreach (var item in info.GetValue (Target) as IList) {
					System.Type itemType = item.GetType ();
					if (IsFundamental (itemType)) continue;
					customTypes.Add (GetModelsFromObject (item));
				}
				if (customTypes.Count > 0) {

				}
			}

			PropertyInfo prop = modelType.GetProperty (info.Name);
			prop.SetValue (
				model,
				Convert.ChangeType (info.GetValue (Target), info.FieldType),
				null);
    	}

    	return new List<object> (models.Values);
    }*/

    public static void WriteJsonData (object obj, string fileName) {
    	string path = Application.dataPath + "/Scripts/Utilities/JsonSerializable/Data/";
		var streamWriter = new StreamWriter (path + "" + fileName + ".json");
        streamWriter.Write(JsonWriter.Serialize (obj));
        streamWriter.Close();
    }

    bool IsFundamental (System.Type type) {
    	return type.IsPrimitive || type.Equals (typeof (string)) || type.Equals (typeof (DateTime));
    }

    /*void SetFieldWithName (KeyValuePair<FieldInfo, Attribute> info, ref System.Object model) {
    	string name = info.Key.Name;
    	FieldInfo[] fieldInfo = model.GetType ().GetFields ();
    	// Debug.Log (fieldInfo.Length);
    	List<FieldInfo> modelFields = new List<FieldInfo> (fieldInfo);
    	// Debug.Log (modelFields.Count);
    	FieldInfo field = modelFields.Find (x => x.Name == name);
    	// Debug.Log (field);
    	field.SetValue (model, info.Key.GetValue (Target));
    }*/

    void Load () {

    }
}

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
        ApplyModelPropertiesToObject (Target);
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
        model = ApplyObjectPropertiesToModel (obj, model, modelProperties);

        return model;
    }

    object ApplyObjectPropertiesToModel (object obj, object applyTo, List<PropertyInfo> modelProperties) {

        for (int i = 0; i < modelProperties.Count; i ++) {
           
            PropertyInfo p = modelProperties[i];
            object value = null;
            System.Type memberType = null;

            PropertyInfo targetProperty = obj.GetType ().GetProperty (p.Name);
            if (targetProperty != null) {
                value = targetProperty.GetValue (obj, null);
                memberType = targetProperty.PropertyType;
            } else {
                FieldInfo targetField = obj.GetType ().GetField (p.Name);
                if (targetField != null) {
                    value = targetField.GetValue (obj);
                    memberType = targetField.FieldType;
                }
            }

            if (typeof (IEnumerable).IsAssignableFrom (memberType)) {
                
                IList ilist = (IList)value;
                IEnumerable<object> list = ilist.Cast<object> ().Select (x => CreateModelFromObject (x));

                // List
                if (value is IList 
                    && value.GetType ().IsGenericType 
                    && !IsFundamental (memberType.GetGenericArguments ()[0])) {
                    value = list.ToList ();

                // Array
                } else if (memberType.IsArray && !IsFundamental (memberType.GetElementType ())) {
                    value = list.ToArray ();
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

    void ApplyModelPropertiesToObject (object obj) {

        System.Type modelType = GetModelType (obj);
        System.Type objType = obj.GetType ();
        object model = ReadJsonData<object> ();
        Dictionary<string, object> dict = (Dictionary<string, object>)model;
        
        foreach (var val in dict) {
            string propName = val.Key;
            PropertyInfo property = modelType.GetProperty (propName);
            if (property == null) {
                // This happens because JsonFX serializes backing fields, 
                // but they aren't needed
                continue;
            }
            object value = val.Value;
            System.Type memberType = property.PropertyType;

            if (typeof (IEnumerable).IsAssignableFrom (memberType)) {
                
                IList ilist = (IList)value;
                List<object> list = ilist.Cast<object> ().ToList ();

                if (memberType.IsGenericType) {
                    System.Type oType = memberType.GetGenericArguments ()[0];
                    if (IsFundamental (oType)) {
                        value = ConvertObjectListToTypeList (
                            list, objType.GetField (propName).FieldType, oType);
                        memberType = value.GetType ();
                    } else {
                        SetObjectsFromModels (
                            objType.GetField (propName).GetValue (obj), list);
                        continue;
                    }
                } else {
                    if (!IsFundamental (memberType.GetElementType ())) {
                        SetObjectsFromModels (
                            objType.GetField (propName).GetValue (obj), list);
                        continue;
                    }
                }
            } else {
                if (!IsFundamental (memberType)) {
                    object o = objType.GetField (propName).GetValue (obj); // reference to object field on the object we're setting values to
                    SetObjectFromModel (o, value);
                    continue;
                }
            }

            SetMemberValue (obj, propName, value);
        }
    }

    void SetObjectsFromModels (object group, List<object> models) {
        IList ilist = (IList)group;
        List<object> list = ilist.Cast<object> ().ToList ();
        for (int i = 0; i < models.Count; i ++) {
            SetObjectFromModel (list[i], models[i]);
        }
    }

    void SetObjectFromModel (object obj, object model) {
        
        if (obj == null)
            return;

        System.Type modelType = GetModelType (obj);
        System.Type objType = obj.GetType ();
        Dictionary<string, object> dict = (Dictionary<string, object>)model;

        foreach (var val in dict) {
            string propName = val.Key;
            PropertyInfo property = modelType.GetProperty (propName);
            if (property == null) {
                continue;
            }

            object value = val.Value;
            System.Type memberType = property.PropertyType;

            if (typeof (IEnumerable).IsAssignableFrom (memberType)) {
                
                IList ilist = (IList)value;
                List<object> list = ilist.Cast<object> ().ToList ();

                if (memberType.IsGenericType) {
                    System.Type oType = memberType.GetGenericArguments ()[0];
                    if (IsFundamental (oType)) {
                        value = ConvertObjectListToTypeList (
                            list, objType.GetField (propName).FieldType, oType);
                        memberType = value.GetType ();
                    } else {
                        SetObjectsFromModels (
                            objType.GetField (propName).GetValue (obj), list);
                        continue;
                    }
                } else {
                    if (!IsFundamental (memberType.GetElementType ())) {
                        SetObjectsFromModels (
                            objType.GetField (propName).GetValue (obj), list);
                        continue;
                    }
                }
            } else {
                if (!IsFundamental (memberType)) {
                    object o = objType.GetField (propName).GetValue (obj);
                    SetObjectFromModel (o, value);
                    continue;
                }
            }

            SetMemberValue (obj, propName, value);
        }
    }

    void SetMemberValue (object obj, string memberName, object value) {
        
        System.Type objType = obj.GetType ();
        System.Type memberType = value.GetType ();
        
        PropertyInfo targetProperty = objType.GetProperty (memberName);
        if (targetProperty != null) {
            targetProperty.SetValue (
                obj, Convert.ChangeType (value, memberType), null);
        }


        FieldInfo targetField = objType.GetField (memberName);
        if (targetField != null) {
            targetField.SetValue (
                obj, Convert.ChangeType (value, memberType));
        }
    }

    object ConvertObjectListToTypeList (List<object> fromList, System.Type toListType, System.Type toItemType) {
        object newList = Activator.CreateInstance (toListType);
        IList l = (IList)newList;
        foreach (object ob in fromList) {
            l.Add (Convert.ChangeType (ob, toItemType));
        }
        return l;
    }

    System.Type GetModelType (object obj) {
        JsonSerializableAttribute j = (JsonSerializableAttribute) Attribute.GetCustomAttribute (
            obj.GetType (), 
            typeof (JsonSerializableAttribute));
        return j.modelType;
    }

    bool IsFundamental (System.Type type) {
        return type.IsPrimitive || type.Equals (typeof (string)) || type.Equals (typeof (DateTime));
    }

    public void WriteJsonData (object obj) {
        string fileName = Target.GetType ().Name;
    	string path = Application.dataPath + "/Scripts/Utilities/JsonSerializable/Data/";
		var streamWriter = new StreamWriter (path + "" + fileName + ".json");
        streamWriter.Write (JsonWriter.Serialize (obj));
        streamWriter.Close ();
    }

    public T ReadJsonData<T> () where T : class {
        string fileName = Target.GetType ().Name;
        string path = Application.dataPath + "/Scripts/Utilities/JsonSerializable/Data/";
        StreamReader streamReader = new StreamReader (path + "" + fileName + ".json");
        string data = streamReader.ReadToEnd ();
        streamReader.Close ();
        return JsonReader.Deserialize<T> (data);
    }
}


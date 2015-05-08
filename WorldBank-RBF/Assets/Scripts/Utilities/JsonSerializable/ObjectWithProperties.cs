using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

// Not going to use this approach, but keeping it as reference for now

public class ObjectWithProperties : MonoBehaviour {

	public Float jfloat = new Float ();
	public Int jint = new Int ();
	public Float happiness = new Float ();

	void Awake () {
		// jfloat.val = 0.98765f;
		// jint.val = 121;
		// happiness.val = 9999.919f;
	}

	void OnGUI () {
		if (GUILayout.Button ("Save")) {
			Save ();
		}
		if (GUILayout.Button ("Load")) {
			Load ();
		}
	}

	void Save () {
		FieldInfo[] info = this.GetType ().GetFields(BindingFlags.Instance | BindingFlags.Public);
		List<Property> props = new List<Property> ();
		for (int i = 0; i < info.Length; i ++) {

			if (info[i].FieldType.IsSubclassOf (typeof (Property))) {
				props.Add (info[i].GetValue (this) as Property);
			}
		}
		Properties propsClass = new Properties ();
		propsClass.properties = props;
		DataHandler.WriteJsonData (propsClass);
	}

	void Load () {
		FieldInfo[] info = this.GetType ().GetFields(BindingFlags.Instance | BindingFlags.Public);
		Properties propsClass = DataHandler.ReadJsonData<Properties> ();
		List<Property> properties = propsClass.properties;
		for (int i = 0; i < properties.Count; i ++) {
			System.Type propType = info[i].FieldType;
			if (propType.IsSubclassOf (typeof (Property))) {
				if (propType == typeof (Float)) {
					Float _float = new Float ();
					_float.val = properties[i].val;
					info[i].SetValue (this, _float);
				} else if (propType == typeof (Int)) {
					Int _int = new Int ();
					_int.val = properties[i].val;
					info[i].SetValue (this, _int);
				}
			}
		}
	}
}


public class Properties {
	public List<Property> properties { get; set; }
}
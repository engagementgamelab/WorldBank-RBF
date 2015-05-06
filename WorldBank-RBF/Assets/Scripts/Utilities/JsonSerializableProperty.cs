using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class JsonSerializableProperty : System.Object {
	
	[SerializeField] public object field;
	public object Property {
		get { return field; }
		set { field = value; }
	}
}

using UnityEngine;
using System.Collections;

public class JsonSerializableAttribute : PropertyAttribute {
	
	public readonly System.Type modelType;

	public JsonSerializableAttribute (System.Type modelType) {
		this.modelType = modelType;
	}
}

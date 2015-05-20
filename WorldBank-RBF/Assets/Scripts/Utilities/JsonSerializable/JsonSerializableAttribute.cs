using System;

[AttributeUsage (AttributeTargets.Class)]
public class JsonSerializableAttribute : System.Attribute {

	public readonly System.Type modelType;

	public JsonSerializableAttribute (System.Type modelType) {
		this.modelType = modelType;
	}
}

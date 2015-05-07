using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Example2 : MonoBehaviour {

	[WindowField, JsonSerializable (typeof (Models.Test3))]
	public List<Example> examples = new List<Example> ();

	// [WindowField, JsonSerializable (typeof (Models.Test3))]
	// public List<string> texts = new List<string> ();
}

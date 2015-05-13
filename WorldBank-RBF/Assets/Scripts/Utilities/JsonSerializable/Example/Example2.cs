using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[JsonSerializable (typeof (Models.Test3))]
public class Example2 : MonoBehaviour {

	// [WindowField, JsonSerializable (typeof (Models.Test3))]
	// public List<Example> examples = new List<Example> ();

	// [WindowField, JsonSerializable (typeof (Models.Test3))]
	// public List<string> texts = new List<string> ();
	/*[WindowField]	
	public int goop = 5;

	[WindowField]
	public List<string> texts = new List<string> ();

	[WindowField]
	public string[] blahs = new string[2];

	[WindowField]
	public List<Example> test2s = new List<Example> ();

	[WindowField]
	public Example[] test3s;
	
	[WindowField]
	public Example ex2;*/
}

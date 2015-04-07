using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class Startup {

	static Startup () {
		EditorApplication.update += Update;
	}

	static void Update () {
		if (ObjectPool.StartupLoad ()) {
			EditorApplication.update -= Update;
		}
	}
}

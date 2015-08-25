using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor (typeof (AmbienceZone))]
public class AmbienceZoneEditor : MyCustomEditor<AmbienceZone> {

	protected override void Draw () {
		
		DrawStringProperty ("context");

		string context = FindStringProperty ("context");
		if (!ValidateContext (context)) {
			if (context == "") {
				EditorGUILayout.HelpBox ("Please enter a valid context.", MessageType.Info);
			} else {
				EditorGUILayout.HelpBox ("There are no ambiences with a context called '" + context + "'", MessageType.Warning);
			}
			return;
		}

		DrawFloatProperty ("width");
		DrawFloatProperty ("offset");
		DrawFloatProperty ("fadeLength");
		if (GUILayout.Button ("Destroy this zone")) {
			ObjectPool.Destroy<AmbienceZone> (Target.Transform);
		}
	}

	bool ValidateContext (string context) {
		string city = FindStringProperty ("cityContext");
		List<string> contexts;
		if (AudioManager.Ambience.Contexts.TryGetValue (city, out contexts)) {
			return contexts.Contains (context);
		}
		return false;
	}
}

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (AmbienceZones))]
public class AmbienceZonesEditor : MyCustomEditor<AmbienceZones> {

	string PATH {
		get { return Application.dataPath + "/Scripts/Utilities/JsonSerializable/Data/"; }
	}

	protected override void Draw () {
		DrawStringProperty ("cityContext");
		if (GUILayout.Button ("Add new zone")) {
			Target.AddZone ();
		}
		if (GUILayout.Button ("Save")) {
			ModelSerializer.Save (Target, PATH + "test.json");
		}
	}
}
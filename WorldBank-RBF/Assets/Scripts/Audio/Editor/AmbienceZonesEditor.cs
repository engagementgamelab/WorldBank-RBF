using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (AmbienceZones))]
public class AmbienceZonesEditor : MyCustomEditor<AmbienceZones> {

	string PATH {
		// get { return Application.dataPath + "/Scripts/Utilities/JsonSerializable/Data/"; }
		get { return Application.dataPath + "/Resources/Config/PhaseOne/AmbienceZones/"; }
	}

	protected override void Draw () {
		
		DrawStringProperty ("cityContext");
		
		if (GUILayout.Button ("Add new zone")) {
			Target.AddZone ();
		}
		
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Save")) {
			ModelSerializer.Save (Target, PATH + FindStringProperty ("cityContext") + ".json");
		}

		if (GUILayout.Button ("Load")) {
			string loadPath = EditorUtility.OpenFilePanel ("Load ambience zones", PATH, "json");
			
			if (loadPath != "") {
				
				RemoveZones ();
				string path = loadPath
					.Replace (Application.dataPath + "/Resources/", "")
					.Replace (".json", "");

				ModelSerializer.Load (Target, path);
			}
		}
		GUILayout.EndHorizontal ();

		if (GUILayout.Button ("Reset")) {
			RemoveZones ();
		}
	}

	void RemoveZones () {
		Target.Reset ();
	}
}
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (AmbienceZones))]
public class AmbienceZonesEditor : MyCustomEditor<AmbienceZones> {

	string PATH {
		get { return Application.dataPath + "/Resources/Config/PhaseOne/AmbienceZones/"; }
	}

	protected override void Draw () {

		DrawStringProperty ("cityContext");

		string city = FindStringProperty ("cityContext");
		if (!ValidateContext (city)) {
			if (city == "") {
				EditorGUILayout.HelpBox ("Please enter a valid city name.", MessageType.Info);
			} else {
				EditorGUILayout.HelpBox ("There are no ambiences for a city called '" + city + "'", MessageType.Warning);
			}
			DrawLoadButton ();
			return;
		}

		if (GUILayout.Button ("Add new zone")) {
			Target.AddZone ();
		}
		
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Save")) {
			ModelSerializer.Save (Target, PATH + FindStringProperty ("cityContext") + ".json");
			EditorUtility.DisplayDialog ("Save successful", "The ambience zones for the city '" + city + "' was successfully saved.", "OK");
		}

		DrawLoadButton ();
		
		GUILayout.EndHorizontal ();

		if (GUILayout.Button ("Reset")) {
			ResetTarget ();
		}
	}

	void ResetTarget () {
		Target.cityContext = "";
		RemoveZones ();
		SerializedTarget.Update ();
	}

	void RemoveZones () {
		Target.Reset ();
	}

	bool ValidateContext (string cityContext) {
		return AudioManager.Ambience.Contexts.ContainsKey (cityContext);
	}

	void DrawLoadButton () {
		if (GUILayout.Button ("Load")) {
			string loadPath = EditorUtility.OpenFilePanel ("Load ambience zones", PATH, "json");
			
			if (loadPath != "") {
				
				RemoveZones ();
				string path = loadPath
					.Replace (Application.dataPath + "/Resources/", "")
					.Replace (".json", "");

				ModelSerializer.Load (Target, path);
				Target.OnLoad ();
				SerializedTarget.Update ();
			}
		}
	}
}
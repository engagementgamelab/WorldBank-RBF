using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RouteRecorder : MonoBehaviour {
	
	string RecordingPath {
		get { return Application.dataPath + "/routelines.txt"; }
	}

	public bool recordRoute = false;
	bool recording = false;
	public string city1 = "";
	public string city2 = "";
	public List<Vector3> routePositions = new List<Vector3> ();

	void Start () {
		#if UNITY_EDITOR
		if (System.IO.File.Exists (RecordingPath))
			System.IO.File.Delete (RecordingPath);
		#else
			gameObject.SetActive (false);
		#endif
	}

	#if UNITY_EDITOR
	void Update () {
		if (recording && !recordRoute) {
			if (!ValidateCityNames ()) {
				recordRoute = true;
				return;
			}
			
			string output =
				"routeLines.Add (" +
				"new Terminals (\"" + city1 + "\", \"" + city2 + "\"), " +
				"new List<Vector3> () {\n\t";

			for (int i = 0; i < routePositions.Count; i ++) {
				Vector3 p = routePositions[i];
				output += string.Format ("new Vector3 ({0}f, {1}f, {2}f)", p.x, p.y, p.z);
				if (i == routePositions.Count-1) {
					output += "\n});\n";
				} else {
					output += ",\n\t";
				}
			}
			System.IO.File.AppendAllText (RecordingPath, output);
			recording = false;
		}
		if (recordRoute && !recording) {
			routePositions.Clear ();
			recording = true;
		}
		if (recording && Input.GetMouseButtonDown (0)) {
			routePositions.Add (Input.mousePosition);
		}
	}

	bool ValidateCityNames () {
		if (CityNameValid (city1) && CityNameValid (city2)) {
			return true;
		} else {
			if (city1 == "" || city2 == "") {
				Debug.LogError ("Please input city names and try submitting again.");
			} else {
				Debug.LogError ("Invalid city name! Be sure '" + city1 + "' and '" + city2 + "' are valid names, then try submitting again.");
			}
			return false;
		}
	}

	bool CityNameValid (string name) {
		return name == "crup" 
			|| name == "valeria" 
			|| name == "capitol" 
			|| name == "kibari" 
			|| name == "zima" 
			|| name == "malcom" 
			|| name == "mile";
	}
	#endif
}

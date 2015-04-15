using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using System.IO;

public class LayerManager : MB {

	void Load () {
		//JsonWriter writer = new JsonWriter (new DataWriterSettings ());
	}

	void Save () {
		List<LayerSettingsJson> layers = ObjectPool.GetInstances<LayerSettings> ().ConvertAll (x => x.GetScript<LayerSettings> ().Json);
		PhaseOne phaseOne = new PhaseOne (layers);
		string PATH = Application.dataPath + "/Config/";
		string fileName = "phase_one.json";
		string data = JsonWriter.Serialize(phaseOne);
        if (!Directory.Exists(PATH)){
            Directory.CreateDirectory(PATH);
        }
        var streamWriter = new StreamWriter(PATH + fileName);
        streamWriter.Write(data);
        streamWriter.Close();
	}

	void OnGUI () {
		if (GUILayout.Button("SAVE")){
            Save ();
        }
	}
}

public class PhaseOne {

	List<LayerSettingsJson> layers;

	public PhaseOne (List<LayerSettingsJson> layers) {
		this.layers = layers;
	}
}
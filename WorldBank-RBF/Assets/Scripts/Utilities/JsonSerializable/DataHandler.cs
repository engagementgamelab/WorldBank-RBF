using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Json;

namespace JsonSerializable {

	public class DataHandler {

		static string PATH { get { return Application.dataPath + "/Scripts/Utilities/JsonSerializable/Data/"; } }

		public static void WriteJsonData (List<Property> props) {
			var streamWriter = new StreamWriter (PATH + "test.json");
	        streamWriter.Write(JsonWriter.Serialize (props));
	        streamWriter.Close();
	    }

	    public static void WriteJsonData (object obj) {
			var streamWriter = new StreamWriter (PATH + "test.json");
	        streamWriter.Write(JsonWriter.Serialize (obj));
	        streamWriter.Close();
	    }

	    public static T ReadJsonData<T> () where T : class {
	    	StreamReader streamReader = new StreamReader (PATH + "test.json");
	    	string data = streamReader.ReadToEnd ();
	    	streamReader.Close ();
	    	return JsonReader.Deserialize<T> (data);
	    }
	}
}
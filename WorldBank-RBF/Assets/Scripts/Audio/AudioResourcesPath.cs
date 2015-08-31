using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class AudioResourcesPaths {

	static string DirectoriesFilePath {
		get { return Application.dataPath + "/Resources/" + DirectoriesFileName; }
	}

	static string[] directories;
	public static string[] Directories {
		get {
			if (directories == null) {
				TextAsset t = (TextAsset)Resources.Load (DirectoriesFileName.Replace (".txt", ""), typeof (TextAsset));
				if (t == null)
					throw new System.Exception ("Could not find " + DirectoriesFileName + ", which is required to load the audio resources.");
				directories = Regex.Split (t.text, "\n");
			}
			return directories;
		}
	}

	static readonly List<string> fileOpened = new List<string> ();
	static readonly Regex regex = new Regex (@"(Audio).*");
	static readonly string DirectoriesFileName = "audiopaths.txt";

	public static void WriteDirectories (string[] lines) {
		foreach (string line in lines) {
			WriteToFile (DirectoriesFilePath, line);
		}
	}

	public static List<string> WriteFileNames (string root)	{
		string[] files = Directory.GetFiles (root, "*.mp3");
		List<string> resourcePaths = new List<string> ();
		foreach (string file in files) {
			resourcePaths.Add (WriteToFile (root + "/paths.txt", file));
		}
		return resourcePaths;
	}

	public static string[] GetFilesAtDirectory (string path) {
		TextAsset t = (TextAsset)Resources.Load (path + "/paths", typeof (TextAsset));
		return t == null ? new string[0] : Regex.Split (t.text, "\n");
	}

	public static string WriteToFile (string path, string line) {
		line = regex.Match (line).ToString ();
		line += "\n";
		if (!fileOpened.Contains (path)) {
			if (File.Exists (path))
				File.Delete (path);
			fileOpened.Add (path);
		}
		if (!File.Exists (path)) {
			File.WriteAllText (path, line);
		} else {
			File.AppendAllText (path, line);
		}
		return line == null ? "" : line.Replace ("\n", "");
	}
}

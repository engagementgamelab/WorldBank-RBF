using System.Text.RegularExpressions;

public class SfxItem : AudioItem {

	string name = "";
	public override string Name { 
		get { return name; }
	}

	string filePath;
	public override string FilePath {
		get { return filePath; }
		set {
			filePath = value;
			string[] n = filePath.Split ('/');
			name = n[n.Length-1];
			name = Regex.Match (name, @"[^_]*.$").ToString ();
		}
	}

	protected override PlaySettings Settings {
		get { return new PlaySettings (true, false); }
	}
}

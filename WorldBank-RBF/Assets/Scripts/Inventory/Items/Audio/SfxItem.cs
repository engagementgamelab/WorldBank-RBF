using System.Text.RegularExpressions;

public class SfxItem : AudioItem {

	string name = "";
	public override string Name { 
		get {
			if (name == "") {
				name = Regex.Match (Clip.name, @"[^_]*.$").ToString ();
			}
			return name;
		}
	}

	protected override PlaySettings Settings {
		get { return new PlaySettings (true, false); }
	}
}

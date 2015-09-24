using System.Text.RegularExpressions;

public class AmbienceItem : AudioItem {

	public string City { get; private set; }
	public string Context { get; private set; }

	protected override PlaySettings Settings {
		get { return new PlaySettings (true, true); }
	}

	string filePath;
	public override string FilePath {
		get { return filePath; }
		set {
			filePath = value;
			City = Regex.Match (Name, @"^.*?(?=_)").ToString ();
			Context = Regex.Match (Name, @"[^_]*.$").ToString ();
		}
	}
}

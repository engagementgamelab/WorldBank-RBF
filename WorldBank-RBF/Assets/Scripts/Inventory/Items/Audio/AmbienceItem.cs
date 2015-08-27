using UnityEngine;
using System.Text.RegularExpressions;

public class AmbienceItem : AudioItem {

	// public override string Name { get { return "Ambience"; } }

	public string City { get; private set; }
	public string Context { get; private set; }

	AudioClip clip;
	public override AudioClip Clip {
		get { return clip; }
		set { 
			clip = value; 
			City = Regex.Match (Name, @"^.*?(?=_)").ToString ();
			Context = Regex.Match (Name, @"[^_]*.$").ToString ();
		}
	}
}

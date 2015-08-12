using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class AmbienceItem : AudioItem {

	public override string Name { get { return "Ambience"; } }

	public string City { get; private set; }
	public string Context { get; private set; }

	public override AudioClip Clip {
		get { return clip; }
		set { 
			clip = value; 
			City = Regex.Match (clip.name, @"^.*?(?=_)").ToString ();
			Context = Regex.Match (clip.name, @"[^_]*.$").ToString ();
		}
	}
}

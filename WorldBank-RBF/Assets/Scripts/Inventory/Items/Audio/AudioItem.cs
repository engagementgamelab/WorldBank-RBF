using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class AudioItem : InventoryItem {
	
	protected struct PlaySettings {
		
		public bool allowMultiple;
		public bool loop;

		public PlaySettings (bool allowMultiple, bool loop) {
			this.allowMultiple = allowMultiple;
			this.loop = loop;
		}
	}

	string name = "";
	public override string Name { 
		get { 
			if (name == "") {
				string[] n = FilePath.Split ('/');
				name = n[n.Length-1];
			}
			return name;
		}
	}

	// example:
	// fem1farewell2
	// placetactic1

	// description and number: 	.*?(\d+)
	// just the number: 		(\d+)
	// just the description: 	^.*?(?=[\d+])
	// all descriptions: 		([a-zA-Z]+)

	// "qualities" are groups specified in the filename and take the format
	// [nameofgroup][#], with "nameofgroup" being a description of the quality
	// and # being and index (just used to make the filename unique).
	List<string> qualities;
	public List<string> Qualities {
		get {
			if (qualities == null) {
				qualities = new List<string> ();
				Regex regex = new Regex (@"([a-zA-Z]+)");
				MatchCollection matches = regex.Matches (Name);
				foreach (Match m in matches) {
					if (m.Success) qualities.Add (m.Value);
				}
			}
			return qualities;
		}
	}

	protected virtual PlaySettings Settings {
		get { return new PlaySettings (false, true); }
	}

	public virtual string FilePath { get; set; }

	AudioClip clip;
	public AudioClip Clip { 
		get {
			if (clip == null) {
				Debug.Log (FilePath);
				clip = (AudioClip)Resources.Load (FilePath);
				Debug.Log (clip);
			}
			return clip;
		}
	}

	float attenuation = 1f;
	public float Attenuation {
		get { return attenuation; }
		set {
			attenuation = value;
			foreach (AudioObject audioObject in audioObjects) {
				audioObject.Attenuation = attenuation;
			}
		}
	}

	readonly List<AudioObject> audioObjects = new List<AudioObject> ();

	public void Play () {
		AudioObject idleObject = GetIdleAudioObject ();
		if (idleObject != null)
			idleObject.Play (this, Settings.loop);
	}

	public void Stop () {
		foreach (AudioObject audioObject in audioObjects)
			audioObject.Stop ();
		audioObjects.Clear ();
	}

	AudioObject GetIdleAudioObject () {

		AudioObject idleObject = audioObjects.Find (x => !x.IsPlaying);
		
		if (idleObject == null && audioObjects.Count == 0 || Settings.allowMultiple) {
			idleObject = EditorObjectPool.Create<AudioObject> ();
			audioObjects.Add (idleObject);
		}

		return idleObject;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioItem : InventoryItem {
	
	protected struct PlaySettings {
		
		public bool allowMultiple;
		public bool loop;

		public PlaySettings (bool allowMultiple, bool loop) {
			this.allowMultiple = allowMultiple;
			this.loop = loop;
		}
	}

	public override string Name { get { return Clip.name; } }

	protected virtual PlaySettings Settings {
		get { return new PlaySettings (false, true); }
	}

	protected AudioClip clip;
	public virtual AudioClip Clip {
		get { return clip; }
		set { clip = value; }
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

	List<AudioObject> audioObjects = new List<AudioObject> ();

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

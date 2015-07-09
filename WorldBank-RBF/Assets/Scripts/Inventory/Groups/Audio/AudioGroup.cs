using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioGroup<T> : ItemGroup<T> where T : AudioItem, new () {

	protected List<AudioItem> AudioItems {
		get { return Items.ConvertAll (x => (AudioItem)x); }
	}

	protected struct Settings {
		
		public bool allowSimultaneous;
		public bool loop;

		public Settings (bool allowSimultaneous, bool loop) {
			this.allowSimultaneous = allowSimultaneous;
			this.loop = loop;
		}
	}

	List<AudioItem> playing = new List<AudioItem> ();

	public AudioGroup (string type) {
		
		string path = "Audio/" + type;

		// TODO: once there are lots of sounds in the game this will be very slow
		// it'll be better to lazy load audio or load over time
		AudioClip[] clips = Array.ConvertAll (Resources.LoadAll (path), x => (AudioClip)x);
		foreach (AudioClip clip in clips) {
			Add (new T () { Clip = clip });
		}
	}

	/*public AudioClip Play (AudioItem item) {
		if (!AllowSimultaneousClips) {

		}
	}

	void StopAll () {

	}*/
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioGroup<T> : ItemGroup<T> where T : AudioItem, new () {

	protected struct PlaySettings {
		
		public bool allowSimultaneous;
		
		public PlaySettings (bool allowSimultaneous) {
			this.allowSimultaneous = allowSimultaneous;
		}
	}

	protected virtual PlaySettings Settings {
		get { return new PlaySettings (false); }
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

	public void Play (AudioItem item) {
		if (!Settings.allowSimultaneous) {
			StopAll ();
		}
		item.Play ();
		playing.Add (item);
	}

	public void Stop (AudioItem item) {
		item.Stop ();
		playing.Remove (item);
	}

	void StopAll () {
		foreach (AudioItem item in playing) {
			item.Stop ();
		}
		playing.Clear ();
	}
}

using UnityEngine;
using System.Collections;

public class AudioObject : MB {

	AudioSource source = null;
	AudioSource Source {
		get {
			if (source == null) {
				source = GetComponent<AudioSource> ();
			}
			return source;
		}
	}

	public void Play (AudioClip clip, bool loop) {
		Source.clip = clip;
		Source.loop = loop;
		Source.Play ();
	}

	public void Stop () {
		Source.Stop ();
		ObjectPool.Destroy<AudioObject> (Transform);
	}
}

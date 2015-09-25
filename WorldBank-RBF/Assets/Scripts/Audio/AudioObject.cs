using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioObject : MB, IEditorPoolable {

	public int Index { get; set; }

	/*AudioSource source = null;
	AudioSource Source {
		get {
			// if (source == null && this != null) {
				// source = GetComponent<AudioSource> ();
			// }
			// return source;
			return GetComponent<AudioSource> ();
		}
	}*/

	public AudioSource Source;

	public bool IsPlaying { 
		get { 
			if (Source == null) return false;
			return Source.isPlaying;
		}
	}

	public float Attenuation { 
		get { return Source.volume; } 
		set { Source.volume = value; }
	}

	public AudioMixerGroup ambienceMixer;
	public AudioMixerGroup musicMixer;
	public AudioMixerGroup sfxMixer;

	public void Init () {}

	public void Play (AudioItem item, bool loop) {
		SetMixer (item.Group.ID);
		Source.clip = item.Clip;
		Source.loop = loop;
		Source.Play ();
		if (!loop) StartCoroutine (StopOnEndPlay ());
	}

	public void SetMixer (string type) {
		switch (type) {
			case "ambience": Source.outputAudioMixerGroup = ambienceMixer; break;
			case "music": Source.outputAudioMixerGroup = musicMixer; break;
			// TODO: should check for item's group root. as it is right now, no other audio groups can have subgroups (or else they'll end up in the sfx mixer)
			// case "sfx": 
			default: Source.outputAudioMixerGroup = sfxMixer; break;
		}
	}

	public void Stop () {
		if (Source != null) {
			Source.Stop ();
			EditorObjectPool.Destroy<AudioObject> (Transform);
		}
	}

	IEnumerator StopOnEndPlay () {
		while (gameObject.activeSelf && Source.isPlaying)
			yield return null;
		Stop ();
	}
}

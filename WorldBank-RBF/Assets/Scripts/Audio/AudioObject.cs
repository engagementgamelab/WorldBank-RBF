using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioObject : MB, IEditorPoolable {

	public int Index { get; set; }

	AudioSource source = null;
	AudioSource Source {
		get {
			if (source == null) {
				source = GetComponent<AudioSource> ();
			}
			return source;
		}
	}

	public bool IsPlaying { get { return Source.isPlaying; } }

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
	}

	public void SetMixer (string type) {
		switch (type) {
			case "ambience": Source.outputAudioMixerGroup = ambienceMixer; break;
			case "music": Source.outputAudioMixerGroup = musicMixer; break;
			case "sfx": Source.outputAudioMixerGroup = sfxMixer; break;
		}
	}

	public void Stop () {
		Source.Stop ();
		EditorObjectPool.Destroy<AudioObject> (Transform);
	}
}

using UnityEngine;
using UnityEngine.UI;

public enum AudioGroups {
	Ambience, Sfx, Music
}

public class AudioTrigger : MonoBehaviour {

	public string clipName = "buttonpresspositive";
	public string group = "ui";
	public AudioGroups type = AudioGroups.Sfx;

	void Awake () {
		Button b = GetComponent<Button> ();
		if (b != null) {
			b.onClick.AddListener (OnTriggerAudio);
		}
	}

	public void OnTriggerAudio () {
		switch (type) {
			case AudioGroups.Ambience: AudioManager.Ambience.Play (clipName, group); break;
			case AudioGroups.Sfx: AudioManager.Sfx.Play (clipName, group); break;
			case AudioGroups.Music: AudioManager.Music.Play (clipName, group); break;
		}
	}

	public void OnTriggerAudio (string clipName, string group) {
		this.clipName = clipName;
		this.group = group;
		OnTriggerAudio ();
	}
}

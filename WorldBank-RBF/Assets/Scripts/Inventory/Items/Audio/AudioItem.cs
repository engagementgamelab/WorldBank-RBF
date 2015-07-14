using UnityEngine;
using System.Collections;

public class AudioItem : InventoryItem {
	
	public override string Name { get { return "Audio"; } }

	protected AudioClip clip;
	public virtual AudioClip Clip {
		get { return clip; }
		set { clip = value; }
	}

	AudioObject audioObject = null;

	public void Play (bool loop) {
		audioObject = ObjectPool.Instantiate<AudioObject> ();
		audioObject.Play (Clip, loop);
	}

	public void Stop () {
		audioObject.Stop ();
	}
}

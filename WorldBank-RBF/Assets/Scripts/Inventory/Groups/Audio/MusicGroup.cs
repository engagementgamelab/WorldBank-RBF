using UnityEngine;
using System.Collections;

public class MusicGroup : AudioGroup<AudioItem> {

	public override string ID { get { return "music"; } }

	protected override PlaySettings Settings {
		get { return new PlaySettings (false); }
	}
	
	public MusicGroup () : base ("Music") {}

}

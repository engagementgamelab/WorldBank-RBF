using UnityEngine;
using System.Collections;

public class SfxGroup : AudioGroup<AudioItem> {

	public override string ID { get { return "sfxs"; } }

	protected override PlaySettings Settings {
		get { return new PlaySettings (true, false); }
	}

	public SfxGroup () : base ("SFX") {}
}

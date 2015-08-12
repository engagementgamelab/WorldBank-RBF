using UnityEngine;
using System.Collections;

public class SfxGroup : AudioGroup<AudioItem> {

	public override string ID { get { return "sfx"; } }

	protected override PlaySettings Settings {
		get { return new PlaySettings (true); }
	}

	public SfxGroup () : base ("SFX") {}
}

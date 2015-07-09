using UnityEngine;
using System.Collections;

public class SfxGroup : AudioGroup<AudioItem> {

	public override string ID { get { return "sfxs"; } }

	public SfxGroup () : base ("SFX") {}
}

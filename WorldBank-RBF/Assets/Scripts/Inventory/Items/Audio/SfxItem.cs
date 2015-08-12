using UnityEngine;
using System.Collections;

public class SfxItem : AudioItem {

	public override string Name { get { return "Sfx"; } }

	protected override PlaySettings Settings {
		get { return new PlaySettings (true, false); }
	}
}

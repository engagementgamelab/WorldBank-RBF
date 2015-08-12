using UnityEngine;
using System.Collections;

public class MusicItem : AudioItem {

	public override string Name { get { return "Music"; } }

	protected override PlaySettings Settings {
		get { return new PlaySettings (false, true); }
	}
}

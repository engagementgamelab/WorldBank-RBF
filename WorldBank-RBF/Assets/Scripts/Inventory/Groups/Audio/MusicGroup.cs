using UnityEngine;
using System.Collections;

public class MusicGroup : AudioGroup<AudioItem> {

	public override string ID { get { return "musics"; } }

	public MusicGroup () : base ("Music") {}
}

using UnityEngine;
using System.Collections;

public class AmbienceGroup : AudioGroup<AmbienceItem> {

	public override string ID { get { return "ambiences"; } }

	public AmbienceGroup () : base ("Ambience") {}

	public void Play (string city) {
		Play (AudioItems.Find (x => ((AmbienceItem)x).City == city));
	}
}

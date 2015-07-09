using UnityEngine;
using System.Collections;

public class AmbienceGroup : AudioGroup<AmbienceItem> {

	public override string ID { get { return "ambiences"; } }

	Settings settings = new Settings (false, true);

	public AmbienceGroup () : base ("Ambience") {}

	/*public AudioClip Play (string city) {

		AudioItems.Find (x => ((AmbienceItem)x).City == city);

	}*/

	public AudioClip Get (string city) {
		return AudioItems.Find (x => ((AmbienceItem)x).City == city).Clip;
	}
}

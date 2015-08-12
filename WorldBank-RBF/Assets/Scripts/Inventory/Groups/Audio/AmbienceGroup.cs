using UnityEngine;
using System.Collections;

public class AmbienceGroup : AudioGroup<AmbienceItem> {

	public override string ID { get { return "ambience"; } }

	protected override PlaySettings Settings {
		get { return new PlaySettings (true); }
	}

	public AmbienceGroup () : base ("Ambience") {}

	public AmbienceItem Play (string city, string context="") {
		AmbienceItem item = FindAmbienceWithContext (city, context);
		Play (item);
		return item;
	}

	public AmbienceItem Stop (string city, string context="") {
		AmbienceItem item = FindAmbienceWithContext (city, context);
		Stop (item);
		return item;
	}

	AmbienceItem FindAmbienceWithContext (string city, string context="") {
		return (context == "")
			? MyItems.Find (x => x.City == city)
			: MyItems.Find (x => x.City == city && x.Context == context);
	}
}

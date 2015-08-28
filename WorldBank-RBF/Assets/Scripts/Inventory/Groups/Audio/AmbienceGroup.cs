using System.Collections.Generic;

public class AmbienceGroup : AudioGroup<AmbienceItem> {

	public override string ID { get { return "ambience"; } }

	protected override PlaySettings Settings {
		get { return new PlaySettings (true); }
	}

	Dictionary<string, List<string>> contexts;
	public Dictionary<string, List<string>> Contexts {
		get {
			#if !UNITY_EDITOR
			if (contexts == null) {
			#endif
				
				contexts = new Dictionary<string, List<string>> ();
				
				foreach (AmbienceItem item in MyItems) {
					
					string city = item.City;
					string context = item.Context;

					if (!contexts.ContainsKey (city))
						contexts.Add (city, new List<string> ());

					contexts[city].Add (context);
				}
			#if !UNITY_EDITOR	
			}
			#endif
			return contexts;
		}
	}

	public AmbienceGroup () : base ("Ambience") {}

	public AmbienceItem PlayAmbience (string city, string context="") {
		AmbienceItem item = FindAmbienceWithContext (city, context);
		if (item != null) Play (item);
		return item;
	}

	public AmbienceItem StopAmbience (string city, string context="") {
		AmbienceItem item = FindAmbienceWithContext (city, context);
		if (item != null) Stop (item);
		return item;
	}

	AmbienceItem FindAmbienceWithContext (string city, string context="") {
		return (context == "")
			? MyItems.Find (x => x.City == city)
			: MyItems.Find (x => x.City == city && x.Context == context);
	}
}


public static class AudioManager {

	static Inventory inventory = null;

	public static Inventory Inventory {
		get {
			if (inventory == null) {
				inventory = new Inventory ();
				inventory.Add (new AmbienceGroup ());
				inventory.Add (new SfxGroup ());
				inventory.Add (new MusicGroup ());
			}
			return inventory;
		}
	}

	public static AmbienceGroup Ambience {
		get { return (AmbienceGroup)Inventory["ambience"]; }
	}

	public static MusicGroup Music {
		get { return (MusicGroup)Inventory["music"]; }
	}

	public static SfxGroup Sfx {
		get { return (SfxGroup)Inventory["sfx"]; }
	}
}
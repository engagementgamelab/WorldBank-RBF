using System.Collections.Generic;

public static class AudioManager {

	public static class Settings {
		public static readonly bool Mute = false;
		public static readonly Dictionary<string, bool> Mutes = new Dictionary<string, bool>
		{
			#if UNITY_EDITOR && !UNITY_WEBPLAYER
			{ "ambience", false },
			{ "music", false },
			{ "sfx", false }
			#else
			{ "ambience", false },
			{ "music", false },
			{ "sfx", false }
			#endif
		};
	}

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
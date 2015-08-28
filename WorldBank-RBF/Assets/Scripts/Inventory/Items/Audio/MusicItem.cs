
public class MusicItem : AudioItem {

	public override string Name { get { return Clip.name; } }

	protected override PlaySettings Settings {
		get { return new PlaySettings (false, true); }
	}
}

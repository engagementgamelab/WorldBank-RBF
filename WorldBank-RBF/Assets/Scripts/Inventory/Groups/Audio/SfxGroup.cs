
public class SfxGroup : AudioGroup<SfxItem> {

	public override string ID { get { return "sfx"; } }

	protected override PlaySettings Settings {
		get { return new PlaySettings (true); }
	}

	public SfxGroup () : base ("SFX") {}
	
}
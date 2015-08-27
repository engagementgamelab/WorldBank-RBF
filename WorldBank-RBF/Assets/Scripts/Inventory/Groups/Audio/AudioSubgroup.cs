
public class AudioSubgroup<T> : AudioGroup<T> where T : AudioItem, new () {

	public override string ID { get { return id; } }
	readonly string id;

	public AudioSubgroup (string id, string path) : base (path) {
		this.id = id;
	}
}


public class MusicManager : MB {

	void Start () {
		PlayerData.CityGroup.onUpdateCurrentCity += OnUpdateCurrentCity;
	}

	void OnUpdateCurrentCity (string city) {
		AudioManager.Music.Play (city);
	}
}

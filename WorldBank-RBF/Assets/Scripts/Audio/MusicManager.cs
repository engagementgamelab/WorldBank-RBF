using System.Collections.Generic;

public class MusicManager : MB {

	MusicGroup music;
	MusicGroup Music {
		get {
			if (music == null) {
				music = AudioManager.Music;
			}
			return music;
		}
	}

	void Start () {
		Events.instance.AddListener<ArriveInCityEvent> (OnArriveInCityEvent);
		PlayerData.CityGroup.onUpdateCurrentCity += OnUpdateCurrentCity;
	}

	void OnUpdateCurrentCity (string city) {
		List<AudioItem> currentlyPlaying = Music.Playing;
		if (currentlyPlaying.Count > 0) {
			Music.FadeOut (currentlyPlaying[0], 1f, () => {
				Music.Stop (currentlyPlaying[0]);
				// Play (city);
			});
		} else {
			// Play (city);
		}
	}

	void Play (string city) {
		Music.Play (city);
		if (Music.Playing.Count > 0)
			Music.FadeIn (city, 1f);
	}

	void OnArriveInCityEvent (ArriveInCityEvent e) {
		Play (e.City);
	}
}

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[JsonSerializable (typeof (Models.AmbienceZones))]
public class AmbienceZones : MB {

	public string cityContext;

	[HideInInspector]
	public List<AmbienceZone> zones = new List<AmbienceZone> ();

	List<Color> colors = new List<Color> {
		Color.white,
		Color.yellow,
		Color.red,
		Color.cyan,
		Color.green
	};

	void Start () {
		PlayerData.CityGroup.onUpdateCurrentCity += OnUpdateCurrentCity;
		SfxGroup s = AudioManager.Sfx;
	}

	public void AddZone () {
		AmbienceZone zone = EditorObjectPool.Create<AmbienceZone> ();
		zone.Parent = Transform;
		zone.Transform.Reset ();
		zone.color = Transform.childCount < colors.Count 
			? colors[Transform.childCount-1] 
			: Color.white;
		UpdateZones ();
	}

	public void Load (string path) {
		Reset ();
		ModelSerializer.Load (this, path);
		AmbienceZone[] zoneArr = GameObject.FindObjectsOfType (typeof (AmbienceZone)) as AmbienceZone[];
		zones = zoneArr.ToList ();
		foreach (AmbienceZone zone in zones) {
			zone.Parent = Transform;
		}
		UpdateZones ();
	}

	void LoadFromSymbol (string citySymbol) {
		Load ("Config/PhaseOne/AmbienceZones/" + citySymbol);
	}

	void UpdateZones () {
		
		if (Transform.childCount != zones.Count) {
			zones.Clear ();
			foreach (Transform child in Transform) {
				AmbienceZone zone = child.GetScript<AmbienceZone> ();
				zones.Add (zone);
			}
		}

		if (zones.Count < 2) return;
		for (int i = 1; i < zones.Count; i ++) {
			AmbienceZone prevZone = zones[i-1];
			zones[i].Transform.SetLocalPositionX (
				prevZone.LocalPosition.x + prevZone.offset + prevZone.width);
		}
	}

	void Update () {
		SetAttenuation ();
	}

	void SetAttenuation () {
		
		#if UNITY_EDITOR
		if (!EditorState.InEditMode) {
		#endif
			foreach (AmbienceZone zone in zones) {
				zone.SetAttenuation (MainCamera.Instance.Position.x);
			}
		#if UNITY_EDITOR
		}
		#endif
	}

	public void Reset () {
		cityContext = "";
		RemoveZones ();
	}

	public void RemoveZones () {
		EditorObjectPool.Destroy<AmbienceZone> (zones);
		zones.Clear ();
	}

	void OnDrawGizmos () {
		float cursor = MainCamera.Instance.Position.x;
		Gizmos.color = Color.red;
		Gizmos.DrawLine (new Vector3 (cursor, 0, 10), new Vector3 (cursor, 10, 10));
	}

	void OnUpdateCurrentCity (string city) {
		LoadFromSymbol (city);
	}
}

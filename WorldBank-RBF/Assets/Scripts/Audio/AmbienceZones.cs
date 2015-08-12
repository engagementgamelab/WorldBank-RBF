using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[JsonSerializable (typeof (Models.AmbienceZones))]
public class AmbienceZones : MB {

	public string cityContext;

	[HideInInspector]
	public List<AmbienceZone> zones = new List<AmbienceZone> ();

	List<Color> colors = new List<Color> () {
		Color.white,
		Color.yellow,
		Color.red,
		Color.cyan,
		Color.green
	};

	public void AddZone () {
		AmbienceZone zone = ObjectPool.Instantiate<AmbienceZone> ();
		zone.Parent = Transform;
		zone.Transform.Reset ();
		zone.color = Transform.childCount < colors.Count 
			? colors[Transform.childCount-1] 
			: Color.white;
		UpdateZones ();
	}

	void UpdateZones () {
		if (Transform.childCount != zones.Count) {
			zones.Clear ();
			foreach (Transform child in Transform) {
				AmbienceZone zone = child.GetScript<AmbienceZone> ();
				zones.Add (zone);
			}
		}
	}

	void Update () {

		SetAttenuation ();

		#if UNITY_EDITOR
		UpdateZones ();
		if (zones.Count < 2) return;
		for (int i = 1; i < zones.Count; i ++) {
			AmbienceZone prevZone = zones[i-1];
			zones[i].Transform.SetLocalPositionX (
				prevZone.LocalPosition.x + prevZone.offset + prevZone.width);
		}
		#endif
	}

	void SetAttenuation () {
		
		float cursor = MainCamera.Instance.Position.x;
		foreach (AmbienceZone zone in zones) {
			zone.SetAttenuation (cursor);
		}

		#if UNITY_EDITOR
		Debug.DrawLine (new Vector3 (cursor, 0, 10), new Vector3 (cursor, 10, 10), Color.red);
		#endif
	}
}

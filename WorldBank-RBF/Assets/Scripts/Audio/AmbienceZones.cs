using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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
		AmbienceZone zone = EditorObjectPool.Create<AmbienceZone> ();
		zone.Parent = Transform;
		zone.Transform.Reset ();
		zone.color = Transform.childCount < colors.Count 
			? colors[Transform.childCount-1] 
			: Color.white;
		UpdateZones ();
	}

	public void OnLoad () {
		AmbienceZone[] zoneArr = GameObject.FindObjectsOfType (typeof (AmbienceZone)) as AmbienceZone[];
		zones = zoneArr.ToList ();
		foreach (AmbienceZone zone in zones) {
			zone.Parent = Transform;
		}
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
		
		float cursor = MainCamera.Instance.Position.x;

		if (!EditorState.InEditMode) {
			foreach (AmbienceZone zone in zones) {
				zone.SetAttenuation (cursor);
			}
		}
	}

	public void Reset () {
		EditorObjectPool.Destroy<AmbienceZone> (zones);
		zones.Clear ();
	}

	void OnDrawGizmos () {
		float cursor = MainCamera.Instance.Position.x;
		Gizmos.color = Color.red;
		Gizmos.DrawLine (new Vector3 (cursor, 0, 10), new Vector3 (cursor, 10, 10));
	}
}

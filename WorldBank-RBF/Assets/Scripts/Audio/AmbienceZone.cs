﻿using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[JsonSerializable (typeof (Models.AmbienceZone))]
public class AmbienceZone : MB {

	public string context;
	public float offset = 0;
	public float width = 100;
	public float fadeLength = 25;
	public Color color = Color.white;
	
	AmbienceItem ambience;

	float attenuation = 0;
	float Attenuation {
		get { return attenuation; }
		set {
			attenuation = value;
			if (attenuation > 0f) {
				ambience = AudioManager.Ambience.Play (CityContext, context);
			} else {
				ambience = AudioManager.Ambience.Stop (CityContext, context);
			}
			if (ambience != null) ambience.Attenuation = attenuation;
		}
	}

	string CityContext {
		get { return Parent.GetScript<AmbienceZones> ().cityContext; }
	}

	float Start { get { return Position.x + offset; } }
	float End { get { return Start + width; } }
	float FadeStart { get { return Start - fadeLength; } }
	float FadeEnd { get { return End + fadeLength; } }

	public void SetAttenuation (float position) {
		if (position < FadeStart || position > FadeEnd) {
			Attenuation = 0;
		} else if (position < Start) {
			Attenuation = Mathf.InverseLerp (FadeStart, Start, position);
		} else if (position > End) {
			Attenuation = Mathf.InverseLerp (FadeEnd, End, position);
		} else {
			Attenuation = 1f;
		}
	}

	#if UNITY_EDITOR
	void Update () {

		width = Mathf.Max (0, width);
		fadeLength = Mathf.Max (0, fadeLength);

		Vector3 start = new Vector3 (Start, 0, 10);
		Vector3 end = new Vector3 (End, 0, 10);
		Vector3 fadeStart = new Vector3 (FadeStart, 10, 10);
		Vector3 fadeEnd = new Vector3 (FadeEnd, 10, 10);

		Debug.DrawLine (start, end, color);
		Debug.DrawLine (start, fadeStart, color);
		Debug.DrawLine (end, fadeEnd, color);
	}
	#endif
}

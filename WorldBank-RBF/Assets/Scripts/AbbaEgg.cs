using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbbaEgg : MonoBehaviour {

	public Image abbaImage;
	public InfoBox infoBox;

	string spelled = "";

	string[] insights = new [] {
		"You are the dancing queen\n\nYoung and sweet\n\nOnly seventeen",
		"See that girl\n\nWatch that scene\n\nDig in the dancing queen",
		"Chiquitita, you and I cry\n\nBut the sun is still in the sky and shining above you\n\nLet me hear you sing once more like you did before\n\nSing a new song, Chiquitita",
		"Sing a new song, Chiquitita\n\nTry once more like you did before\n\nSing a new song, Chiquitita",
		"Mamma mia, does it show again\n\nMy my, just how much I've missed you",
		"Money, money, money\n\nMust be funny\n\nIn the rich man's world",
		"Somewhere deep inside you must know I miss you,\n\nBut what can I say, rules must be obeyed",
		"Waterloo, I was defeated, you won the war\n\nWaterloo, Promise to love you for ever more\n\nWaterloo, Couldn't escape if I wanted to\n\nWaterloo, Knowing my fate is to be with you",
		"The stars were bright, Fernando\n\nThey were shining there for you and me\n\nFor liberty, Fernando",
		"If you see the wonder of a fairy tale\n\nYou can take the future even if you fail",
		"Facing twenty thousand of your friends\n\nHow can anyone be so lonely?",
		"Don't go wasting your emotion\n\nLay all your love on me",
		"My love is strong enough to last when things are rough\n\nIt's magic",
		"Without a song or a dance what are we?",
		"Without a song or a dance what are we?",
		"Without a song or a dance what are we?",
		"Without a song or a dance what are we?",
		"Without a song or a dance what are we?",
		"You seemed so far away though you were standing near\n\nYou made me feel alive but something died I fear",
		"Memories, good days, bad days\n\nThey'll be with me\n\nAlways"
	};

	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			spelled += "a";
		} else if (Input.GetKeyDown (KeyCode.B)) {
			spelled += "b";
		} else if (Input.anyKeyDown) {
			spelled = "";
		}
		if (spelled == "abba") {
			spelled = "";
			AudioManager.Music.Play ("abba-tune");
			CancelInvoke ("StopDancingQueen");
			Invoke ("StopDancingQueen", 12f);
			abbaImage.gameObject.SetActive (true);
		}
	}

	public void OnPressAbba () {
		abbaImage.gameObject.SetActive (false);
		int r = (int)Mathf.Round (Random.value * insights.Length-1);
		infoBox.Open ("ABBA moment of insight", "\n" + insights[r]);
	}

	void StopDancingQueen () {
		AudioManager.Music.Stop ("abba-tune");
	}
}

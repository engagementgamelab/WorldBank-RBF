using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenusManager : MonoBehaviour {

	public static string defaultScreen = "title";
	public static string gotoSceneOnLoad = "PhaseOne";

	public Transform title;
	public Transform register;
	public Transform phaseSelection;
	public Transform planSelection;
	public Transform loading;
	public Transform credits;

	Transform currentScreen;
	Transform previousScreen;
	bool sliding = false;

	readonly List<Transform> screenHistory = new List<Transform> ();

	int screenWidth = -1;
	int ScreenWidth {
		get {
			if (screenWidth == -1) {
				screenWidth = Screen.width;
			}
			return screenWidth;
		}
	}

	List<int> slidePositions;
	List<int> SlidePositions {
		get {
			if (slidePositions == null) {
				slidePositions = new List<int> ();
				slidePositions.Add (-ScreenWidth);
				slidePositions.Add (0);
				slidePositions.Add (ScreenWidth);
			}
			return slidePositions;
		}
	}

	Dictionary<string, Transform> screens;
	Dictionary<string, Transform> Screens {
		get {
			if (screens == null) {
				screens = new Dictionary<string, Transform> ();
				screens.Add ("title", title);
				screens.Add ("register", register);
				screens.Add ("phase", phaseSelection);
				screens.Add ("plan", planSelection);
				screens.Add ("loading", loading);
				screens.Add ("credits", credits);
			}
			return screens;
		}
	}

	void Awake () {

		Resolution[] resolutions = Screen.resolutions;
        foreach (Resolution res in resolutions) {
            print(res.width + "x" + res.height);
        }

		currentScreen = Screens[defaultScreen];
		SetActiveScreen (currentScreen);
		AudioManager.Music.Play ("title_theme");
	}

	public static void GotoScreen (string key) {
		defaultScreen = key;
		Application.LoadLevel ("Menus");
	}

	public void SetScreen (string key) {
		SetScreen (Screens[key]);
	}

	public void SetScreen (Transform screen) {
		
		if (sliding || screen == currentScreen) return;
		sliding = true;

		previousScreen = currentScreen;
		
		currentScreen = screen;
		currentScreen.gameObject.SetActive (true);
		currentScreen.SetPositionX (SlidePositions[2]);

		bool slideLeft = SlideLeft (currentScreen);
		if (!slideLeft) {
			RemoveLast ();
			RemoveLast ();
		}
		
		StartCoroutine (CoSlide (previousScreen, currentScreen, slideLeft,
			() => {
				SetActiveScreen (currentScreen);
				sliding = false;
				if (currentScreen == loading) {
					Application.LoadLevel (gotoSceneOnLoad);
				}
			}
		));
	}

	IEnumerator CoSlide (Transform screenOut, Transform screenIn, bool slideLeft, System.Action onEnd) {
		
		float time = 0.25f;
		float eTime = 0f;

		int outStart = SlidePositions[1];
		int outEnd = slideLeft ? SlidePositions[0] : SlidePositions[2];
		int inStart = slideLeft ? SlidePositions[2] : SlidePositions[0];
		int inEnd = SlidePositions[1];
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			screenOut.SetPositionX (Mathf.Lerp (outStart, outEnd, progress));
			screenIn.SetPositionX (Mathf.Lerp (inStart, inEnd, progress));
			yield return null;
		}

		screenOut.SetPositionX (outEnd);
		screenIn.SetPositionX (inEnd);
		onEnd ();
	}

	void SetActiveScreen (Transform screen) {
		screenHistory.Add (screen);
		foreach (var s in Screens)
			s.Value.gameObject.SetActive (s.Value == screen);
	}

	bool SlideLeft (Transform newScreen) {
		if (screenHistory.Count < 2)
			return true;
		return screenHistory[screenHistory.Count-2] != newScreen;
	}

	void RemoveLast () {
		screenHistory.RemoveAt (screenHistory.Count-1);
	}
}

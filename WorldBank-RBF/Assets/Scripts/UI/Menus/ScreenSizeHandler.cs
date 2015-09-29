using UnityEngine;
using System.Collections;

public class ScreenSizeHandler : MonoBehaviour {

	void Awake () {

		Resolution[] resolutions = Screen.resolutions;
        int lastResolution = resolutions.Length-1;
        Screen.SetResolution (resolutions[lastResolution].width, resolutions[lastResolution].height, false);

        MenusManager.GotoScreen ("title");
	}
}

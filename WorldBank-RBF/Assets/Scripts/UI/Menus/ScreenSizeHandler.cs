using UnityEngine;
using System.Collections;

public class ScreenSizeHandler : MonoBehaviour {

	void Awake () {

		Resolution[] resolutions = Screen.resolutions;
        int lastResolution = resolutions.Length-1;
        // if (resolutions[lastResolution].height )
        Screen.SetResolution (resolutions[lastResolution].width, resolutions[lastResolution].height, false);

        MenusManager.GotoScreen ("title");
	}
}

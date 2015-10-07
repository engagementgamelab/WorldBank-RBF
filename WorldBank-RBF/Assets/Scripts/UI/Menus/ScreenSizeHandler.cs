using UnityEngine;
using System.Collections;

public class ScreenSizeHandler : MonoBehaviour {

  void Awake () {

    // Resize only for desktop/standalone, otherwise just load title
    #if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN

      int screenHeight = Screen.currentResolution.height;
      Resolution[] resolutions = Screen.resolutions;
      Resolution targetResolution = resolutions[resolutions.Length-1];
      int maxHeight = targetResolution.height;

      int menuBarsHeight = 0;

      #if UNITY_STANDALONE_OSX
        menuBarsHeight = 44;
      #elif UNITY_STANDALONE_WIN
        menuBarsHeight = 40;
      #endif

      if (maxHeight-menuBarsHeight < screenHeight) {
      	if (resolutions.Length >= 2)
        	targetResolution = resolutions[resolutions.Length-2];
        else
        	targetResolution = resolutions[0];
      }

      Screen.SetResolution (targetResolution.width, targetResolution.height, false);

    #endif

    MenusManager.GotoScreen ("title");

  }
}
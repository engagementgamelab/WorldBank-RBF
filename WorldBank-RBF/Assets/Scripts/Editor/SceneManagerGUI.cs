/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 SceneManagerGUI.cs
 SceneManager GUI override.

 Created by Johnny Richardson on 8/1/15.
==============
*/

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneManager))]
public class TestOnInspector : Editor
{
	string[] environments = new string[] { "local", 
									       "development",
									       "staging",
									       "production"
									     };

	// Display game environments dropdown and pass choice to SceneManager
    public override void OnInspectorGUI()
    {
    
    	SceneManager _sceneManager = (SceneManager)target;
	    
	    // Draw the default inspector
	    DrawDefaultInspector();
	    
        GUILayout.Label ("Game Environment:");

	    _sceneManager.environmentIndex = EditorGUILayout.Popup(_sceneManager.environmentIndex, environments);
	    
	    // Update the selected choice in the underlying object
	    _sceneManager.environment = environments[_sceneManager.environmentIndex];
	    
	    // Save the changes back to the object
	    EditorUtility.SetDirty(target);

    }
    
}
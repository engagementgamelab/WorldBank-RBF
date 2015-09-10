/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 TutorialGUI.cs
 GUI for tutorial creation.

 Created by Johnny Richardson on 9/3/15.
==============
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;

[CustomEditor(typeof(TutorialScreen))]
public class TutorialGUI : Editor
{
	string[] layoutLabels = TutorialScreen.Positions.Keys.ToArray();
	string[] tooltipLabels;

	int tooltipIndex;

	Models.Tooltip[] tooltips;

	[HideInInspector]
	public bool spotlightEnabled;

	// Display game layoutLabels dropdown and pass choice to SceneManager
    public override void OnInspectorGUI()
    {
    
    	TutorialScreen _tutorialScreen = (TutorialScreen)target;
	    
	    // Draw the default inspector
	    // DrawDefaultInspector();

	    if (GUILayout.Button ("Load tooltips")) {
 
	        TextAsset dataJson = (TextAsset)Resources.Load("data", typeof(TextAsset));
			StringReader strData = new StringReader(dataJson.text);
	        
			string gameData = strData.ReadToEnd();

			strData.Close();		

			// Set global game data
			if(gameData != null && gameData.Length > 0)	
				DataManager.SetGameData(gameData);

			// Get tooltips
			tooltipLabels = DataManager.GetTooltipKeys();

		}

		if(tooltipLabels != null && tooltipLabels.Length > 0) {
			tooltipIndex = EditorGUILayout.Popup(tooltipIndex, tooltipLabels);

			// Update selected tooltip
		    if (tooltipLabels[tooltipIndex] != _tutorialScreen.TooltipKey)
				_tutorialScreen.TooltipKey = tooltipLabels[tooltipIndex]; 

	        GUILayout.Label ("Text:");

		   	_tutorialScreen.overlayText = EditorGUILayout.TextField(_tutorialScreen.overlayText, GUILayout.Height(90));

	        GUILayout.Label ("Overlay Location:");

		    _tutorialScreen.layoutIndex = EditorGUILayout.Popup(_tutorialScreen.layoutIndex, layoutLabels);

		    _tutorialScreen.spotlightEnabled = EditorGUILayout.Toggle("Spotlight On", _tutorialScreen.spotlightEnabled);

		    if(_tutorialScreen.spotlightEnabled) {
		    
		        GUILayout.Label ("Spotlight Position:");
			    _tutorialScreen.spotlightRect.x = EditorGUILayout.Slider("X", _tutorialScreen.spotlightRect.x, .7f, -2.72f);
			    _tutorialScreen.spotlightRect.y = EditorGUILayout.Slider("Y", _tutorialScreen.spotlightRect.y, .7f, -2.72f);

		        GUILayout.Label ("Spotlight Size:");
			    _tutorialScreen.spotlightRect.width = EditorGUILayout.Slider("Width", _tutorialScreen.spotlightRect.width, 1, 5);
			    _tutorialScreen.spotlightRect.height = EditorGUILayout.Slider("Height", _tutorialScreen.spotlightRect.height, 1, 5);

		        GUILayout.Label ("Mask Position:");
			    _tutorialScreen.maskRect.x = EditorGUILayout.Slider("X", _tutorialScreen.maskRect.x, 0, 800);
			    _tutorialScreen.maskRect.y = EditorGUILayout.Slider("Y", _tutorialScreen.maskRect.y, 0, -600);

		        GUILayout.Label ("Mask Size:");

			    _tutorialScreen.maskRect.width = EditorGUILayout.Slider("Width", _tutorialScreen.maskRect.width, 1, 500);
			    _tutorialScreen.maskRect.height = EditorGUILayout.Slider("Height", _tutorialScreen.maskRect.height, 1, 500);

				_tutorialScreen.SpotlightPosition();
			    _tutorialScreen.MaskPosition();
			
			}
			else
				_tutorialScreen.DisableSpotlight();
		    
		    // Update the selected choice in the underlying object
		    _tutorialScreen.Layout = layoutLabels[_tutorialScreen.layoutIndex];

		    string yamlPreview = String.Format("{0}:\n" + 
											    	"  overlay_location: \"{1}\"\n" + 
										    	    "  text: \"{2}\"", 
										    	    _tutorialScreen.TooltipKey, 
										    	    _tutorialScreen.overlayLocation,
										    	    _tutorialScreen.overlayText);


		    if(_tutorialScreen.spotlightEnabled) {

		    	yamlPreview += String.Format(
					    	   "\n  spotlight_position: [{0}, {1}]\n" + 
							   "  spotlight_size: [{2}, {3}]\n" + 
							   "  mask_position: [{4}, {5}]\n" + 
							   "  mask_size: [{6}, {7}]", 
					    	    _tutorialScreen.spotlightRect.x, _tutorialScreen.spotlightRect.y,
					    	    _tutorialScreen.spotlightRect.width, _tutorialScreen.spotlightRect.height,
					    	    _tutorialScreen.maskRect.x, _tutorialScreen.maskRect.y,
					    	    _tutorialScreen.maskRect.width , _tutorialScreen.maskRect.height);

		    }
									
			GUI.color = Color.cyan;
	        GUILayout.Label ("YAML Preview:", EditorStyles.boldLabel);
		    EditorGUILayout.LabelField(yamlPreview, GUILayout.Height(100));
							    	   
		    if (GUILayout.Button ("Copy YAML"))
				EditorGUIUtility.systemCopyBuffer = yamlPreview;
		}
	    
	    // Save the changes back to the object
	    EditorUtility.SetDirty(target);

    }
    
}
#endif
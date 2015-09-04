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
	public float spotlightXPos = 0;

	[HideInInspector]
	public float spotlightYPos = 0;

	[HideInInspector]
	public float spotlightWidth = 3;

	[HideInInspector]
	public float spotlightHeight = 3;

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
			// tooltips = 

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
		    
	        GUILayout.Label ("Spotlight Position:");

	        EditorGUILayout.BeginHorizontal ("X");
	        EditorGUILayout.PrefixLabel("X");
		    spotlightXPos = EditorGUILayout.Slider(spotlightXPos, -2, .6f);
		    EditorGUILayout.EndHorizontal();

	        EditorGUILayout.BeginHorizontal ("Y");
	        EditorGUILayout.PrefixLabel("Y");
		    spotlightYPos = EditorGUILayout.Slider(spotlightYPos, -2, .6f);
		    EditorGUILayout.EndHorizontal();

		    // Update the selected choice in the underlying object
		    _tutorialScreen.Layout = layoutLabels[_tutorialScreen.layoutIndex];

		    _tutorialScreen.SpotlightRect = new Rect(spotlightXPos, spotlightYPos, spotlightWidth, spotlightHeight);

		    string yamlPreview = String.Format("{0}:\n" + 
												    "  spotlight_position: [{1}, {2}]\n" + 
												    "  spotlight_size: [{3}, {4}]\n" + 
											    	"  overlay_location: \"{5}\"\n" + 
										    	    "  text: \"{6}\"", 
										    	    _tutorialScreen.TooltipKey, 
										    	    spotlightXPos,
										    	    spotlightYPos,
										    	    spotlightWidth,
										    	    spotlightHeight,
										    	    _tutorialScreen.overlayLocation,
										    	    _tutorialScreen.overlayText);
									
			GUI.color = Color.cyan;
	        GUILayout.Label ("YAML Preview:", EditorStyles.boldLabel);
		    EditorGUILayout.LabelField(yamlPreview, GUILayout.Height(90));
							    	   
		    if (GUILayout.Button ("Copy YAML"))
				EditorGUIUtility.systemCopyBuffer = yamlPreview;
		}
	    
	    // Save the changes back to the object
	    EditorUtility.SetDirty(target);

    }
    
}
#endif
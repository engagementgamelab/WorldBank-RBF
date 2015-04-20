/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 DialogueManager.cs
 Unity NPC dialog handler.

 Created by Johnny Richardson on 4/14/15.
==============
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;

public class DialogManager : MonoBehaviour {

	private static DialogManager _instance;

	// Singleton Manager
	public static DialogManager instance {

		get {

			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<DialogManager>();

				// Do not destroy on new scene
				DontDestroyOnLoad(_instance.gameObject);
			}

			return _instance;

		}

	}

	// Variable Definitions
	public Button btnLoadData;
	public Button btnPrefab;
	public NPCBehavior npcPrefab;
	public Button btnGoBack;
	
	public GameObject dialoguePanel;
	public GameObject dialogueBtnPanel;
	
	public Text dialogueTxt;

	void Awake() {

		if(_instance == null) {
			// This is the singleton
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else {
			// Ensure there is only one reference
			if(this != _instance)
				Destroy(this.gameObject);
		}

	}

	public void LoadDialogForCity(string city)
	{
		// Data tests
		DataManager.NPC[] itr = DataManager.GetDataForCity(city);

		int index = 1;

        foreach(DataManager.NPC npc in itr) {
        	GenerateNPC(npc, index);
        	index++;
        }

        btnLoadData.gameObject.SetActive(false);
	}

	public void HideCharacterDialog() {

		dialoguePanel.SetActive (false);

		CameraBehavior.ZoomOut();

	}

	/// <summary>
	/// Generate an NPC
	/// </summary>
	/// <param name="npcData">Instance of DataManager.NPC for this NPC</param>
	/// <param name="index">Index of this NPC</param>
	private void GenerateNPC(DataManager.NPC npcData, int index) {

		// Create NPC prefab instance
		NPCBehavior currentNpc = (NPCBehavior)Instantiate(npcPrefab);
	  
	    currentNpc.transform.localScale = Vector3.one;

	    // Temporary: set NPC position automatically
	    currentNpc.transform.position = new Vector3(.1f + (index/2), 0, 1.5f);

	    // Initialize this NPC
	    currentNpc.Initialize(npcData, gameObject);

	}

	public void OpenCharacterDialog(DataManager.NPC currNpc, string strDialogueKey) {

		string strInitial = currNpc.dialogue[strDialogueKey]["text"];
		
		// Match any characters in between [[ and ]]
		string strKeywordRegex = "(\\[)(\\[)(.*?)(\\])(\\])";

		foreach (Transform child in dialogueBtnPanel.transform)
		    GameObject.Destroy(child.gameObject);

		if(currNpc.dialogue[strDialogueKey].ContainsKey("unlocks"))
			strInitial += "\n\n<color=green>Unlocks</color>: " + currNpc.dialogue[strDialogueKey]["unlocks"];

		// Search for "keywords" in between [[ and ]]
		Regex regexKeywords = new Regex(strKeywordRegex, RegexOptions.IgnoreCase);
		MatchCollection keyMatches = regexKeywords.Matches(strInitial);
	
		foreach(Match m in keyMatches) {
		    if (m.Success)
			{
		        string strKeyword = m.Groups[3].ToString();

				CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
				TextInfo textInfo = cultureInfo.TextInfo;

				strKeyword = textInfo.ToTitleCase(strKeyword);

				Button currentNpc = (Button)Instantiate(btnPrefab);
			  
			    currentNpc.transform.parent = dialogueBtnPanel.transform;
			    currentNpc.transform.localScale = new Vector3(1, 1, 1);
			    Text label = currentNpc.transform.FindChild("Text").GetComponent<Text>();
				label.text = strKeyword;

			    currentNpc.onClick.AddListener(() => OpenCharacterDialog(currNpc, strKeyword));
			}
		}

		dialoguePanel.SetActive (true);
		dialogueTxt.text = strInitial.Replace("[[", "<color=orange>").Replace("]]", "</color>");

	}

/*	// Interface implementation for city selection
	void INPC.OnCitySelected (string strCityName) {

		// Load data for given city
		LoadDialogForCity(strCityName);

	}

	// Interface implementation for NPC selection
	void INPC.OnNPCSelected (DataManager.NPC currNpc) {

		// Load "Initial" dialogue for this NPC
		OpenCharacterDialog(currNpc, "Initial");

	}*/
}
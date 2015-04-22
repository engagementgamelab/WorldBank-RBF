/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 DialogueManager.cs
 Unity NPC dialog handler.

 Created by Johnny Richardson on 4/14/15.
==============
*/

/*using UnityEngine;
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
	public Button btnGoBack;
	public NPCBehavior npcPrefab;
	
	public GameObject dialoguePanel;
	public GameObject dialogueBtnPanel;

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
		Models.NPC[] itr = DataManager.GetDataForCity(city);

		int index = 1;

        foreach(Models.NPC npc in itr) {
        	GenerateNPC(npc, index);
        	index++;
        }

        btnLoadData.gameObject.SetActive(false);
	}

	public CanvasRenderer CreateGenericDialog(Transform canvasParent, string strInitialTxt) {

		GameObject canvasObject = Instantiate(Resources.Load("Prefabs/DialogueBox", typeof(GameObject))) as GameObject;
	    
		if(canvasParent != null)
		    canvasObject.transform.parent = canvasParent;

		CanvasRenderer diagRenderer = canvasObject.transform.Find("Box").gameObject.GetComponent<CanvasRenderer>() as CanvasRenderer;
		
		// diagRenderer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;  
		// diagRenderer.GetComponent<RectTransform>().pivot = new Vector2(-.5f, -.5f);

	    Text diagText = diagRenderer.transform.Find("Panel/Dialogue Text").GetComponent<Text>() as Text;
	    diagText.text = strInitialTxt;

	    return diagRenderer;

	}

	public void HideCharacterDialog() {

		dialoguePanel.SetActive (false);

		CameraBehavior.ZoomOut();

	}

	/// <summary>
	/// Generate an NPC
	/// </summary>
	/// <param name="npcData">Instance of Models.NPC for this NPC</param>
	/// <param name="index">Index of this NPC</param>
	private void GenerateNPC(Models.NPC npcData, int index) {

		// Create NPC prefab instance
		NPCBehavior currentNpc = (NPCBehavior)Instantiate(npcPrefab);

	    // Initialize this NPC
	    currentNpc.Initialize(npcData);

	}

	/// <summary>
	/// Open specified dialog for a given character
	/// </summary>
	/// <param name="currNpc">Instance of Models.NPC for this NPC</param>
	/// <param name="strDialogueKey">The key corresponding to the dialogue to show</param>
	public void OpenCharacterDialog(Models.NPC currNpc, string strDialogueKey) {

		string strInitial = currNpc.dialogue[strDialogueKey]["text"];
		
		// Match any characters in between [[ and ]]
		string strKeywordRegex = "(\\[)(\\[)(.*?)(\\])(\\])";

		CanvasRenderer diagRenderer = CreateGenericDialog(null, strInitial.Replace("[[", "<color=orange>").Replace("]]", "</color>"));

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

	}
}
*/
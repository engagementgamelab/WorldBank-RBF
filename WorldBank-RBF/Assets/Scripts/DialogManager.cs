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

public class DialogManager : MonoBehaviour, INPC {

	public Button btnLoadData;
	public Button btnPrefab;
	public NPCBehavior npcPrefab;
	public Button btnGoBack;
	
	public GameObject panel;
	public GameObject dialoguePanel;
	public GameObject dialogueBtnPanel;
	
	public Canvas dialogueContainer;
	public Text dialogueTxt;


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

	private void GenerateNPC(DataManager.NPC npcData, int index) {

		NPCBehavior currentNpc = (NPCBehavior)Instantiate(npcPrefab);
	  
	    currentNpc.transform.localScale = new Vector3(1, 1, 1);

	    // Temporary: set NPC position automatically
	    currentNpc.transform.position = new Vector3(.1f + (index/2), 0, 3);

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

				CultureInfo cultureInfo   = Thread.CurrentThread.CurrentCulture;
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

	void INPC.OnNPCSelected (DataManager.NPC currNpc) {

		OpenCharacterDialog(currNpc, "Initial");

	}
}
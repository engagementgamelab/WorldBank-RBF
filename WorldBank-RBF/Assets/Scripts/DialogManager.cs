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

	public Button btnPrefab;
	public Button btnGoBack;
	
	public GameObject panel;
	public GameObject dialoguePanel;
	public GameObject dialogueBtnPanel;
	
	public Text dialogueTxt;


	public void LoadDialogForCity(string city)
	{
		// Data tests
		DataManager.NPC[] itr = DataManager.GetDataForCity(city);

        foreach(DataManager.NPC npc in itr)
        	GenerateNPC(npc);
	}

	public void HideCharacterDialog(string city) {

		dialoguePanel.SetActive (false);

	}

	private void GenerateNPC(DataManager.NPC currNpc) {

		Button go = (Button)Instantiate(btnPrefab);
	  
	    go.transform.parent = panel.transform;
	    go.transform.localScale = new Vector3(1, 1, 1);

	    Text label = go.transform.FindChild("Text").GetComponent<Text>();
		
		label.text = currNpc.character;

	    go.onClick.AddListener(() => OpenCharacterDialog(currNpc, "Initial"));

	}

	private void OpenCharacterDialog(DataManager.NPC currNpc, string strDialogueKey) {

		foreach (Transform child in dialogueBtnPanel.transform) {
		    GameObject.Destroy(child.gameObject);
		}

		string strInitial = currNpc.dialogue[strDialogueKey]["text"];
		
		// Match any characters in between [[ and ]]
		string strKeywordRegex = "(\\[)(\\[)(.*?)(\\])(\\])";

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

				Button go = (Button)Instantiate(btnPrefab);
			  
			    go.transform.parent = dialogueBtnPanel.transform;
			    go.transform.localScale = new Vector3(1, 1, 1);
			    Text label = go.transform.FindChild("Text").GetComponent<Text>();
				label.text = strKeyword;

			    go.onClick.AddListener(() => OpenCharacterDialog(currNpc, strKeyword));
			}
		}

		dialoguePanel.SetActive (true);
		dialogueTxt.text = strInitial.Replace("[[", "<color=orange>").Replace("]]", "</color>");

	}
}
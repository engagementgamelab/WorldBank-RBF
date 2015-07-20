/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 ScenarioDecisionDialog.cs
 Scenario year break decision UI logic/rendering.

 Created by Johnny Richardson on 7/16/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScenarioDecisionDialog : GenericDialogBox {

	Models.ScenarioConfig _data;
	int _year = 0;

    /// <summary>
    /// Set the data for this card
    /// </summary>
    public Models.ScenarioConfig Data {
        set {

        	_data = value;

        	Content = (_year == 0) ? _data.prompt_year_1 : _data.prompt_year_2;

        	AddChoices();

        }
    }

    public int Year {
    	set {

    		_year = value;

    	}
    }

	public Text textCardPrompt;

	List<string> currentCardOptions;

	List<GenericButton> btnListChoices = new List<GenericButton>();

	void AddChoices() {

		string currentChoicesConcat = string.Join(" ", DataManager.ScenarioDecisions().ToArray());

		Dictionary<string, string>[] choiceData = _data.choices.Where(choice => !currentChoicesConcat.Contains(choice["text"])).ToArray();

		foreach(Dictionary<string, string> choice in choiceData) {

			string choiceTxt = choice["text"];
			string choiceVal = choice["load"];

			// Create buttons for all year end choices
			GenericButton btnChoiceYes = ObjectPool.Instantiate<GenericButton>();
			// GenericButton btnChoiceNo = ObjectPool.Instantiate<GenericButton>();
			
			btnChoiceYes.Text = choiceTxt;
			// btnChoiceNo.Text = choiceData[1]["text"];

			btnChoiceYes.Button.onClick.RemoveAllListeners();
			btnChoiceYes.Button.onClick.AddListener (() => OptionSelected(choiceTxt, choiceVal));

			btnListChoices.Add(btnChoiceYes);
			// btnListChoices.Add(btnChoiceNo);			

		}
		
		AddButtons<GenericButton>(btnListChoices, true);

	}

	// Scenario year end decision was selected
	void OptionSelected(string strOptionName, string strOptionValue) {

		// Update selected decisions
		DataManager.ScenarioDecisions(strOptionName);

		// Broadcast to affect current scenario path with the config value
		Events.instance.Raise(new ScenarioEvent(ScenarioEvent.DECISION_SELECTED, strOptionValue));

		Close();

	}
	
}
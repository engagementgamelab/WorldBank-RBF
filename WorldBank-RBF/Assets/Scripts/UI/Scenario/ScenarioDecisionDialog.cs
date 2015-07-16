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

	Models.YearEndCard _data;

    /// <summary>
    /// Set the data for this card
    /// </summary>
    public Models.YearEndCard Data {
        set {

        	_data = value;

        	AddChoices();

        }
    }

	public Text textCardPrompt;
	public Transform promptButtonGroup;

	List<string> currentCardOptions;

	List<GenericButton> btnListChoices = new List<GenericButton>();

	void AddChoices() {

		Dictionary<string, string>[] choiceData = _data.choices;

		// Create buttons for all year end choices
		GenericButton btnChoiceYes = ObjectPool.Instantiate<GenericButton>();
		GenericButton btnChoiceNo = ObjectPool.Instantiate<GenericButton>();
		
		btnChoiceYes.Text = choiceData[0]["text"];
		btnChoiceNo.Text = choiceData[1]["text"];

		// btnChoice.Button.onClick.AddListener (() => AdvisorSelected(charRef.symbol));

		btnListChoices.Add(btnChoiceYes);
		btnListChoices.Add(btnChoiceNo);
		
		AddButtons<GenericButton>(btnListChoices, false, promptButtonGroup);

	}
	
}
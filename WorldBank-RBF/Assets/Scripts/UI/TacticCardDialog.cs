/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 TacticCardDialog.cs
 Tactic dialog UI logic/rendering.

 Created by Johnny Richardson on 6/2/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TacticCardDialog : ScenarioCardDialog {

	public Models.TacticCard data;

	public List<GenericButton> btnListOptions = new List<GenericButton>();

	private string selectedOption;

	public override void AddOptions(List<string> options) {
	
		foreach(string option in options) {

			string optionName = option;

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
			
			btnChoice.Text = optionName;

			btnChoice.Button.onClick.RemoveAllListeners();

			if(option == "Observe")
				btnChoice.Button.onClick.AddListener (() => GetFeedback("observe"));
			else
				btnChoice.Button.onClick.AddListener (() => Events.instance.Raise(new ScenarioEvent(optionName.ToLower())));

			btnListOptions.Add(btnChoice);
		}

		AddButtons(btnListOptions);

	}
    
    public void GetResultOptions() {

    	Content = data.investigate;
	
		foreach(KeyValuePair<string, string> option in data.new_options) {

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();

			string optionKey = option.Key;
			
			btnChoice.Text = option.Value;

			btnChoice.Button.onClick.RemoveAllListeners();

			btnChoice.Button.onClick.AddListener (() => GetFeedback(optionKey));

			btnListOptions.Add(btnChoice);
		}

		AddButtons(btnListOptions);

    }
    
    private void GetFeedback(string optionChosen) {

    	Content = data.feedback[optionChosen];

    	RemoveButtons();

		GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
		btnChoice.Text = "Close";

		btnChoice.Button.onClick.RemoveAllListeners();
		btnChoice.Button.onClick.AddListener (() => Close());
		btnChoice.Button.onClick.AddListener (() => Events.instance.Raise(
														new ScenarioEvent(ScenarioEvent.TACTIC_CLOSED)
													)
											 );

		AddButtons(new List<GenericButton> { btnChoice });

    }

}
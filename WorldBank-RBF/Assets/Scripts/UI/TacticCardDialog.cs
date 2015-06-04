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

	private string selectedOption;
	private TimerUtils.Cooldown investigateCooldown;

	/*public override void AddOptions(List<string> options) {

		List<GenericButton> btnListOptions = new List<GenericButton>();
	
		foreach(string option in options) {

			string optionName = option;

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
			
			btnChoice.Text = optionName;

			btnChoice.Button.onClick.RemoveAllListeners();

			if(option == "Observe")
				btnChoice.Button.onClick.AddListener (() => GetFeedback("observe"));
			else if(option == "Investigate") {
				btnChoice.Button.onClick.AddListener (() => StartInvestigate());
				btnChoice.Button.onClick.AddListener (() => Events.instance.Raise(new ScenarioEvent("Investigate")));
			}			

			btnListOptions.Add(btnChoice);
		}

		AddButtons(btnListOptions);

	}*/
    
    public void GetResultOptions() {

		List<GenericButton> btnListOptions = new List<GenericButton>();

    	Content = data.investigate;
	
		foreach(KeyValuePair<string, string> option in data.new_options) {

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();

			string optionKey = option.Key;
			
			btnChoice.Text = option.Value;

			btnChoice.Button.onClick.RemoveAllListeners();

			btnChoice.Button.onClick.AddListener (() => GetFeedback(optionKey));

			btnListOptions.Add(btnChoice);
		}

		AddButtons(btnListOptions, false, choiceGroup);

    }

    public void StartInvestigate() {

    	Disable();

		investigateCooldown = new TimerUtils.Cooldown();
		
		investigateCooldown.Init(data.cooldown, new ScenarioEvent(ScenarioEvent.TACTIC_RESULTS));

		Events.instance.Raise(new ScenarioEvent("Investigate"));

    }

    
    public void GetFeedback(string optionChosen) {

    	Content = data.feedback[optionChosen];

		GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
		btnChoice.Text = "Close";

		btnChoice.Button.onClick.RemoveAllListeners();
		btnChoice.Button.onClick.AddListener (() => Close());
		btnChoice.Button.onClick.AddListener (() => Events.instance.Raise(
														new ScenarioEvent(ScenarioEvent.TACTIC_CLOSED)
													)
											 );

		AddButtons<GenericButton>(new List<GenericButton> { btnChoice });

    }

}
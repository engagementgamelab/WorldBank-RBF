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

	Models.TacticCard _data;

    /// <summary>
    /// Set the data for this card
    /// </summary>
    public Models.TacticCard Data {
        set {

        	_data = value;

        	Initialize();

        }
    }

	public Text tooltipTxt;
	
	public RectTransform investigatePanel;
	public GenericButton buttonInvestigate;
	public GenericButton buttonObserve;

	string selectedOption;

	bool open = false;
	bool close = true;
	bool finished = false;
	bool investigateDone = false;
	bool investigateFurther = false;

	Transform cardContainer;

	public override void Initialize() {

		Content = _data.initiating_dialogue;
		Header = _data.name;
/*
		GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();

		btnChoice.Text = "Invee"
		btnChoice.Button.onClick.RemoveAllListeners();
		btnChoice.Button.onClick.AddListener (() => AdvisorSelected(charRef.symbol));
*/
	}

	public void Init() {

		// if(cardContainer == null)
		// 	cardContainer = transform.GetChild(0);

		// cardContainer.localPosition = new Vector3(-Screen.width, 0, cardContainer.localPosition.z);

		open = false;
		close = true;

		// Show default icon
		// tooltipDoneImg.gameObject.SetActive(false);
		// tooltipAlertImg.gameObject.SetActive(true);
		
		// Show tooltip
		// tooltipDoneImg.transform.parent.gameObject.SetActive(true);

	}

	public void Animate(bool isFinished=false) {

		// Set to close
		if(open) {
			close = true;
			open = false;

			finished = isFinished;
			
			// if(finished)
			// 	tooltipDoneImg.transform.parent.gameObject.SetActive(false);
		}
		// Set to open
		else if(close) {

			open = true;
			close = false;

		}

	}
    
    public void GetResultOptions() {

    	investigateDone = true;

		List<GenericButton> btnListOptions = new List<GenericButton>();

    	string[] currentOptions = investigateFurther ? _data.further_options : _data.new_options;

    	Content = investigateFurther ? _data.investigate_further_dialogue : _data.investigate_dialogue;
	
		foreach(string option in currentOptions) {

			Models.Unlockable unlockableRef = DataManager.GetUnlockableBySymbol(option);

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
			
			btnChoice.Text = unlockableRef.title;

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => GetFeedback(option));

			btnListOptions.Add(btnChoice);
		}

		if(investigateFurther) {
			AppendButtons(btnListOptions);

			buttonInvestigate.gameObject.SetActive(false);
		}
		else
			AddButtons(btnListOptions, false, choicesGroup);

		// If we can't investigate more, remove button and show close
		if(_data.further_options == null) {
			buttonInvestigate.gameObject.SetActive(false);
			buttonObserve.Text = "Close";
		}
		
		// Show done icon
		/*tooltipDoneImg.gameObject.SetActive(true);
		tooltipClockImg.gameObject.SetActive(false);
		tooltipTxt.gameObject.SetActive(false);

		// Show choices
		actionsPanel.gameObject.SetActive(true);*/

    }

    public void StartInvestigate() {

    	// Disable();

    	investigateFurther = investigateDone;

    	int[] thisCooldown = investigateFurther ? _data.investigate_further_cooldown : _data.investigate_cooldown;

		Events.instance.Raise(new ScenarioEvent( "investigate" + (investigateFurther ? "_further" : ""), null, thisCooldown));

		// Listen for cooldown tick
		// Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);

		// animatorTactic.Play("TacticClose");
		// Animate();

    }

    
    public void GetFeedback(string optionChosen) {

    	Content = _data.feedback_dialogue[optionChosen];

		// Hide choices
		RemoveButtons <GenericButton>(choicesGroup);

		buttonObserve.Text = "Close";

		buttonObserve.Button.onClick.RemoveAllListeners();
		buttonObserve.Button.onClick.AddListener (() => Close() );
		buttonObserve.Button.onClick.AddListener (() => Events.instance.Raise(
														new ScenarioEvent(ScenarioEvent.TACTIC_CLOSED)
													)
											 );

		buttonInvestigate.gameObject.SetActive(false);

    }

}
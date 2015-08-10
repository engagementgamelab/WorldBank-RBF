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
	int _cardIndex;

    /// <summary>
    /// Set the data for this card
    /// </summary>
    public Models.TacticCard Data {
        set {

        	_data = value;

        	Initialize();

        }
        get {
        	return _data;
        }
    }

    public int Index {
    	set {
    		_cardIndex = value;
    	}
    }

	public Text tooltipTxt;
	
	public RectTransform investigatePanel;
	public GenericButton buttonInvestigate;
	public GenericButton buttonObserve;

	string selectedOption;

	bool open;
	bool finished;
	bool investigateDone;
	bool investigateFurther;

	Transform cardContainer;

	public override void Initialize() {

		Content = _data.initiating_dialogue;
		Header = _data.name;

		// Cleanup
		RemoveButtons <GenericButton>(choicesGroup);

	}

	public void ResetButtons() {

		buttonInvestigate.gameObject.SetActive(!investigateDone || (_data.further_options != null && !investigateFurther));

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

		Events.instance.Raise(new TacticsEvent( "investigate" + (investigateFurther ? "_further" : ""), null, thisCooldown));

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

		buttonObserve.Button.onClick.AddListener (() => Events.instance.Raise(
														new TacticsEvent(TacticsEvent.TACTIC_CLOSED, _cardIndex.ToString())
												 ));
		buttonObserve.Button.onClick.AddListener (() => ObjectPool.Destroy<TacticCardDialog>(transform) );

		buttonInvestigate.gameObject.SetActive(false);

    }

}
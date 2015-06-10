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

	public Image tooltipAlertImg;
	public Image tooltipClockImg;
	public Image tooltipDoneImg;
	public Text tooltipTxt;
	
	public RectTransform actionsPanel;

	public Animator animatorTactic;

	public float openCloseDuration;

	private string selectedOption;
	private TimerUtils.Cooldown investigateCooldown;

	private int cooldownTotal = 0;
	private int cooldownElapsed = 0;

	private bool open = false;
	private bool close = true;
	private bool finished = false;

	private Transform cardContainer;

	void Start() {

		cardContainer = transform.GetChild(0);

	}

	void Update() {

		 tooltipTxt.text = cooldownElapsed + "s";
		 
		if(close) {
		  	if(cardContainer.localPosition.x > -Screen.width)
				cardContainer.Translate(-(Screen.width / openCloseDuration) * Time.deltaTime * Vector3.right);
		}
		else if(open && cardContainer.localPosition.x < 0)
			cardContainer.Translate((Screen.width / openCloseDuration) * Time.deltaTime * Vector3.right);
		
	}

	public void Init() {

		if(cardContainer == null)
			cardContainer = transform.GetChild(0);

		cardContainer.localPosition = new Vector3(-Screen.width, 0, cardContainer.localPosition.z);

		open = false;
		close = true;

		// Show default icon
		tooltipDoneImg.gameObject.SetActive(false);
		tooltipAlertImg.gameObject.SetActive(true);

		// Show actions
		actionsPanel.gameObject.SetActive(true);
		activeBox.horizontalGroup.gameObject.SetActive(false);
		
		// Show tooltip
		tooltipDoneImg.transform.parent.gameObject.SetActive(true);

	}

	public void Animate(bool isFinished=false) {

		// Set to close
		if(open) {
			close = true;
			open = false;

			finished = isFinished;
			
			if(finished)
				tooltipDoneImg.transform.parent.gameObject.SetActive(false);
		}
		// Set to open
		else if(close) {
			open = true;
			close = false;
		}

	}
    
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

		AddButtons(btnListOptions, false, HorizontalGroup);
		
		// Show done icon
		tooltipDoneImg.gameObject.SetActive(true);
		tooltipClockImg.gameObject.SetActive(false);
		tooltipTxt.gameObject.SetActive(false);

		// Show choices
		actionsPanel.gameObject.SetActive(false);
		activeBox.horizontalGroup.gameObject.SetActive(true);

    }

    public void StartInvestigate() {

    	// Disable();

    	if(investigateCooldown == null)
			investigateCooldown = new TimerUtils.Cooldown();
		
	 	cooldownTotal = investigateCooldown.Init(data.cooldown, new ScenarioEvent(ScenarioEvent.TACTIC_RESULTS));

		Events.instance.Raise(new ScenarioEvent("Investigate"));

		// Show cooldown text
		tooltipDoneImg.gameObject.SetActive(false);
		tooltipAlertImg.gameObject.SetActive(false);
		tooltipClockImg.gameObject.SetActive(true);
		tooltipTxt.gameObject.SetActive(true);

		// Listen for cooldown tick
		Events.instance.AddListener<GameEvents.TimerTick>(OnCooldownTick);

		// animatorTactic.Play("TacticClose");
		Animate();

    }

    
    public void GetFeedback(string optionChosen) {

    	Content = data.feedback[optionChosen];

		GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
		btnChoice.Text = "Close";

		btnChoice.Button.onClick.RemoveAllListeners();

		btnChoice.Button.onClick.AddListener (() => Animate(true));
		btnChoice.Button.onClick.AddListener (() => Events.instance.Raise(
														new ScenarioEvent(ScenarioEvent.TACTIC_CLOSED)
													)
											 );

		AddButtons<GenericButton>(new List<GenericButton> { btnChoice });

		// Show choices
		actionsPanel.gameObject.SetActive(false);
		activeBox.horizontalGroup.gameObject.SetActive(true);
    }

    /// <summary>
    // Callback for TimerTick, filtering for type of event
    /// </summary>
    private void OnCooldownTick(GameEvents.TimerTick e) {

    	cooldownElapsed = cooldownTotal - e.secondsElapsed;

    }

}
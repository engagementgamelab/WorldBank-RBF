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
	
	public RectTransform investigatePanel;
	public RectTransform actionsPanel;
	public GenericButton buttonInvestigate;
	public GenericButton buttonObserve;

	public Animator animatorTactic;

	public float openCloseDuration;

	private string selectedOption;
	private TimerUtils.Cooldown investigateCooldown;

	private int cooldownTotal = 0;
	private int cooldownElapsed = 0;

	private bool open = false;
	private bool close = true;
	private bool finished = false;
	private bool investigateDone = false;
	private bool investigateFurther = false;

	private Transform cardContainer;

	void Start() {

		cardContainer = transform.GetChild(0);

	}

	// Used to animate card
	// TODO: Use animation
	void Update() {

		tooltipTxt.text = cooldownElapsed + "s";
		
		float step = (openCloseDuration * 1000f) * Time.deltaTime;
		float targetX = close ? -Screen.width : 0;

		cardContainer.localPosition = Vector3.MoveTowards(cardContainer.localPosition, new Vector3(targetX, 0, 0), step);
		
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

    	investigateDone = true;

		List<GenericButton> btnListOptions = new List<GenericButton>();

    	Dictionary<string, string> options = investigateFurther ? data.further_options : data.new_options;

    	Content = investigateFurther ? data.investigate_further_dialogue : data.investigate_dialogue;
	
		foreach(KeyValuePair<string, string> option in options) {

			string optionKey = option.Key;

			GenericButton btnChoice = ObjectPool.Instantiate<GenericButton>();
			
			btnChoice.Text = option.Value;

			btnChoice.Button.onClick.RemoveAllListeners();
			btnChoice.Button.onClick.AddListener (() => GetFeedback(optionKey));

			btnListOptions.Add(btnChoice);
		}

		if(investigateFurther) {
			AppendButtons(btnListOptions);

			buttonInvestigate.gameObject.SetActive(false);
		}
		else
			AddButtons(btnListOptions, false, HorizontalGroup);
		
		// Show done icon
		tooltipDoneImg.gameObject.SetActive(true);
		tooltipClockImg.gameObject.SetActive(false);
		tooltipTxt.gameObject.SetActive(false);

		// Show choices
		actionsPanel.gameObject.SetActive(true);

    }

    public void StartInvestigate() {

    	// Disable();

    	investigateFurther = investigateDone;

    	int[] thisCooldown = investigateFurther ? data.investigate_further_cooldown : data.investigate_cooldown;

    	if(investigateCooldown == null)
			investigateCooldown = new TimerUtils.Cooldown();
		
	 	cooldownTotal = investigateCooldown.Init(thisCooldown, new ScenarioEvent(ScenarioEvent.TACTIC_RESULTS));

		Events.instance.Raise(new ScenarioEvent( "Investigate" + (investigateFurther ? "_Further" : "") ));

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

    	Content = data.feedback_dialogue[optionChosen];

		// Hide choices
		actionsPanel.gameObject.SetActive(false);
		// activeBox.horizontalGroup.gameObject.SetActive(true);

		buttonObserve.Text = "Close";

		buttonObserve.Button.onClick.RemoveAllListeners();
		buttonObserve.Button.onClick.AddListener (() => Animate(true));
		buttonObserve.Button.onClick.AddListener (() => Events.instance.Raise(
														new ScenarioEvent(ScenarioEvent.TACTIC_CLOSED)
													)
											 );

		buttonInvestigate.gameObject.SetActive(false);
    }

    /// <summary>
    // Callback for TimerTick, filtering for type of event
    /// </summary>
    private void OnCooldownTick(GameEvents.TimerTick e) {

    	cooldownElapsed = cooldownTotal - e.secondsElapsed;

    }

}
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

	public Animator animatorTactic;

	private string selectedOption;
	private TimerUtils.Cooldown investigateCooldown;

	private int cooldownTotal = 0;
	private int cooldownElapsed = 0;

	private bool open = false;
	private bool close = true;

	private Transform cardContainer;

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

	void Start() {

		cardContainer = transform.GetChild(0);

		StartCoroutine(Init());

	}

	void Update() {

		 tooltipTxt.text = cooldownElapsed + "s";
		 
		 if(close) {
		  	if(cardContainer.localPosition.x > -Screen.width)
				cardContainer.Translate(-1500.0f * Time.deltaTime * Vector3.right);
			// else
			// 	ObjectPool.Destroy<TacticCardDialog>(transform);

		}
		 else if(open && cardContainer.localPosition.x < 0)
		 	cardContainer.Translate(1500.0f * Time.deltaTime * Vector3.right);
		
	}

	public void Animate() {

		if(open) {
			close = true;
			open = false;
		}
		else if(close) {
			open = true;
			close = false;
		}

	}

	private IEnumerator Init() {

		transform.SetAsFirstSibling();
	    transform.GetChild(0).localPosition = new Vector3(-Screen.width, 0, 0);
		
		yield return new WaitForSeconds(2);

		// animatorTactic.enabled = true;

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

    }

    public void StartInvestigate() {

    	// Disable();

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
		btnChoice.Button.onClick.AddListener (() => Animate());
		// btnChoice.Button.onClick.AddListener (() => animatorTactic.Play("TacticClose"));
		// btnChoice.Button.onClick.AddListener (() => ObjectPool.Destroy<TacticCardDialog>(transform));
		btnChoice.Button.onClick.AddListener (() => Events.instance.Raise(
														new ScenarioEvent(ScenarioEvent.TACTIC_CLOSED)
													)
											 );

		AddButtons<GenericButton>(new List<GenericButton> { btnChoice });

		// 

    }

    /// <summary>
    // Callback for TimerTick, filtering for type of event
    /// </summary>
    private void OnCooldownTick(GameEvents.TimerTick e) {

    	cooldownElapsed = cooldownTotal - e.secondsElapsed;

    }

}
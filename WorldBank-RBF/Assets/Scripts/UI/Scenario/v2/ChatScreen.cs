using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatScreen : GenericDialogBox {

	public Transform messagesContainer;
	public Scrollbar messagesScrollbar;
	public LayoutElement rightPanel;
	public Animator advisorsPanel;
	public Animator tabAnimator;

    public bool rightPanelActive = true;

	public List<ScenarioOptionButton> _btnListOptions;

	protected bool panelOpen = false;
	 
	public class ChatAction
	{
	    public string option;
	    public UnityAction action;
	}

	public void AddOptions(List<string> btnContent)  {

		AddOptions(btnContent, null, false);

	}

	public void AddOptions(List<string> btnContent, List<ChatAction> btnAction)  {

		AddOptions(btnContent, btnAction, false);

	}

	// TODO: This seems to break if there are more than two buttons???
	public virtual void AddOptions(List<string> btnContent, List<ChatAction> btnAction, bool clearAll)  {

		if (btnAction != null && (btnContent.Count != btnAction.Count))
			throw new System.Exception ("Button content count must match the button action count");

		if(clearAll)
			RemoveOptions ();


		int btnIndex = 0;

		foreach (string content in btnContent) {

			string buttonSymbol = content;

			ScenarioOptionButton btnChoice = _btnListOptions[btnIndex];
			btnChoice.gameObject.SetActive (true);

			// Reset animation state
			btnChoice.GetComponent<Button>().animator.CrossFade("Normal", 0f);

			btnChoice.Button.onClick.RemoveAllListeners();

			if(btnAction == null) {
				btnChoice.Text = DataManager.GetUnlockableBySymbol(buttonSymbol).title;
				btnChoice.Button.onClick.AddListener (() => OptionSelected(buttonSymbol));
				
				btnIndex ++;

				continue;
			}

			if(btnAction[btnIndex].action != null)
				btnChoice.Button.onClick.AddListener (btnAction[btnIndex].action);
			else if(btnAction[btnIndex].option != null)
				btnChoice.Button.onClick.AddListener (() => OptionSelected (btnAction[btnIndex].option));

			// SFX
			btnChoice.Button.onClick.AddListener (() => AudioManager.Sfx.Play ("sendmessage", "Phase2"));

			btnChoice.Text = content;
			btnIndex ++;

		}

	}

	public void RemoveResponses () {
		
		foreach (ScenarioChatMessage msg in messagesContainer.GetComponentsInChildren<ScenarioChatMessage>()) 
			msg.gameObject.SetActive (false);

	}

	public virtual void Clear () {

    	ObjectPool.DestroyChildren<ScenarioChatMessage>(messagesContainer, "Scenario");
    	ObjectPool.DestroyChildren<SystemMessage>(messagesContainer, "Scenario");
    	ObjectPool.DestroyChildren<IndicatorsMessage>(messagesContainer, "Scenario");

    	RemoveOptions ();

    }

	// Scenario option was selected
	protected virtual void OptionSelected(string strOptionSymbol) {

		Debug.Log("option selected: " + strOptionSymbol);

		// Broadcast to get card feedback
		Events.instance.Raise(new ScenarioEvent(ScenarioEvent.FEEDBACK, strOptionSymbol));
		
	}

	protected void RemoveOptions () {
		foreach (ScenarioOptionButton btn in _btnListOptions) {
			btn.gameObject.SetActive (false);
		}
	}

	protected void AddResponseSpeech(string strDialogue, Models.Character npc, bool initial=false, bool feedback=false, Dictionary<string, int> affects=null) {
		
		ScenarioChatMessage response = ObjectPool.Instantiate<ScenarioChatMessage>("Scenario");
		
		response.Initial = initial;
		response.Feedback = feedback;

		response.NPCName = npc.display_name;
		response.Content = strDialogue;
		response.NPCSymbol = npc.symbol;


		// Show indicators for feedback
		if(feedback && affects != null)
			response.feedbackIndicators.Display(affects);
		
		response.transform.SetParent(messagesContainer);
		response.transform.localScale = Vector3.one;
		
		if (gameObject.activeSelf)
			StartCoroutine (CoScrollToEnd ());
	}

	protected SystemMessage AddSystemMessage (string content) {

		SystemMessage message = ObjectPool.Instantiate<SystemMessage>("Scenario");
		message.Content = content;
		message.transform.SetParent(messagesContainer);
		message.transform.localScale = Vector3.one;
		
		if (gameObject.activeSelf)
			StartCoroutine (CoScrollToEnd ());

		return message;

	}

	protected void AddSystemButtons (List<string> btnContent, List<ChatAction> btnAction) {

		if (btnContent.Count != btnAction.Count)
			throw new System.Exception ("Systembutton content count must match the button action count");

		int btnIndex = 0;

		foreach (string content in btnContent) {
			
			SystemButton button = ObjectPool.Instantiate<SystemButton>("Scenario");
			button.Content = content;
			button.transform.SetParent(messagesContainer);
			button.transform.localScale = Vector3.one;

			button.Button.onClick.RemoveAllListeners();
			button.Button.onClick.AddListener (btnAction[btnIndex].action);
			
			btnIndex++;
		}
		
		if (gameObject.activeSelf)
			StartCoroutine (CoScrollToEnd ());

	}

	protected void RemoveSystemMessage(SystemMessage message) {

		message.gameObject.SetActive(false);

	}

	IEnumerator CoScrollToEnd () {

		yield return new WaitForFixedUpdate ();

		yield return new WaitForSeconds(.5f);

		float startValue = messagesScrollbar.value;
		float time = 0.5f;
		float eTime = 0f;

		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			messagesScrollbar.value = Mathf.Lerp (startValue, 0, progress);
			yield return null;
		}
	}
}

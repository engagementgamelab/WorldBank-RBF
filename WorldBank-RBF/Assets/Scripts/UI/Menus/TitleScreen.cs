﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreen : MonoBehaviour {

	public MenusManager menus;
	public Text email;
	public Text txtError;

	bool returningPlayer = true;

	void Awake () {
		Events.instance.AddListener<PlayerLoginEvent>(OnFormEvent);
		ClearError ();
	}

	void Start() {

		#if UNITY_EDITOR
			email.transform.parent.GetComponent<InputField>().text = "tester@elab.emerson.edu";
		#endif

	}

	void OnEnable () {
		ClearError ();
	}

	public void OnLogin () {
		string e = email.text;
		if (e == "") {
			ShowError ("Please enter an email address.");
		} else {
			ShowError ("Loading...");
			PlayerManager.Instance.Authenticate(e.Replace ("\n", ""));
		}
	}

	public void OnRegister () {
		menus.SetScreen ("register");
	}

	public void OnEmailInput () {
		ClearError ();
	}

	public void OnCredits () {
		menus.SetScreen ("credits1");
	}

	void OnFormEvent (PlayerLoginEvent e) {

    	if (!e.success)
	    		ShowError (e.error);
	    
	    else {
	    	returningPlayer = PlayerManager.Instance.PlanSubmitted;
	    	OnAuthenticate ();
	    }
    }

    void OnAuthenticate () {
    	if (returningPlayer) {
    		menus.SetScreen ("phase");
		} else {
			AudioManager.Music.FadeOut ("title_theme", 0.5f, () => {
					MenusManager.gotoSceneOnLoad = "PhaseOne";
					AudioManager.StopAll ();
					StartCoroutine (CoGotoLoad ());
				}
			);
		}
    }

	IEnumerator CoGotoLoad () {
		yield return new WaitForFixedUpdate ();
		ObjectPool.Clear ();
		menus.SetScreen ("loading");
	}

    void ShowError (string error) {
    	txtError.text = error;
    }

    void ClearError () {
    	txtError.text = "";
    }
}

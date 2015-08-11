/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 PlayerLoginRegisterUI.cs
 Player login and registration UI handling.

 Created by Johnny Richardson on 6/12/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PlayerLoginRegisterUI : MB {

	public Text txtEmail;
	public Text txtLocation;
	public Text txtUsername;
	public Text txtError;
	public InputField inputPassword;
	public InputField inputPasswordAgain;

	public GameObject objNameLocation;
	public GameObject objPassAgain;
	public GameObject btnLogin;
	public GameObject btnRegister;
	public GameObject btnNewUser;
	public GameObject btnGoBack;

	public delegate void AuthCallback(bool success);

	private bool loggedIn;
	private EventSystem system;

	void Start() {

		system = EventSystem.current;

		// Ensure UI is under our canvas
		// transform.SetParent(GameObject.FindGameObjectsWithTag("CanvasRoot")[0].transform);

		// GetComponent<RectTransform>().anchoredPosition = new Vector2(2011f, 1f);

		/*objPassAgain = transform.Find("PasswordAgainField").gameObject;
		btnLogin = transform.Find("LoginButton").gameObject;
		btnRegister = transform.Find("RegisterButton").gameObject;
		btnNewUser = transform.Find("NewUserButton").gameObject;
*/
		// Listen for cooldown tick

		Events.instance.AddListener<PlayerLoginEvent>(OnFormEvent);

		transform.SetAsFirstSibling();

	}

	public AuthCallback Callback { get; set; }

	public void Login() {

		PlayerManager.Instance.Authenticate(txtEmail.text, inputPassword.text);

	}

	public void Register() {

		PlayerManager.Instance.Register(txtEmail.text, txtUsername.text, txtLocation.text, inputPassword.text, inputPasswordAgain.text);

	}

	public void ToggleRegistration(bool open) {

		objNameLocation.SetActive(open);
		objPassAgain.SetActive(open);
		btnRegister.SetActive(open);
		btnGoBack.SetActive(open);

		btnLogin.SetActive(!open);
		btnNewUser.SetActive(!open);

	}

    private void OnFormEvent(PlayerLoginEvent e) {

    	if(!e.success) {
	    	txtError.text = e.error;
	    	
	    	txtError.gameObject.SetActive(true);
	    	Callback(false);
	    }
	    else {
	    	ObjectPool.Destroy<PlayerLoginRegisterUI>(transform);
	    	Callback(true);
	    }

    }

    private void Update () {

    	// Tab navigation - really rough
    	if (Input.GetKeyDown (KeyCode.Tab)) {
    		Selectable next = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)
    			? system.currentSelectedGameObject.GetComponent<Selectable> ().FindSelectableOnUp ()
    			: system.currentSelectedGameObject.GetComponent<Selectable> ().FindSelectableOnDown ();

    		if (next != null) {
    			InputField input = next.GetComponent<InputField> ();
    			if (input != null) {

    				// Navigate forwards
    				input.OnPointerClick (new PointerEventData (system));
    				system.SetSelectedGameObject (next.gameObject);
    				
    			} else {
    				
    				// Navigate backwards
    				next = Selectable.allSelectables[0];
    				system.SetSelectedGameObject(next.gameObject);
    			}
    		}
    	}
    }
}
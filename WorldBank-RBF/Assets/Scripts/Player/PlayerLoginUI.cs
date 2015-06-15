/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 PlayerLoginUI.cs
 Player login UI handling.

 Created by Johnny Richardson on 6/12/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerLoginUI : MB {

	public Text txtEmail;
	public Text txtError;
	public InputField inputPassword;
	public InputField inputPasswordAgain;

	GameObject objPassAgain;
	GameObject btnLogin;
	GameObject btnRegister;
	GameObject btnNewUser;

	/*void Start() {

		// Ensure UI is under our canvas
		transform.SetParent(GameObject.FindGameObjectsWithTag("CanvasRoot")[0].transform);

		// GetComponent<RectTransform>().anchoredPosition = new Vector2(2011f, 1f);

		objPassAgain = transform.Find("PasswordAgainField").gameObject;
		btnLogin = transform.Find("LoginButton").gameObject;
		btnRegister = transform.Find("RegisterButton").gameObject;
		btnNewUser = transform.Find("NewUserButton").gameObject;

	}*/

	public void Login() {

		PlayerManager.Instance.Authenticate(txtEmail.text, inputPassword.text);

	}

	public void Register() {

		PlayerManager.Instance.Register(txtEmail.text, inputPassword.text, inputPasswordAgain.text);

	}

	public void OpenRegistration() {

		objPassAgain.SetActive(true);
		btnRegister.SetActive(true);

		btnLogin.SetActive(false);
		btnNewUser.SetActive(false);


	}

}
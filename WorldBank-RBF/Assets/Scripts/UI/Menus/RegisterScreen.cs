using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RegisterScreen : MonoBehaviour {

	public MenusManager menus;
	public Text email;
	public Text firstName;
	public Text lastName;

	public void OnCancel () {
		menus.SetScreen ("title");
	}

	public void OnRegister () {
		/*PlayerManager.Instance.Register(
			email.text.Replace ("\n", ""), 
			txtUsername.text.Replace ("\n", ""), 
			txtLocation.text.Replace ("\n", ""), 
			inputPassword.text.Replace ("\n", ""), 
			inputPasswordAgain.text.Replace ("\n", ""));*/
	}
}

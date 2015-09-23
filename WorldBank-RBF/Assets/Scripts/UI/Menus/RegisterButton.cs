using UnityEngine.UI;

public class RegisterButton : LoginButton {

	public InputField firstName;
	public InputField lastName;

	public void OnUpdateName () {
		SetInteractable ();
	}

	protected override void SetInteractable () {
		Button.interactable = IsEmailValid && firstName.text != "" && lastName.text != "";
	}
}

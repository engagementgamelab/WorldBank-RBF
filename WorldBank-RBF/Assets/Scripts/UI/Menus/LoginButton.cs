using UnityEngine;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour {

	public InputField email;

	Button button = null;
	protected Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}

	protected bool IsEmailValid {
		get { return EmailField.IsValidEmail (email.text); }
	}

	void OnEnable () {
		SetInteractable ();
	}

	public void OnUpdateEmail () {
		SetInteractable ();	 
	}

	protected virtual void SetInteractable () {
		Button.interactable = IsEmailValid;
	}
}

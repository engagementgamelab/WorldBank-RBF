using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RegisterScreen : MonoBehaviour {

	public MenusManager menus;
	public Text email;
	public Text firstName;
	public Text lastName;

	EventSystem system;

	void Start () {
		system = EventSystem.current;
	}

	public void OnCancel () {
		menus.SetScreen ("title");
	}

	public void OnRegister () {
		PlayerManager.Instance.Register (
			email.text.Replace ("\n", ""), 
			firstName.text.Replace ("\n", ""), 
			lastName.text.Replace ("\n", ""));
		menus.SetScreen ("title");
	}

	void Update () {

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

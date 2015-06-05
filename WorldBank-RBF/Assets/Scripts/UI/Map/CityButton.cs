using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CityButton : MB {

	Button button = null;
	Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}

	Transform currentCityIndicator = null;
	Transform CurrentCityIndicator {
		get {
			if (currentCityIndicator == null) {
				currentCityIndicator = Transform.GetChild (0);
			}
			return currentCityIndicator;
		}
	}

	[SerializeField] bool unlocked = false;
	public bool Unlocked {
		get { return unlocked; }
		set { 
			unlocked = value;
			Button.interactable = unlocked;
		}
	}

	[SerializeField] bool currentCity = false;
	public bool CurrentCity {
		get { return currentCity; }
		set { 
			currentCity = value;
			CurrentCityIndicator.gameObject.SetActive (currentCity);
		}
	}

	public string symbol;

	void Awake () {
		Button.interactable = unlocked;
	}
}

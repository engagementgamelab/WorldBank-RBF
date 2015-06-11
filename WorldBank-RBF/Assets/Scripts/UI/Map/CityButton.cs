using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CityButton : MB {

	enum State {
		Locked,
		Unlocked,
		Visited
	}

	State state = State.Locked;

	Button button = null;
	Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}

	public bool Unlocked {
		get { return state == State.Unlocked || state == State.Visited; }
	}

	public bool Visited {
		get { return state == State.Visited; }
	}

	bool currentCity = false;
	public bool CurrentCity {
		get { return currentCity; }
	}

	public bool Interactable {
		get { return Button.interactable; }
		set { Button.interactable = value; }
	}

	Models.City model;
	public Models.City Model {
		get {
			if (model == null) {
				model = DataManager.GetCityInfo (symbol);
			}
			return model;
		}
	}

	public string symbol;
	Color visitedColor = new Color (1f, 0.5f, 0f, 1f);

	void Awake () {
		Button.interactable = Unlocked;
		if (symbol == "capitol") Visit ();
	}

	public void Unlock () {
		state = State.Unlocked;
		Button.interactable = true;
	}

	public bool Visit () {
		bool wasVisited = state == State.Visited;
		currentCity = true;
		state = State.Visited;
		ColorBlock block = Button.colors;
		block.normalColor = visitedColor;
		Button.colors = block;
		return wasVisited;
	}

	public void Leave () {
		currentCity = false;
	}
}

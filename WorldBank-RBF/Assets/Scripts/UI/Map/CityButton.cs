using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CityButton : MB {

	public enum State {
		Locked,
		Unlocked,
		Visiting,
		ExtraDayUnlocked,
		StayingExtraDay,
		PassThrough
	}

	State state = State.Locked;
	public State CityState {
		get { return state; }
	}

	Button button = null;
	Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}

	public bool Clickable {
		get { return state != State.Locked; }
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

	CityInfoBox infoBox;
	CityInfoBox InfoBox {
		get {
			if (infoBox == null) {
				infoBox = GameObject.Find ("CityInfoBox").GetScript<CityInfoBox> ();
			}
			return infoBox;
		}
	}

	public string symbol;
	Color visitedColor = new Color (1f, 0.5f, 0f, 1f);

	void Awake () {
		Button.interactable = false;
		Button.onClick.AddListener (HandleClick);
		if (symbol == "capitol") {
			state = State.PassThrough;
			Button.interactable = true;
		}
	}

	public void UpdateState (bool currentCity) {
		if (state == State.Locked) return;
		if (currentCity) {
			bool hasInteractions = InteractionsManager.Instance.HasInteractions;
			switch (state) {
				case State.Visiting:
					if (!hasInteractions) 
						state = State.ExtraDayUnlocked;
					break;
				case State.ExtraDayUnlocked:
				case State.StayingExtraDay:
					if (!hasInteractions)
						state = State.PassThrough;
					break;
			}
		}
	}

	public void HandleClick () {
		if (state == State.Locked) return;
		InfoBox.Open (this);
	}

	public void Visit () {
		if (state == State.Unlocked)
			state = State.Visiting;
	}

	public void StayExtraDay () {
		if (state == State.ExtraDayUnlocked)
			state = State.StayingExtraDay;
	}

	public void Unlock () {
		if (state == State.Locked) {
			state = State.Unlocked;
			Button.interactable = true;
		}
	}
}

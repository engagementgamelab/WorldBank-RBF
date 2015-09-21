using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoBox : MB {

	public CanvasToggle mapToggle;
	public CanvasToggle planToggle;

	public Text header;
	public Text body;
	public Text buttonText;

	string currentKey;

	GameObject panel = null;
	GameObject Panel {
		get {
			if (panel == null) {
				panel = Transform.GetChild (1).gameObject;
			}
			return panel;
		}
	}

	GameObject background = null;
	GameObject Background {
		get {
			if (background == null) {
				background = Transform.GetChild (0).gameObject;
			}
			return background;
		}
	}

	bool ShowPlan {
		get {
			int dayCount = PlayerData.DayGroup.Count;
			if (dayCount == 0)
				return true;
			if (dayCount > 3 || !PlayerData.CityGroup.CurrentCityItem.StayedExtraDay)
				return false;
			return RoutesManager.Instance.MapRoutes
				.Find (x => x.RouteItem.Cost <= dayCount) != null;
		}
	}

	void Start () {
		PlayerData.InteractionGroup.onEmpty += OnNoInteractions;
	}

	void Open (string key) {

		currentKey = key;
		body.text = DataManager.GetUIText (key);

		if (key == "copy_out_of_interactions") {
			header.text = "Out of interactions";
			buttonText.text = "Map";
		} else if (key == "copy_out_of_days") {
			header.text = "Out of days";
			buttonText.text = "Plan";
		}

		SetActive (true);
	}

	public void OnButtonPress () {
		if (currentKey == "copy_out_of_days")
			planToggle.OnClick ();
		else if (currentKey == "copy_out_of_interactions")
			mapToggle.OnClick ();
		SetActive (false);
	}

	void OnNoInteractions () {
		// Open (ShowPlan ? "copy_out_of_days" : "copy_out_of_interactions");
		StartCoroutine (CoOpen ());
	}

	void SetActive (bool active) {
		Panel.SetActive (active);
		Background.SetActive (active);
	}

	IEnumerator CoOpen () {
		
		while (!NPCFocusBehavior.Instance.Unfocused)
			yield return null;

		Open (ShowPlan ? "copy_out_of_days" : "copy_out_of_interactions");
	}
}

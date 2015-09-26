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
	bool noDays = false;

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

	Image backgroundImage = null;
	Image BackgroundImage {
		get {
			if (backgroundImage == null) {
				backgroundImage = Background.GetComponent<Image> ();
			}
			return backgroundImage;
		}
	}

	bool ShowPlan {
		get { return PlayerData.DayGroup.Empty; }
	}

	void Start () {
		PlayerData.InteractionGroup.onEmpty += OnNoInteractions;
		PlayerData.DayGroup.onEmpty += OnNoDays;
	}

	public void Open (string headerText, string contentText) {
		AudioManager.Sfx.Play ("openinfo", "ui");
		body.text = contentText;
		header.text = headerText;
		SetActive (true);
	}

	void Open (string key) {

		AudioManager.Sfx.Play ("openinfo", "ui");

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
		StartCoroutine (CoOpen ());
	}

	void OnNoDays () {
		if (!noDays && PlayerData.InteractionGroup.Empty)
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
		noDays = ShowPlan;
		StartCoroutine (CoFade (0f, 0.5f, 0.25f));
		StartCoroutine (CoExpand (Vector3.zero, Vector3.one, 0.25f));
	}

	IEnumerator CoFade (float from, float to, float time) {
		
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			BackgroundImage.color = new Color (0f, 0f, 0f, Mathf.Lerp (from, to, progress));
			yield return null;
		}
	}

	IEnumerator CoExpand (Vector3 from, Vector3 to, float time) {
		
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			Panel.transform.SetLocalScale (Vector3.Lerp (from, to, progress));
			yield return null;
		}

		Panel.transform.SetLocalScale (to);
	}
}

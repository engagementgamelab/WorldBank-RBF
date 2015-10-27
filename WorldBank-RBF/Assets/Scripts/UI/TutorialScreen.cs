using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TutorialScreen : MonoBehaviour {

	public class TutorialOverlayPosition {

		public TutorialOverlayPosition(float maxX, float maxY, float minX, float minY, float pivotX, float pivotY) {

			anchorMax = new Vector2(maxX, maxY);
			anchorMin = new Vector2(minX, minY);
			pivot = new Vector2(pivotX, pivotY);

		}

		public Vector2 Max { 
			get { return anchorMax; }
		}
		public Vector2 Min { 
			get { return anchorMin; }
		}
		public Vector2 Pivot { 
			get { return pivot; }
		}

		Vector2 anchorMax;
		Vector2 anchorMin;
		Vector2 pivot;

	}

	static public Dictionary<string, TutorialOverlayPosition> Positions = new Dictionary<string, TutorialOverlayPosition>()
	{

		{ "Center", new TutorialOverlayPosition(0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f) },
		{ "Top-Right", new TutorialOverlayPosition(1, 1, 1, 1, 1, 1) },
		{ "Top-Left", new TutorialOverlayPosition(0, 1, 0, 1, 0, 1) },
		{ "Bottom-Right", new TutorialOverlayPosition(1, 0, 1, 0, 1, 0) },
		{ "Bottom-Left", new TutorialOverlayPosition(0, 0, 0, 0, 0, 0) }

	};

	[HideInInspector]
	public string TooltipKey {
		get {
			return tooltipKey;
		}
		set { 
			tooltipKey = value;
			Load(value);
		}
	}

	[HideInInspector]
	public string Layout {
		set { 
			currentLayout = value;
			OverlayPosition(currentLayout); 
		}
	}

	[HideInInspector]
	public Rect SpotlightRect {
		set { 
			spotlightRect = value;
			SpotlightPosition();
		}
	}

	[HideInInspector]
	public Rect MaskRect {
		set { 
			maskRect = value;
			MaskPosition();
		}
	}

	bool Drag {
		get { return MainCamera.Instance.Positioner.Drag.Enabled; }
		set { MainCamera.Instance.Positioner.Drag.Enabled = value; }
	}

	[HideInInspector]
	public bool spotlightEnabled;
	
	[HideInInspector]
	public int layoutIndex;

	[HideInInspector]
	public string overlayLocation;

	public string overlayText;

	public RectTransform overlayPanel;
	public RectTransform maskButtonRect;
	public RawImage spotlightImage;

	GenericButton confirmButton;
	GenericButton yesButton;
	GenericButton noButton;

	CanvasGroup group;

	public Rect maskRect = new Rect(1, 0, 1, 1);
	public Rect spotlightRect = new Rect(1, 0, 1, 1);

	string tooltipKey;
	string currentLayout;

	void OnEnable () {
		Events.instance.AddListener<CloseTutorialEvent> (OnCloseTutorialEvent);
	}

	void OnDisable() {
		Events.instance.RemoveListener<CloseTutorialEvent>(OnCloseTutorialEvent);
	}

	void Start() {


		Image maskImg = maskButtonRect.gameObject.GetComponent<Image>();

		Color imgColor = maskImg.color;
    
    // #if DEVELOPMENT_BUILD && UNITY_IOS
	   //  imgColor.a = 1;
    // #else
	    imgColor.a = 0;
    // #endif

    maskImg.color = imgColor;

	}

	public void Load(string strKey, string strNextKey=null, UnityAction confirmAction=null) {

		MainCamera.Instance.Positioner.Drag.OnDragUp ();
		Drag = KeyCanDrag (strKey);
		AudioManager.Sfx.Play ("openinfo", "ui");

		tooltipKey = strKey;

		group = gameObject.GetComponent<CanvasGroup>();

		confirmButton = transform.Find("Overlay/Buttons/Confirm button").GetComponent<GenericButton>();
		yesButton = transform.Find("Overlay/Buttons/Yes button").GetComponent<GenericButton>();
		noButton = transform.Find("Overlay/Buttons/No button").GetComponent<GenericButton>();

		Models.Tooltip tooltip = DataManager.GetTooltipByKey(strKey);
		OverlayPosition(tooltip.overlay_location);

		overlayPanel.GetComponentInChildren<Text>().text = tooltip.text;
		overlayText = tooltip.text;

		if(tooltip.confirm_next != null)
			strNextKey = tooltip.confirm_next;

		if(tooltip.spotlight_position == null) {
			spotlightEnabled = false;
			group.blocksRaycasts = true;

			DisableSpotlight();
			SpotlightPosition();
			MaskPosition();
		}
		else {
			spotlightEnabled = true;
			group.blocksRaycasts = false;

			#if UNITY_ANDROID
				tooltip.spotlight_position[0] += .4f;
			#endif

			SpotlightRect = new Rect(tooltip.spotlight_position[0], tooltip.spotlight_position[1], tooltip.spotlight_size[0], tooltip.spotlight_size[1]);
			MaskRect = new Rect(tooltip.mask_position[0], tooltip.mask_position[1], tooltip.mask_size[0], tooltip.mask_size[1]);
		}

		confirmButton.gameObject.SetActive(tooltip.confirm);

		confirmButton.Button.onClick.RemoveAllListeners ();
		confirmButton.AddAudioTriggerListener ();
		confirmButton.Button.onClick.AddListener(() => Events.instance.Raise (new CloseTutorialEvent ()));//ObjectPool.Destroy<TutorialScreen>(transform));

		// Custom action
		if(confirmAction != null)
			confirmButton.Button.onClick.AddListener(confirmAction);

		if(!String.IsNullOrEmpty(strNextKey)) {
			confirmButton.Button.onClick.AddListener(() => DialogManager.instance.CreateTutorialScreen(strNextKey));
			confirmButton.Text = "Next";
		}
		else {
			confirmButton.Text = "Ok";

			if(tooltip.confirm_action != null)
				confirmButton.Button.onClick.AddListener(() => Events.instance.Raise(new TutorialEvent(tooltip.confirm_action)));
		}

		if(tooltip.yes_no) {
			yesButton.gameObject.SetActive(true);
			noButton.gameObject.SetActive(true);

			yesButton.Text = tooltip.yes_label;
			noButton.Text = tooltip.no_label;

			yesButton.Button.onClick.RemoveAllListeners ();
			yesButton.AddAudioTriggerListener ();
			yesButton.Button.onClick.AddListener(() => Events.instance.Raise(new TutorialEvent(tooltip.yes_action)));

			noButton.Button.onClick.RemoveAllListeners ();
			noButton.Button.onClick.AddListener(() => Events.instance.Raise(new TutorialEvent(tooltip.no_action)));
			noButton.AddAudioTriggerListener ();
		}
		else {

			yesButton.gameObject.SetActive(false);
			noButton.gameObject.SetActive(false);

		}
		
	}

	void OverlayPosition(string strPositionLabel) {

		TutorialOverlayPosition position = Positions[strPositionLabel];

		overlayPanel.anchorMax = position.Max;
		overlayPanel.anchorMin = position.Min;
		overlayPanel.pivot = position.Pivot;

		overlayLocation = strPositionLabel;

		int i = 0;
		foreach (string label in Positions.Keys)
		{
			if(label == strPositionLabel) {
		    	layoutIndex = i;
		    	break;
		    }
		    i++;
		}

	}

	public void DisableSpotlight() {

		SpotlightRect = new Rect(1, 0, 1, 1);
		MaskRect = new Rect(0, 0, 0, 0);
	}

	public void SpotlightPosition() {

		float widthFactor = gameObject.GetComponent<RectTransform>().rect.width / (800 - spotlightRect.width);
		
		Rect factoredRect = spotlightRect;
		factoredRect.x = spotlightRect.x * widthFactor;

		spotlightImage.uvRect = factoredRect;

	}

	public void MaskPosition() {

		float widthFactor = gameObject.GetComponent<RectTransform>().rect.width / (800 - maskRect.width);

		maskButtonRect.sizeDelta = new Vector2(maskRect.width, maskRect.height);	
		maskButtonRect.anchoredPosition = new Vector2(maskRect.x * widthFactor, maskRect.y);
		
	}
	
	void OnCloseTutorialEvent (CloseTutorialEvent e) {

		if(transform == null)
			return;

		ObjectPool.Destroy<TutorialScreen>(transform);
		Drag = true;
	}

	bool KeyCanDrag (string key) {
		if (key == "phase_1_move_around")
			return true;
		return false;
	}
}

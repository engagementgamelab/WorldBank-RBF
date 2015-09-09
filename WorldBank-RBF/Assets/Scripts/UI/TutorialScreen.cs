using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
	public bool spotlightEnabled;
	
	[HideInInspector]
	public int layoutIndex;

	[HideInInspector]
	public Rect spotlightRect = new Rect(1, 0, 1, 1);

	[HideInInspector]
	public string overlayLocation;

	public string overlayText;

	public RectTransform overlayPanel;
	public RawImage spotlightImage;

	Button confirmButton;
	CanvasGroup group;

	string tooltipKey;
	string currentLayout;

	// Use this for initialization
	void Start () {
	
	}

	void Update() {
	}
	
	void OnGUI() {

		Debug.Log( GUIUtility.ScreenToGUIPoint(Event.current.mousePosition).normalized );
    
    }

	public void Load(string strKey) {

		group = gameObject.GetComponent<CanvasGroup>();
		confirmButton = gameObject.GetComponentInChildren<Button>();

		Models.Tooltip tooltip = DataManager.GetTooltipByKey(strKey);
		OverlayPosition(tooltip.overlay_location);

		overlayPanel.GetComponentInChildren<Text>().text = tooltip.text;
		overlayText = tooltip.text;

		if(tooltip.spotlight_position == null) {
			spotlightEnabled = false;
			group.blocksRaycasts = true;
		}
		else {
			spotlightEnabled = true;
			group.blocksRaycasts = false;

			SpotlightRect = new Rect(tooltip.spotlight_position[0], tooltip.spotlight_position[1], tooltip.spotlight_size[0], tooltip.spotlight_size[1]);
		}

		Debug.Log(GUIUtility.ScreenToGUIPoint(new Vector3(tooltip.spotlight_position[0], tooltip.spotlight_position[1], 0)).normalized);

		confirmButton.gameObject.SetActive(tooltip.confirm);

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

	void SpotlightPosition() {

		spotlightImage.uvRect = spotlightRect;

	}
}

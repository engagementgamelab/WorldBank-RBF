using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tactic : MB {

	public static Tactic selected = null;
	public readonly float Height = 180f;

	public PortraitTextBox portrait;
	public Text titleText;
	public Text descriptionText;
	public Text contextText;
	TacticsContainer container;

	Vector2 dragPosition;
	bool dragging = false;
	bool verticalDrag = false;
	bool wasDropped = false;
	bool wasClicked = false;
	float delay = 0.005f;
	TacticPlaceholder placeholder;
	
	TacticItem item;
	public TacticItem Item {
		get { return item; }
	}

	public int index;
	public int Index {
		get { return index; }
		set { index = value; }
	}

	Transform planPanel;
	Transform PlanPanel {
		get {
			if (planPanel == null) {
				planPanel = GameObject.Find ("PlanPanel").transform;
			}
			return planPanel;
		}
	}

	CanvasGroup canvasGroup = null;
	CanvasGroup CanvasGroup {
		get {
			if (canvasGroup == null) {
				canvasGroup = GetComponent<CanvasGroup> ();
			}
			return canvasGroup;
		}
	}

	void Awake () {
		CanvasGroup.blocksRaycasts = false;
		Events.instance.AddListener<ScrollDirectionEvent> (OnScrollDirectionEvent);
		Events.instance.AddListener<DropTacticEvent> (OnDropTacticEvent);
	}

	public void Init (TacticItem item, TacticsContainer container) {
		this.item = item;
		this.container = container;
		titleText.text = item.Title;
		descriptionText.text = item.Description;
		contextText.text = item.Context;
		verticalDrag = false;
		dragging = false;
	}

	bool MouseOver () {
		RectTransform objectRectTransform = gameObject.GetComponent<RectTransform> (); 
		float width = objectRectTransform.rect.width;
		float height = objectRectTransform.rect.height;
		float xpos = transform.position.x;
		float ypos = transform.position.y;
		return (Input.mousePosition.x > xpos - width * 0.5f
         	&& Input.mousePosition.x < xpos + width * 0.5f
         	&& Input.mousePosition.y < ypos
			&& Input.mousePosition.y > ypos - height);
	}

	void Update () {

		if (!Input.GetMouseButton (0)) {
			wasClicked = false;
		}

		if (dragging) {
			transform.position = dragPosition + (Vector2)Input.mousePosition;
			if (!Input.GetMouseButton (0)) {

				// Stop dragging
				dragging = false;
				delay = 0.1f;
				selected = null;
				if (!wasDropped) {
					StartCoroutine (CoReturnToContainer ());
				} else {
					placeholder.ShrinkAndDestroy ();
					ObjectPool.Destroy<Tactic> (Transform);
				}
				Events.instance.Raise (new EndDragTacticEvent ());
			}
		}

		if (selected != null || !MouseOver () || verticalDrag) return;

		if (Input.GetMouseButtonDown (0)) {
			wasClicked = true;
		}

		if (Input.GetMouseButton (0)) {
			if (delay > 0) {
				delay -= Time.deltaTime;
				return;
			}

			// Start dragging
			if (!dragging && wasClicked) {
				StartDragging ();			
				CreatePlaceholder ();
				Events.instance.Raise (new BeginDragTacticEvent ());
			}
		}
	}

	public void StartDragging () {
		selected = this;
		transform.SetParent (PlanPanel);
		dragging = true;
		dragPosition = transform.position - Input.mousePosition;
	}

	void CreatePlaceholder () {
		placeholder = ObjectPool.Instantiate<TacticPlaceholder> ();
		placeholder.Transform.SetParent (container.Transform);
		placeholder.Transform.Reset ();
		placeholder.Transform.SetSiblingIndex (Index);
	}

	void ReturnToContainer () {
		StartCoroutine (CoReturnToContainer ());
	}

	IEnumerator CoReturnToContainer () {
		
		float time = 0.2f;
		float eTime = 0f;
		Vector3 startPosition = Position;
		Vector3 endPosition = placeholder.Position;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			Position = Vector3.Lerp (startPosition, endPosition, progress);
			yield return null;
		}

		ObjectPool.Destroy<TacticPlaceholder> (placeholder.Transform);
		Transform.SetParent (container.Transform);
		Transform.SetSiblingIndex (Index);
	}

	void OnScrollDirectionEvent (ScrollDirectionEvent e) {
		verticalDrag = e.Vertical;
	}

	void OnDropTacticEvent (DropTacticEvent e) {
		if (e.Tactic == this)
			wasDropped = true;
	}
}

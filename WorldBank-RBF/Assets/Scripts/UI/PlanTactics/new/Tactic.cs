using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tactic : MB {

	public static Tactic selected = null;

	public PortraitTextBox portrait;
	public Text titleText;
	public Text descriptionText;
	public Text contextText;

	const float delayAmount = 0.1f;
	float delay = 0.1f;

	Vector2 dragPosition;
	bool dragging = false;
	bool verticalDrag = false;
	bool wasClicked = false;
	bool animating = false;
	bool locationChanged = false;
	
	TacticPlaceholder placeholder;

	[SerializeField] TacticDragData dragData;
	public TacticDragData DragData {
		get {
			if (dragData == null) {
				dragData = new TacticDragData (this, Container);
			}
			return dragData;
		}
	}
	
	TacticItem item;
	public TacticItem Item {
		get { return item; }
	}

	public int index; // just for debugging - this should be an auto property
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

	TacticsContainer container = null;
	TacticsContainer Container {
		get {
			if (container == null) {
				container = GameObject.Find ("TacticsContainer").GetScript<TacticsContainer> ();
			}
			return container;
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

	RectTransform rectTransform = null;
	RectTransform RectTransform {
		get {
			if (rectTransform == null) {
				rectTransform = GetComponent<RectTransform> ();
			}
			return rectTransform;
		}
	}

	void Awake () {
		CanvasGroup.blocksRaycasts = false;
		Events.instance.AddListener<ScrollDirectionEvent> (OnScrollDirectionEvent);
	}

	void OnEnable () {
		RectTransform.sizeDelta = new Vector2 (380f, 180f);
		if (LocalScale.x < 1 || LocalScale.y < 1)
			StartCoroutine (CoScale (LocalScale, Vector3.one));
	}

	public void Init (TacticItem item) {
		
		this.item = item;
		
		titleText.text = item.Title;
		descriptionText.text = item.Description;
		contextText.text = item.Context;
		verticalDrag = false;
		dragging = false;
	}

	void Update () {

		if (!Input.GetMouseButton (0)) {
			wasClicked = false;
		}

		if (dragging) {
			transform.position = dragPosition + (Vector2)Input.mousePosition;
			if (!Input.GetMouseButton (0)) {

				// Stop dragging
				StopDragging ();
				HandleDrop ();
				Events.instance.Raise (new EndDragTacticEvent ());
			}
		}

		if (selected != null || animating || !MouseOver () || verticalDrag) return;

		if (Input.GetMouseButtonDown (0)) {
			wasClicked = true;
		}

		if (Input.GetMouseButton (0)) {
			
			// Wait a small amount of time to check if user is performing vertical or horizontal drag
			if (delay > 0) {
				delay -= Time.deltaTime;
				return;
			}

			// Start dragging
			if (!dragging && wasClicked) {
				StartDragging (Container);			
				CreatePlaceholder ();
				Events.instance.Raise (new BeginDragTacticEvent ());
			}
		}
	}

	public void SetDropLocation (DragLocation toLocation) {
		locationChanged = true;
		DragData.ToLocation = toLocation;
	}

	public void ForceFromSlot (Tactic replacementTactic) {
		PlayerData.TacticPriorityGroup.Remove (Item);
		TacticDragData replacementData = replacementTactic.DragData;
		if (replacementData.FromContainer != null) {
			MoveToContainerBottom ();
		} else {
			MoveToSlot (replacementData.FromSlot);
		}
	}

	void HandleDrop () {

		if (!locationChanged) {
			
			if (DragData.FromLocation is TacticsContainer) {
				ReturnToContainer ();
			} else {
				ReturnToSlot ();
			}

		} else if (DragData.FromLocation is TacticSlot) {

			if (DragData.ToLocation is TacticsContainer) {
				
				// slot to container
				MoveToContainerBottom ();
			} else {

				// slot to slot
				ShrinkAndDestroy ();
			} 
		
		} else if (DragData.FromLocation is TacticsContainer) {

			if (DragData.ToLocation is TacticSlot) {

				// container to slot
				placeholder.ShrinkAndDestroy ();
				ShrinkAndDestroy ();
			} else {

				// container to container
				ReturnToContainer ();
			}
		}

		locationChanged = false;
		DragData.FromLocation = DragData.ToLocation;
		Container.UpdateIndices ();
	}

	public void StartDragging (DragLocation fromLocation) {
		DragData.FromLocation = fromLocation;
		selected = this;
		transform.SetParent (PlanPanel);
		dragging = true;
		dragPosition = transform.position - Input.mousePosition;
	}

	void StopDragging () {
		dragging = false;
		delay = delayAmount;
		selected = null;
	}

	void CreatePlaceholder (bool createAtIndex=true) {
		placeholder = ObjectPool.Instantiate<TacticPlaceholder> ();
		placeholder.Transform.SetParent (Container.Transform);
		placeholder.Transform.Reset ();
		if (createAtIndex)
			placeholder.Transform.SetSiblingIndex (Index);
	}

	void DestroyPlaceholder () {
		ObjectPool.Destroy<TacticPlaceholder> (placeholder.Transform);
	}

	void ShrinkAndDestroy () {
		StartCoroutine (CoScale (
			Vector3.one, 
			Vector3.zero, 
			0.1f, 
			() => { ObjectPool.Destroy<Tactic> (Transform); })
		);
	}

	IEnumerator CoScale (Vector3 startScale, Vector3 endScale, float time=0.1f, System.Action onEnd=null) {
		
		animating = true;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			LocalScale = Vector3.Lerp (startScale, endScale, progress);
			yield return null;
		}

		animating = false;
		if (onEnd != null)
			onEnd ();
	}

	void ReturnToContainer () {
		StartCoroutine (CoMove (
			Position,
			placeholder.Position,
			0.2f,
			() => {
				DestroyPlaceholder ();
				Container.AddTactic (this, item, Index);
			})
		);
	}

	void ReturnToSlot () {
		StartCoroutine (CoMove (
			Position,
			DragData.FromSlot.Position,
			0.2f,
			() => {
				DragData.FromSlot.FillSlot (this);
				ShrinkAndDestroy ();
			})
		);
	}

	void MoveToContainerBottom () {
		Parent = PlanPanel;
		CreatePlaceholder (false);
		StartCoroutine (CoMove (
			Position,
			placeholder.GetComponent<RectTransform> ().anchoredPosition,
			0.2f,
			() => {
				DestroyPlaceholder ();
				Container.AddTactic (this, item);
			})
		);
	}

	void MoveToSlot (TacticSlot slot) {
		Parent = PlanPanel;
		DragData.ToLocation = slot;
		StartCoroutine (CoMove (
			Position,
			DragData.ToLocation.Position,
			0.2f,
			() => {
				DragData.ToSlot.FillSlot (this);
				ShrinkAndDestroy ();
			})
		);
	}

	IEnumerator CoMove (Vector3 startPosition, Vector3 endPosition, float time=0.2f, System.Action onEnd=null) {
		
		animating = true;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			Position = Vector3.Lerp (startPosition, endPosition, progress);
			yield return null;
		}

		animating = false;
		if (onEnd != null)
			onEnd ();
	}

	bool MouseOver () {
		float width = RectTransform.rect.width;
		float height = RectTransform.rect.height;
		float xpos = transform.position.x;
		float ypos = transform.position.y;
		Vector3 mousePosition = Input.mousePosition;
		return (mousePosition.x > xpos - width * 0.5f
         	&& mousePosition.x < xpos + width * 0.5f
         	&& mousePosition.y < ypos + height * 0.5f
			&& mousePosition.y > ypos - height * 0.5f);
	}

	// Events

	void OnScrollDirectionEvent (ScrollDirectionEvent e) {
		verticalDrag = e.Vertical;
	}
}

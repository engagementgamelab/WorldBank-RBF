/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 CoverFlow.cs
 Cover flow-esque UI utility (adapted from https://github.com/rakkarage/Flop).

 Created by Johnny Richardson on 7/21/15.
==============
*/
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
public class CoverFlow : UIBehaviour, IDragHandler
{
	public float Offset = 164f;
	public Transform LookAt;
	
	void Start()
	{
		base.Start();
	}

	public void AddTransform(Transform newTransform)  
	{
		newTransform.SetParent(transform);
		for (int i = 0; i < transform.childCount; i++)
		{
			var x = i * Offset;
			Drag(x, transform.GetChild(i));
		}
		Order();
	}

	public void Drag(PointerEventData e)
	{
		foreach (Transform i in transform)
		{
			var x = i.localPosition.x + e.delta.x;
			Drag(x, i);
		}
		Order();
	}
	
	void Order()
	{
		var children = GetComponentsInChildren<Transform>();
		var sorted = from child in children orderby child.localPosition.z descending select child;
		for (int i = 0; i < sorted.Count(); i++)
		{
			sorted.ElementAt(i).SetSiblingIndex(i);
		}
	}
	
	void Drag(float x, Transform t)
	{
		t.localPosition = new Vector3(x, transform.localPosition.y, x < 0 ? -x : x);
	}

	public void OnDrag(PointerEventData e)
	{
		Drag(e);
	}
}

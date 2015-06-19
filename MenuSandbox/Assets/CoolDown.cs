using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class CoolDown : MonoBehaviour {

	void Start()
	{
		foreach (var Skill in skills)
		{
			Skill.currentCoolDown = Skill.cooldown;
		}
	}

	public List<Skill> skills;



	void FixedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{ 
			if (skills[0].currentCoolDown >= skills [0].cooldown)
			{
				skills[0].currentCoolDown = 0;
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			if (skills[1].currentCoolDown >= skills [1].cooldown)
			{
				skills[1].currentCoolDown = 0;
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			if (skills[2].currentCoolDown >= skills [2].cooldown)
			{
				skills[2].currentCoolDown = 0;
			}
		}
	}			       



	// Update is called once per frame
	void Update () 
	{
			foreach (Skill s in skills)
			{
				if (s.currentCoolDown < s.cooldown)
					{
							s.currentCoolDown += Time.deltaTime;
							s.skillIcon.fillAmount = s.currentCoolDown/s.cooldown;
					}
				}
			}


[System.Serializable]

public class Skill
{
	public float cooldown;
	public Image skillIcon;
	[HideInInspector]
	public float currentCoolDown;
}
}
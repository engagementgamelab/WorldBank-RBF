/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 ScenarioYearEndDialog.cs
 Scenario year end dialog.

 Created by Johnny Richardson on 7/16/15.
==============
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScenarioYearEndDialog : GenericDialogBox {
    
    /// <summary>
    /// Set the previous choices the player has made (currently just splits an array and shows as a list).
    /// </summary>
    public List<string> PreviousChoices {
        set {
            textPreviousChoices.text = "Choices: " + string.Join(", ", value.ToArray());
        }
    }

	public Text textPreviousChoices;
    
}
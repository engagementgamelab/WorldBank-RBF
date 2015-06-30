using UnityEngine;
using System.Collections;

public class NotebookCanvas : MB {

	public virtual void Open () {}
	public virtual void Close () {}

	public virtual void UpdateIndicators(int intBirths, int intVaccinations, int intQOC) {}

}

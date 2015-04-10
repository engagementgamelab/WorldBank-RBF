using UnityEngine;
using System.Collections;

public interface IEditorPoolable {
	int Index { get; set; }
	void Init ();
}

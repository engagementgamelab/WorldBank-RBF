using UnityEngine;
using System.Collections;

public interface IEditorObjectDrawer<T> where T : class {
	T Target { get; set; }
}

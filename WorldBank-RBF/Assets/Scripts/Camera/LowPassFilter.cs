using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Takes a number of values and applies a low pass filter.
/// </summary>
public class LowPassFilter {

	Queue<float> values = new Queue<float> ();
	
	/// <summary>
	/// Gets/sets the number of values to apply the filter to.
	/// </summary>
	public int Buffer { get; set; }

	/// <summary>
	/// Gets the filtered value.
	/// </summary>
	public float Average {
		get { 
			float total = 0f;
			foreach (float value in values) {
				total += value;
			}
			return total / (float)Buffer;
		}
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="buffer">The initial buffer size</param>
	public LowPassFilter (int buffer=10) {
		this.Buffer = buffer;
	}

	/// <summary>
	/// Adds a value to the buffer.
	/// </summary>
	/// <param name="value">Value to add.</param>
	public void Add (float value) {
		values.Enqueue (value);
		if (values.Count > Buffer)
			values.Dequeue ();
	}

	/// <summary>
	/// Clears the buffer.
	/// </summary>
	public void Reset () {
		values.Clear ();
	}
}

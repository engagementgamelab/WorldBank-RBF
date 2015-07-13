using UnityEngine;
using System.Collections;

/// <summary>
/// A city based on a data model.
/// </summary>
public class CityItem : InventoryItem {
	
	/// <summary>
	/// Gets the name of the city.
	/// </summary>
	public override string Name { get { return Model.display_name; } }

	/// <summary>
	/// The symbol of the model.
	/// </summary>
	public string Symbol { get { return Model.symbol; } }

	/// <summary>
	/// The model that this city is based off of.
	/// </summary>
	public Models.City Model { get; private set; }

	/// <summary>
	/// Gets/sets whether or not the city has been visited (in general this will be false initially
	/// and only be set to true once during gameplay)
	/// </summary>
	public bool Visited { get; set; }

	/// <summary>
	/// Gets/sets whether or not the player has stayed an extra day in the city (in general this will
	/// be false initially and only be set to true once during gameplay)
	/// </summary>
	public bool StayedExtraDay { get; set; }

	public CityItem () {}

	/// <summary>
	/// Creates a city based on the given model.
	/// </summary>
	/// <param name="model">The model to base this city on.</param>
	public CityItem (Models.City model) {
		this.Model = model;

		// Special case: capitol can't be entered (yet)
		if (Model.symbol == "capitol") {
			Visited = true;
			StayedExtraDay = true;
		}
	}
}

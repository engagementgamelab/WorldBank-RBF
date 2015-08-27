﻿using UnityEngine;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class AudioGroup<T> : ItemGroup<T> where T : AudioItem, new () {

	protected struct PlaySettings {
		
		public bool allowSimultaneous;
		
		public PlaySettings (bool allowSimultaneous) {
			this.allowSimultaneous = allowSimultaneous;
		}
	}

	protected virtual PlaySettings Settings {
		get { return new PlaySettings (false); }
	}

	protected string GroupPath {
		get { return "Audio/" + subpath; }
	}

	protected string subpath;
	public string Subpath {
		get { return subpath; }
		set {
			subpath = value;
			LoadFromPath (GroupPath);
		}
	}

	readonly Inventory subgroups = new Inventory ();
	readonly List<AudioItem> playing = new List<AudioItem> ();

	public AudioGroup (string subpath) {
		this.subpath = subpath;
		LoadFromPath (GroupPath);
		LoadFromSubdirectories ();
	}

	protected void LoadFromPath (string path) {
		AudioClip[] clips = Array.ConvertAll (Resources.LoadAll (path), x => (AudioClip)x);
		foreach (AudioClip clip in clips) {
			Add (new T { Clip = clip });
		}
	}

	protected void LoadFromSubdirectories () {
		string[] dirs = Directory.GetDirectories (Application.dataPath + "/Resources/" + GroupPath);	
		if (dirs.Length == 0) return;
		foreach (string d in dirs) {
			string id = Regex.Match (d, @"[^\/]*.$").ToString ();
			AudioSubgroup<T> a = new AudioSubgroup<T> (id.ToLower (), Subpath + "/" + id);
			subgroups.Add (a);
		}
	}

	/**
	 *
	 * Returns an AudioItem in the group "groupId" and with the name "name."
	 * If no item with this name exists, AudioGroup assumes that it should play a random sound,
	 * with "name" specifying its qualities.
	 *
	 * So passing in "fem1greeting1" will play that specific AudioItem,
	 * "fem1greeting" will play an AudioItem with the qualities "fem1" and a random "greeting",
	 * 		(e.g. fem1greeting2, fem1greeting1, fem1greeting4)
	 * "fem1" will play an AudioItem with the quality "fem1" and a random quality,
	 *		(e.g. fem1greeting3, fem1farewell2, fem1response1)
	 * "fem" will play an AudioItem the quality "fem"
	 *		(e.g. fem2greeting1, fem1farewell3, fem3response2)
	 */
	public AudioItem GetItem (string name, string groupId) {
		AudioItem item = (groupId == "")
			? GetItem (name)
			: FindGroup (groupId).GetItem (name);
		
		if (item == null)
			throw new Exception ("Couldn't find an AudioItem with the name '" + name + "' in the group '" + groupId + "'");

		return item;
	}

	public AudioItem GetItem (string name) {
		AudioItem item = MyItems.Find (x => x.Name == name);
		if (item == null) {

			List<string> qualities = new List<string> ();
			Regex regex = new Regex (@"([a-zA-Z]+)");
			MatchCollection matches = regex.Matches (name);
			foreach (Match m in matches) {
				if (m.Success) qualities.Add (m.Value);
			}
			return FindItemWithQualities (qualities);
		}
		return item;
	}

	// TODO: this is slow
	AudioItem FindItemWithQualities (List<string> qualities) {
		List<AudioItem> itemsWithQualities = new List<AudioItem> ();
		foreach (AudioItem item in MyItems) {
			if (qualities.All (x => item.Qualities.Contains (x)))
				itemsWithQualities.Add (item);
		}
		return itemsWithQualities[UnityEngine.Random.Range (0, itemsWithQualities.Count-1)];
	}

	AudioSubgroup<T> FindGroup (string groupId) {
		groupId = groupId.ToLower ();
		ItemGroup g;
		if (subgroups.Groups.TryGetValue (groupId, out g)) {
			return (AudioSubgroup<T>)g;
		} else {

			// Recursive lookup
			foreach (var group in subgroups.Groups) {
				AudioSubgroup<T> subgroup = ((AudioSubgroup<T>)group.Value)
					.FindGroup (groupId);
				if (subgroup != null) {
					return subgroup;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Plays the given AudioItem.
	/// </summary>
	/// <param name="item">The AudioItem to play.</param>
	public void Play (AudioItem item) {
		if (!Settings.allowSimultaneous)
			StopAll ();
		item.Play ();
		playing.Add (item);
	}

	/// <summary>
	/// Finds an AudioItem with the appropriate name and group and plays it.
	/// </summary>
	/// <param name="name">Name of the AudioItem.</param>
	/// <param name="groupId">Name of the group that the AudioItem is in.</param>
	public void Play (string name, string groupId="") {
		Play (GetItem (name, groupId));
	}

	/// <summary>
	/// Stops the given AudioItem.
	/// </summary>
	/// <param name="item">The AudioItem to stop.</param>
	public void Stop (AudioItem item) {
		item.Stop ();
		playing.Remove (item);
	}

	/// <summary>
	/// Finds an AudioItem with the appropriate name and group and stops it.
	/// </summary>
	/// <param name="name">Name of the AudioItem.</param>
	/// <param name="groupId">Name of the group that the AudioItem is in.</param>
	public void Stop (string name, string groupId="") {
		Stop (name, groupId);
	}

	void StopAll () {
		foreach (AudioItem item in playing)
			item.Stop ();
		playing.Clear ();
	}
}

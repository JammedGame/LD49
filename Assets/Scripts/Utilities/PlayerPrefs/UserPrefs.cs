using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities.Preferences
{
	public class BoolPlayerPrefEntry : PlayerPrefsEntry
	{
		public bool GetValue() => PlayerPrefs.GetInt(Key, 0) != 0;
		public void SetValue(bool value) => PlayerPrefs.SetInt(Key, value ? 1 : 0);

		public BoolPlayerPrefEntry(string key) : base(key)
		{
		}
	}

	public abstract class PlayerPrefsEntry
	{
		static readonly Dictionary<string, PlayerPrefsEntry> prefs = new Dictionary<string, PlayerPrefsEntry>();

		public readonly string Key;

		public PlayerPrefsEntry(string key)
		{
			this.Key = key;
		}
	}
}
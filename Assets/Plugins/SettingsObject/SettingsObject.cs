using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Base class for all settings scriptable objects in game - adds 'Version' counter to base ScriptableObject,
/// to allow code to poll for changes (found this method far more robust and simpler than using events).
/// </summary>
public class SettingsObject : ScriptableObject
{
	private int version;

	public int Version => version;

	protected void OnValidate()
	{
		version++;
	}

	public new void SetDirty()
	{
		version++;
#if UNITY_EDITOR
		EditorUtility.SetDirty(this);
#endif
	}
}
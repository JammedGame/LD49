using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AnimationImporter
{
    [MenuItem("GameObject/Retarget anim")]
	public static void RetargetAnim()
    {
		var activeTransform = Selection.activeTransform;
        if (!activeTransform)
			return;

		foreach(var transform in Selection.activeTransform.GetComponentsInChildren<Transform>(true))
        {
            var go = transform.gameObject;
			var removeIndex = go.name.IndexOf(':');
            if (removeIndex >= 0)
            {
				go.name = go.name.Remove(0, removeIndex + 1);
			}
		}
    }

    /// <summary>
	/// Given fbx path, export anim file (gets rid of wrapper)
	/// </summary>
    [MenuItem("Assets/Export FBX Clip")]
    public static void ExportAnimation()
    {
		AssetDatabase.StartAssetEditing();

		foreach(var assetPath in Selection.assetGUIDs.Select(AssetDatabase.GUIDToAssetPath))
		{
            if (!assetPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
				continue;

			var folderPath = Path.GetDirectoryName(assetPath);
			var folderName = Path.GetFileName(folderPath);

			var fbxClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
			if (fbxClip == null)
				continue;

			var originalFileName = Path.GetFileName(assetPath).Replace(' ', '-');
			var fileName = $"anim_{originalFileName}";
			var clipCopy = AnimationClip.Instantiate(fbxClip);
			var fbxClipPath = Path.ChangeExtension(Path.Combine(folderPath, fileName), ".anim");
			clipCopy.name = fileName;
			AssetDatabase.CreateAsset(clipCopy, fbxClipPath);
		}

		AssetDatabase.StopAssetEditing();
	}
}

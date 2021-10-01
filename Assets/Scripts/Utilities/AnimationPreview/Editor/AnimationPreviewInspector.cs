using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[CustomEditor(typeof(AnimationPreview))]
public class AnimationPreviewInspector : Editor
{
	[SerializeField] SearchField searchField;
	[SerializeField] string searchFieldText;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var animationPreviewBehaviour = target as AnimationPreview;
		if (animationPreviewBehaviour == null)
			return;

		var animationBehaviour = animationPreviewBehaviour.GetComponent<Animation>();
		if (animationBehaviour == null)
			return;

		searchField ??= new SearchField();
		searchFieldText = searchField.OnGUI(searchFieldText);

		foreach(var obj in animationBehaviour)
		{
			var animatonState = (AnimationState)obj;
			var clipName = animatonState.name;
			if (!string.IsNullOrWhiteSpace(searchFieldText) && !clipName.Contains(searchFieldText))
				continue;

			if (GUILayout.Button(clipName))
			{
				animationBehaviour.Play(clipName);
			}
		}
	}
}
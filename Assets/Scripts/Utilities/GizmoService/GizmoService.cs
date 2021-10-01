#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEngine;
using Game;
using UnityEditor;
using Game.Utilities.Preferences;

public class GizmoMethodInfo
{
	public readonly MethodInfo methodInfo;
	public readonly ParameterInfo[] parametersInfo;
	public readonly object[] args;
	public readonly BoolPlayerPrefEntry IsEnabledPref;

	public GizmoMethodInfo(MethodInfo method)
	{
		this.methodInfo = method;
		this.parametersInfo = method.GetParameters();
		this.args = new object[parametersInfo.Length];
		this.IsEnabledPref = new BoolPlayerPrefEntry(method.Name);
	}
}

public static class GizmoService
{
	static GizmoMethodInfo[] gizmoMethodList;

	public static void Draw(GameWrapper wrapper)
	{
		foreach (var gizmoInfo in GetAllGizmoMethods())
		{
			if (!gizmoInfo.IsEnabledPref.GetValue())
				continue;

			gizmoInfo.args[0] = wrapper;
			gizmoInfo.methodInfo.Invoke(null, gizmoInfo.args);

			// reset gizmo parameters.
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = Color.white;
		}
	}

	private static GizmoMethodInfo[] BuildGizmoMethodList() => TypeCache.GetMethodsWithAttribute<GizmoAttribute>()
		.Where(method =>
			method.IsStatic
			&& method.GetParameters() is ParameterInfo[] parameterList
			&& parameterList[0].ParameterType == typeof(GameWrapper))
		.Select(x => new GizmoMethodInfo(x))
		.ToArray();

	public static GizmoMethodInfo[] GetAllGizmoMethods() =>
		gizmoMethodList ??= BuildGizmoMethodList();
}

#endif
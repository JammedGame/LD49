
using BattleSimulator.Spells;
using Game.View;
using Game.View.SpellView;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpellView), true)]
public class SpellViewInspector : Editor<SpellView>
{
    Editor settingsEditor;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpellSettings settings = target.Data is Spell spell
            ? spell.Settings
            : Resources.Load<SpellSettings>("Settings/SpellSettings/" + target.name.Replace("View", ""));

        if (settings != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.ObjectField("Spell Settings", settings, typeof(SpellSettings), allowSceneObjects: false);
            EditorGUILayout.Space();
            Editor.DrawFoldoutInspector(settings, ref settingsEditor);
        }
    }
}
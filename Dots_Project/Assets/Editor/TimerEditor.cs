using UnityEngine;
using UnityEditor;

namespace Game.Editor
{
	[CustomEditor(typeof(Timer))]
	public class TimerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI() {
			SerializedProperty maxTimeProp = serializedObject.FindProperty("maxTime");
			EditorGUILayout.Slider(maxTimeProp, 10f, 30f, new GUIContent("Общее время", "Максимальное значение шкалы времени на протяжении игры"));
			SerializedProperty partOfTheMaxTimeProp = serializedObject.FindProperty("partOfTheMaxTime");
			EditorGUILayout.Slider(partOfTheMaxTimeProp, 1f, 5f, new GUIContent("При старте", "Часть от общего времени (1/2, 1/3, 1/4...), которая будет дана при старте"));

			serializedObject.ApplyModifiedProperties();     // сохраняем изменения введенных значений
		}
	}
}
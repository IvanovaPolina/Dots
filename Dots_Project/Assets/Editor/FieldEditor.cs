using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
	[CustomEditor(typeof(Field))]
	public class FieldEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI() {
			Field field = (Field)target;
			SerializedProperty _cellPrefab = serializedObject.FindProperty("cellPrefab");
			EditorGUILayout.PropertyField(_cellPrefab, new GUIContent("Префаб клетки", "Префаб клетки поля"));
			SerializedProperty _size = serializedObject.FindProperty("size");
			EditorGUILayout.IntSlider(_size, 2, 4, new GUIContent("Масштаб поля", "Масштаб квадратного поля (N x N)"));

			serializedObject.ApplyModifiedProperties();		// сохраняем изменения введенных значений
		}
	}
}
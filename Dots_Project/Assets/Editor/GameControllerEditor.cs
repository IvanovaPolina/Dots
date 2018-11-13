using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
	[CustomEditor(typeof(GameController))]
	public class GameControllerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI() {
			SerializedProperty trailPrefabProp = serializedObject.FindProperty("trailPrefab");
			EditorGUILayout.PropertyField(trailPrefabProp, new GUIContent("Префаб линии", "Объект с компонентом TrailRenderer"));
			SerializedProperty lineLengthProp = serializedObject.FindProperty("lineStartLength");
			EditorGUILayout.IntSlider(lineLengthProp, 2, 9, new GUIContent("Длина линии", "Изначальная длина линии при старте игры"));
			SerializedProperty drawSpeedProp = serializedObject.FindProperty("drawSpeed");
			EditorGUILayout.Slider(drawSpeedProp, 1f, 10f, new GUIContent("Скорость рисования", "Изначальная скорость отрисовки линии"));

			serializedObject.ApplyModifiedProperties();     // сохраняем изменения введенных значений
		}
	}
}
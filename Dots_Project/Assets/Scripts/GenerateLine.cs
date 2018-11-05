using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// Класс отвечает за генерацию и отрисовку рандомной линии
	/// </summary>
	public class GenerateLine : MonoBehaviour
	{
		/// <summary>
		/// Линия, сгенерированная компьютером
		/// </summary>
		public List<Cell> Path { get; private set; }

		/// <summary>
		/// Генерирует линию из count точек, которую должен повторить игрок
		/// </summary>
		/// <param name="count">Количество точек в линии</param>
		public void NewLine(int count) {
			Cell[,] gameField = Field.Instance.GameField;
			if (count < 2) count = 2;
			else if (count > gameField.Length) count = gameField.Length;
			List<Cell> path = new List<Cell>(count);

			// строим линию
			#region Test
			List<Cell> restCells = new List<Cell>(gameField.Length);    // создаем список оставшихся клеток
			for (int i = 0; i < gameField.GetLength(0); i++)
				for (int j = 0; j < gameField.GetLength(1); j++)
					restCells.Add(gameField[i, j]);

			for (int i = 0; i < count; i++) {       // формируем путь
				Cell temp = restCells[Random.Range(0, restCells.Count)];
				path.Add(temp);
				restCells.Remove(temp);
			}
			#endregion

			Path = path;
		}

		/// <summary>
		/// Рисует линию, сгенерированную компьютером
		/// </summary>
		/// <param name="trail">Компонент TrailRenderer, который будет представлять из себя линию</param>
		/// <param name="drawSpeed">Скорость отрисовки линии</param>
		/// <param name="endDelay">Задержка по времени после отрисовки линии</param>
		public IEnumerator DrawLine(TrailRenderer trail, float drawSpeed, float endDelay) {
			if (Path == null) yield break;
			trail.transform.position = Path[0].transform.position;  // задаем изначальную позицию линии
			trail.Clear();
			trail.enabled = true;
			for (int i = 1; i < Path.Count; i++) {
				float speed = 0;
				Vector3 start = trail.transform.position;
				Vector3 destination = Path[i].transform.position;
				while (Vector3.Distance(trail.transform.position, destination) > 0) {
					trail.transform.position = Vector3.Lerp(start, destination, speed);
					speed += drawSpeed;
					yield return null;
				}
			}
			yield return new WaitForSeconds(endDelay);    // задерживаем линию на какое-то время, дабы игрок её запомнил
			trail.enabled = false;
		}
	}
}
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

		#region Check
		public TrailRenderer line;
		private void Start() {
			NewLine(6);
			TrailRenderer trail = Instantiate(line);
			trail.enabled = false;
			StartCoroutine(DrawLine(trail, 0.02f, 1f));
		}
		#endregion

		/// <summary>
		/// Генерирует линию из count точек, которую должен повторить игрок
		/// </summary>
		/// <param name="count">Количество точек в линии</param>
		public void NewLine(int count) {
			Cell[,] gameField = Field.Instance.GameField;
			if (count < 2) count = 2;
			else if (count > gameField.Length) count = gameField.Length;
			List<Cell> path = new List<Cell>(count);

			// спавним первую точку в рандомной позиции
			int indexI = Random.Range(0, gameField.GetUpperBound(0) + 1);
			int indexJ = Random.Range(0, gameField.GetUpperBound(1) + 1);
			Cell firstPoint = gameField[indexI, indexJ];
			path.Add(firstPoint);
			firstPoint.ChangeColor(Color.blue);
			
			int countWithoutLostCorners = gameField.Length - 3;	// Кол-во клеток поля минус углы, которые могут быть потеряны
			RestPoints(path, count - path.Count, countWithoutLostCorners);

			// строим линию
			#region Test
			//List<Cell> restCells = new List<Cell>(gameField.Length);    // создаем список оставшихся клеток
			//for (int i = 0; i < gameField.GetLength(0); i++)
			//	for (int j = 0; j < gameField.GetLength(1); j++)
			//		restCells.Add(gameField[i, j]);

			//for (int i = 0; i < count; i++) {       // формируем путь
			//	Cell temp = restCells[Random.Range(0, restCells.Count)];
			//	path.Add(temp);
			//	restCells.Remove(temp);
			//}
			#endregion

			Path = path;
		}

		/// <summary>
		/// Проверяет последнюю точку пути на отсечение угловой точки поля. Если проверка не пройдена - заменяет точку на новую
		/// </summary>
		/// <param name="firstNeighbours">Соседние точки предпоследней точки пути</param>
		/// <param name="secondPoint">Последняя точка пути</param>
		private Cell CheckForEmptyCorners(List<Cell> firstNeighbours, Cell secondPoint) {
			#region Description
			// следует сразу избежать ситуации, когда генерация точек пойдет при подобных условиях:
			// count = 9, firstPoint = gameField[0, 1], secondPoint = gameField[1, 0]
			// таким образом мы теряем точку gameField[0, 0], и будем строить дальнейший путь зря (и потеряем время)
			// задача: отыскать точку в углу, которая могла бы оказаться отсеченной при неправильном выборе второй точки пути
			// решение: если первая и вторая точка имеют общего соседа (таких может быть два в нашем случае), 
			// который находится в углу (а значит имеет только трёх соседей, два из которых - это первая и вторая точка)
			// в таком случае меняем вторую точку на какую-либо другую
			#endregion
			bool find = false;  // флаг, определяющий, нашли ли мы точку, подходящую под наши условия
			while (!find) {
				List<Cell> secondNeighbours = new List<Cell>(secondPoint.Neighbours);
				foreach (var n in firstNeighbours) {
					if (secondNeighbours.Contains(n) && n.Neighbours.Count == 3) {
						firstNeighbours.Remove(secondPoint);
						secondPoint = firstNeighbours[Random.Range(0, firstNeighbours.Count)];
						if (secondPoint.Neighbours.Count == 8) return secondPoint;
						find = false;
						break;
					} else find = true;
				}
			}
			return secondPoint;
		}

		/// <summary>
		/// Выстраивает дальнейшие точки пути, основываясь на уже начатом пути
		/// </summary>
		/// <param name="path">Начатый путь</param>
		/// <param name="count">Количество точек, которые осталось задействовать в построении</param>
		/// <param name="countWithoutLostCorners">Величина, необходимая для проверки на отсечение угловых точек. = кол-во клеток поля минус углы, которые могут быть потеряны</param>
		private void RestPoints(List<Cell> path, int count, int countWithoutLostCorners) {
			// найти соседей последней точки из пути
			// если в этих соседях имеются точки пути - удалить из соседей эти точки
			// выбрать рандомную точку в этих соседях
			// проверка: если пусть содержит больше 3-х точек, значит надо делать проверку на пересечение с путём!
			// если нет - идем дальше. проверяем, построен ли путь (count == 0). Если да - return;
			// если (count > 0) строим еще точку

			// обернуть весь метод в цикл while (count > 0). Это исключит рекурсию.
			while (count > 0) {
				Cell tail = path[path.Count - 1];   // последний элемент в пути
				List<Cell> tailNeighbours = new List<Cell>(tail.Neighbours);
				//if(tailNeighbours.Count == 0) {
				//	count++;
				//	path.Remove(tail);
				//	continue;
				//}
				ClearNeighboursFromPathCells(tailNeighbours, path);
				Cell next = tailNeighbours[Random.Range(0, tailNeighbours.Count)];
				if(path.Count + count > countWithoutLostCorners)	// если путь обязан задействовать угловые клетки
					next = CheckForEmptyCorners(tailNeighbours, next);	// не допускаем отсечение углов последней точкой пути

				if(path.Count >= 3) {
					// делаем проверку на пересечение отрезков (next; path[path.Count - 1]) и остальных отрезков пути
					// если проверка не пройдена - continue;
					// если пройдена - идем дальше
					bool intersect = false;
					for (int i = 0; i < path.Count - 1; i++) {
						if (!IsIntersect(next.transform.position, tail.transform.position, path[i].transform.position, path[i + 1].transform.position)) {
							intersect = true;
							break;
						}
					}
					if (intersect) continue;
				}
				path.Add(next);
				count--;
			}
		}

		/// <summary>
		/// Убирает из списка соседей все клетки, которые уже есть в построенном пути
		/// </summary>
		/// <param name="neighbours">Список соседних клеток</param>
		/// <param name="path">Построенный путь</param>
		private void ClearNeighboursFromPathCells(List<Cell> neighbours, List<Cell> path) {
			for (int i = 0; i < neighbours.Count; i++) {
				if (path.Contains(neighbours[i])) {
					neighbours.Remove(neighbours[i]);
					i--;
				}
			}
		}

		/// <summary>
		/// Определяет, пересекаются ли отрезки AB и CD
		/// </summary>
		private bool IsIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D) {
			float v1 = (D.x - C.x) * (A.y - C.y) - (D.y - C.y) * (A.x - C.x);
			float v2 = (D.x - C.x) * (B.y - C.y) - (D.y - C.y) * (B.x - C.x);
			float v3 = (B.x - A.x) * (C.y - A.y) - (B.y - A.y) * (C.x - A.x);
			float v4 = (B.x - A.x) * (D.y - A.y) - (B.y - A.y) * (D.x - A.x);
			return ((v1 * v2 <= 0) && (v3 * v4 <= 0));
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
					// когда мы ставим игру на паузу через timeScale, инструкция yield return null продолжает выполняться,
					// потому что она рассчитывается, исходя из каждого фрейма, а не секунды.
					// из-за этого я сделала кривовато, но результативно:
					if (Time.timeScale == 0) yield return new WaitForSeconds(0.5f);
					trail.transform.position = Vector3.Lerp(start, destination, speed);
					speed += drawSpeed;
					yield return null;
				}
			}
			yield return new WaitForSeconds(endDelay);    // задерживаем линию на какое-то время, дабы игрок её запомнил
			trail.enabled = false;
		}

		#region IntersectsTest
		//private void IntersectsTest() {
		//	Debug.Log("В скобках указано значение, которое должно получиться");

		//	// для пересекающихся отрезков
		//	Vector2 A = new Vector2(1, 2);  // точка A
		//	Vector2 B = new Vector2(5, 6);  // точка B
		//	Vector2 C = new Vector2(2, 4);  // точка C
		//	Vector2 D = new Vector2(5, 3);  // точка D
		//	Debug.Log("Пересекающиеся отрезки (True): " + IsIntersect(A, B, C, D));

		//	// для параллельных отрезков
		//	A = new Vector2(1, 1);
		//	B = new Vector2(1, 4);
		//	C = new Vector2(3, 2);
		//	D = new Vector2(3, 4);
		//	Debug.Log("Параллельные (False): " + IsIntersect(A, B, C, D));

		//	// для непересекающихся отрезков
		//	A = new Vector2(1, 3);
		//	B = new Vector2(3, 4);
		//	C = new Vector2(2, 2);
		//	D = new Vector2(3, 1);
		//	Debug.Log("Непересекающиеся (False): " + IsIntersect(A, B, C, D));

		//	// для отрезков, у которых одна координата - общая
		//	A = new Vector2(1, 2);
		//	B = new Vector2(2, 3);
		//	C = new Vector2(2, 3);
		//	D = new Vector2(3, 5);
		//	Debug.Log("С одной общей координатой (True): " + IsIntersect(A, B, C, D));
		//}
		#endregion
	}
}
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
		/// Черный список для каждой точки пути. Содержит клетки, которые ранее приводили линию в тупик
		/// </summary>
		private List<List<Cell>> blackList = new List<List<Cell>>();

		#region Check
		public TrailRenderer line;
		private TrailRenderer trail;
		private void Start() {
			trail = Instantiate(line);
			trail.enabled = false;
			StartCoroutine(Test());
		}
		IEnumerator Test() {
			int testCount = Field.Instance.GameField.Length;
			while (true) {
				yield return new WaitForSeconds(1f);
				NewLine(testCount);
				yield return StartCoroutine(DrawLine(trail, 0.02f, 1f));
			}
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
			blackList.Clear();

			// спавним первую точку в рандомной позиции
			int indexI = Random.Range(0, gameField.GetUpperBound(0) + 1);
			int indexJ = Random.Range(0, gameField.GetUpperBound(1) + 1);
			Cell firstPoint = gameField[indexI, indexJ];
			path.Add(firstPoint);
			blackList.Add(new List<Cell>());	// создаем для первой точки черный список

			BuildRestPoints(path, count - path.Count);	// достраиваем остальные клетки
			Path = path;
		}

		/// <summary>
		/// Выстраивает дальнейшие точки пути, основываясь на уже начатом пути
		/// </summary>
		/// <param name="path">Начатый путь</param>
		/// <param name="count">Количество точек, которые осталось задействовать в построении</param>
		/// <param name="countWithoutLostCorners">Величина, необходимая для проверки на отсечение угловых точек. = кол-во клеток поля минус углы, которые могут быть потеряны</param>
		private void BuildRestPoints(List<Cell> path, int count) {
			while (count > 0) {     // если осталось 0 клеток - путь построен
				Cell tail = path[path.Count - 1];   // последний элемент в пути
				List<Cell> tailNeighbours = new List<Cell>(tail.Neighbours);
				// очищаем от соседних клеток, которые уже есть в пути
				ClearNeighboursFromPathCells(tailNeighbours, path);
				// очищаем от соседних клеток, которые ведут к пересечению линии самой себя
				ClearNeighboursFromIntersects(tailNeighbours, path);
				// очищаем от соседних клеток, которые уже были задействованы ранее и привели линию в тупик
				ClearNeighboursFromBlackList(tailNeighbours, path);
				if (tailNeighbours.Count == 0) {	// если после всех очисток соседних клеток не осталось
					blackList[path.Count - 2].Add(tail);	// заносим последнюю клетку в черный список предпоследней клетки 
					count++;
					path.Remove(tail);
					continue;	// запускаем цикл while заново
				}	// если соседние клетки остались - строим путь дальше
				Cell next = tailNeighbours[Random.Range(0, tailNeighbours.Count)];
				path.Add(next);
				count--;
				blackList.Add(new List<Cell>());	// создаем для этой клетки черный список
			}
		}

		/// <summary>
		/// Очищает список соседей от всех клеток, которые были задействованы ранее и привели линию в тупик
		/// </summary>
		private void ClearNeighboursFromBlackList(List<Cell> tailNeighbours, List<Cell> path) {
			int tailIndex = path.Count - 1;		// индекс последней клетки из пути
			if (blackList[tailIndex].Count > 0) {	// если в черном списке есть соседи этой клетки, которые приводили ранее линию в тупик
				foreach (var item in blackList[tailIndex])	// очищаем список соседей от уже ненужных клеток
					tailNeighbours.Remove(item);
				// если последний индекс черного списка больше, чем последний индекс пути (если в blackList записано больше точек, чем осталось в пути)
				// очищаем лист от использованных точек, для того, чтобы в дальнейшем его переиспользовать
				if (blackList.Count - 1 > tailIndex)	// if(4 - 1 > 2)
					blackList.RemoveRange(tailIndex + 1, blackList.Count - 1 - tailIndex);	// RemoveRange(3, 1);
			}
		}

		/// <summary>
		/// Убирает из списка соседей все клетки, которые уже есть в построенном пути
		/// </summary>
		private void ClearNeighboursFromPathCells(List<Cell> neighbours, List<Cell> path) {
			for (int i = 0; i < neighbours.Count; i++) {
				if (path.Contains(neighbours[i])) {
					neighbours.Remove(neighbours[i]);
					i--;
				}
			}
		}

		/// <summary>
		/// Убирает из списка соседей все клетки, при соединении с которыми линия пересекает саму себя
		/// </summary>
		private void ClearNeighboursFromIntersects(List<Cell> neighbours, List<Cell> path) {
			if (path.Count < 3) return;     // для линии из < 3 точек нет смысла производить расчеты
			// берем последнюю точку пути, соединяем с соседней точкой - получаем один отрезок
			// берем по очереди отрезок пути (2 последовательные точки) - это второй отрезок
			// сравниваем эти два отрезка на пересечение. В случае пересечения удаляем эту соседнюю точку
			for (int i = 0; i < neighbours.Count; i++) {	// перебираем соседние точки
				for (int j = 0; j < path.Count - 2; j++) {	// перебираем точки из пути (последние две не берем, т.к. они образовывают угол с отрезком, содержащим соседнюю точку)
					if (IsIntersect(path[path.Count - 1], neighbours[i], path[j], path[j + 1])) {
						neighbours.RemoveAt(i);
						i--;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Определяет, пересекаются ли отрезки AB и CD
		/// </summary>
		private bool IsIntersect(Cell A, Cell B, Cell C, Cell D) {
			float v1 = (D.I - C.I) * (A.J - C.J) - (D.J - C.J) * (A.I - C.I);
			float v2 = (D.I - C.I) * (B.J - C.J) - (D.J - C.J) * (B.I - C.I);
			float v3 = (B.I - A.I) * (C.J - A.J) - (B.J - A.J) * (C.I - A.I);
			float v4 = (B.I - A.I) * (D.J - A.J) - (B.J - A.J) * (D.I - A.I);
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

		#region Intersects_Test
		/// <summary>
		/// Проверяет два отрезка на пересечение (рассчитано для поля 3х3 и больше)
		/// </summary>
		private void IntersectsTest() {
			Cell[,] field = Field.Instance.GameField;
			Debug.Log("В скобках указано значение, которое должно получиться");

			// для пересекающихся отрезков
			Cell A = field[0, 0];  // клетка A
			Cell B = field[1, 1];  // клетка B
			Cell C = field[0, 1];  // клетка C
			Cell D = field[1, 0];  // клетка D
			Debug.Log("Пересекающиеся отрезки (True): " + IsIntersect(A, B, C, D));

			// для параллельных отрезков
			A = field[0, 0];
			B = field[1, 0];
			C = field[0, 1];
			D = field[1, 1];
			Debug.Log("Параллельные (False): " + IsIntersect(A, B, C, D));

			// для непересекающихся отрезков
			A = field[0, 0];
			B = field[1, 1];
			C = field[2, 1];
			D = field[1, 2];
			Debug.Log("Непересекающиеся (False): " + IsIntersect(A, B, C, D));

			// для отрезков, у которых одна координата - общая
			A = field[0, 0];
			B = field[0, 1];
			C = field[0, 1];
			D = field[1, 1];
			Debug.Log("С одной общей координатой (True): " + IsIntersect(A, B, C, D));
		}
		#endregion
	}
}
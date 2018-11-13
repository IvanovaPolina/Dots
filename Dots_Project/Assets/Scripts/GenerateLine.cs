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
		private List<List<Cell>> blackList = new List<List<Cell>>(9);

		#region Test
		// перед тестом выключаем все скрипты, кроме Field, и добавляем этот скрипт на сцену
		//public TrailRenderer line;
		//private TrailRenderer trail;
		//private void Start() {
		//	//trail = Instantiate(line);
		//	//trail.enabled = false;
		//	//StartCoroutine(Test());

		//	IntersectsTest();
		//}
		//IEnumerator Test() {
		//	int testCount = Field.Instance.GameField.Length;
		//	while (true) {
		//		yield return new WaitForSeconds(1f);
		//		NewLine(testCount);
		//		yield return StartCoroutine(DrawLine(trail, 0.02f, 1f));
		//	}
		//}
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
			Path = path;	// запоминаем построенный путь
		}

		/// <summary>
		/// Выстраивает дальнейшие точки пути, основываясь на уже начатом пути
		/// </summary>
		/// <param name="path">Начатый путь</param>
		/// <param name="count">Количество точек, которые осталось задействовать в построении</param>
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
				if (tailNeighbours.Count == 0) {    // если после всех очисток соседних клеток не осталось
					blackList[path.Count - 2].Add(tail);    // заносим эту клетку в черный список предпоследней клетки 
					count++;
					path.Remove(tail);
					// очищаем черный список от использованных точек, для последующих ходов по ним через другие пути
					if (blackList.Count > path.Count)    // if(4 > 3)
						blackList.RemoveRange(path.Count, blackList.Count - path.Count);  // RemoveRange(3, 1);
					continue;   // запускаем цикл while заново
				}   // если соседние клетки остались - строим путь дальше
				Cell next = tailNeighbours[Random.Range(0, tailNeighbours.Count)];
				path.Add(next);
				count--;
				blackList.Add(new List<Cell>());    // создаем для новой клетки черный список
			}
		}

		/// <summary>
		/// Очищает список соседей от всех клеток, которые были задействованы ранее и привели линию в тупик
		/// </summary>
		private void ClearNeighboursFromBlackList(List<Cell> tailNeighbours, List<Cell> path) {
			int tailIndex = path.Count - 1;		// индекс последней клетки из пути
			if (blackList[tailIndex].Count > 0)	// если в черном списке есть соседи этой клетки, которые приводили ранее линию в тупик
				foreach (var item in blackList[tailIndex])	// очищаем список соседей от уже ненужных клеток
					tailNeighbours.Remove(item);
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
				for (int j = 0; j < path.Count - 2; j++) {  // перебираем точки из пути (последние две не берем, т.к. они образовывают угол с отрезком, содержащим соседнюю точку)
					if (IsIntersect(path[path.Count - 1], neighbours[i], path[j], path[j + 1])) {
						neighbours.RemoveAt(i);
						i--;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Определяет, пересекаются ли отрезки p1p2 и p3p4
		/// </summary>
		private bool IsIntersect(Cell p1, Cell p2, Cell p3, Cell p4) {
			Vector2 A = new Vector2(p1.I, p1.J);
			Vector2 B = new Vector2(p2.I, p2.J);
			Vector2 C = new Vector2(p3.I, p3.J);
			Vector2 D = new Vector2(p4.I, p4.J);

			float v1 = Vector3.Cross(D - C, A - C).z;
			float v2 = Vector3.Cross(D - C, B - C).z;
			float v3 = Vector3.Cross(B - A, C - A).z;
			float v4 = Vector3.Cross(B - A, D - A).z;

			//float n = Mathf.Abs(v4 - v3);
			//if (n == 0) return false;
			// расчет точки пересечения прямых. Если отрезки лежат на одной прямой и не имеют общих точек,
			// деление на n даст нам 0. Поэтому включаем это условие в возвращаемое значение.
			// источник двух нижних формул - статья на хабре: https://habr.com/post/267037/
			//float crossX = C.x + (D.x - C.x) * Mathf.Abs(v3) / n;
			//float crossY = C.y + (D.y - C.y) * Mathf.Abs(v3) / n;
			//Debug.Log("Crossing point: " + new Vector2(crossX, crossY));
			return (v1 * v2 <= 0) && (v3 * v4 <= 0) && (v4 - v3 != 0);

			#region Description
			// проверяем взаимное расположение отрезков с помощью векторных произведений:
			// v1 = [p3 p4, p3 p1]; v2 = [p3 p4, p3 p2]; v3 = [p1 p2, p1 p3]; v4 = [p1 p2, p1 p4]
			// векторное произведение для векторов [a,b] вычисляется по формуле: ax*by-bx*ay;
			//float v1 = (p4.I - p3.I) * (p1.J - p3.J) - (p1.I - p3.I) * (p4.J - p3.J);   // a = p4-p3; b = p1-p3
			//float v2 = (p4.I - p3.I) * (p2.J - p3.J) - (p2.I - p3.I) * (p4.J - p3.J);   // x = I, y = J
			//float v3 = (p2.I - p1.I) * (p3.J - p1.J) - (p3.I - p1.I) * (p2.J - p1.J);
			//float v4 = (p2.I - p1.I) * (p4.J - p1.J) - (p4.I - p1.I) * (p2.J - p1.J);
			//return ((v1 * v2 <= 0) && (v3 * v4 <= 0));
			// функция расчета формулы ax*by-bx*ay - это Vector3.Cross. Нам требуется значение
			// результирующего вектора по оси Z, т.к. получаемый вектор перпендикулярен
			// исходным двум, и его координаты по X и Y в нашем случае равны нулю.
			#endregion
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
		/// Проверяет два отрезка на пересечение (рассчитано для поля 4х4 и больше)
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

			// для пересекающихся отрезков (инвертированные координаты)
			A = field[1, 1];
			B = field[0, 0];
			C = field[0, 1];
			D = field[1, 0];
			Debug.Log("Пересекающиеся отрезки инверт. (True): " + IsIntersect(A, B, C, D));
			
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
			
			// для отрезков, лежащих на одной горизонтальной прямой
			A = field[0, 0];
			B = field[0, 1];
			C = field[0, 2];
			D = field[0, 3];
			Debug.Log("Отрезки на одной горизонтальной прямой (False): " + IsIntersect(A, B, C, D));

			// для отрезков, лежащих на одной вертикальной прямой
			A = field[0, 0];
			B = field[1, 0];
			C = field[2, 0];
			D = field[3, 0];
			Debug.Log("Отрезки на одной вертикальной прямой (False): " + IsIntersect(A, B, C, D));
		}
		#endregion
	}
}
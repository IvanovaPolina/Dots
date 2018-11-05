using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
	public class Path : MonoBehaviour
	{
		[SerializeField]
		private GameObject edgesParent;

		private Edge[] allEdges;
		int i;  // из скольки ребер будет состоять путь

		/// <summary>
		/// Готовый путь
		/// </summary>
		public List<Edge> TargetPath { get; private set; }

		private void Start() {
			allEdges = edgesParent.GetComponentsInChildren<Edge>();

			SetPath(8);     // пока что задаем эту функцию в старте
			StartCoroutine(DrawPath());
		}

		IEnumerator DrawPath() {
			foreach (var edge in TargetPath) {
				yield return new WaitForSeconds(1f);
				edge.GetComponent<SpriteRenderer>().color = Color.green;
			}
		}

		/// <summary>
		/// Строит путь заданной длины
		/// </summary>
		/// <param name="pathLength">Из скольки отрезков будет состоять путь?</param>
		public void SetPath(int pathLength) {
			int dotsCount = allEdges.Length / 2 - 1;    // количество соединяемых точек
			if (pathLength > dotsCount - 1)     // если задана слишком большая длина пути
				pathLength = dotsCount;     // задаем максимально возможную длину
			else if (pathLength <= 0)       // если передали отрицательное значение
				pathLength = 1;     // устанавливаем минимально возможную длину пути
			i = pathLength;
			TargetPath = new List<Edge>(i);
			Edge start = allEdges[Random.Range(0, allEdges.Length)];    // первое ребро
			i--;
			TargetPath.Add(start);
			Vector2 randomPoint = Random.Range(0, 2) == 0 ? start.StartPoint : start.EndPoint;  // строим путь от рандомного конца ребра
			List<Edge> startNeighbours = GetEdgesFromPoint(randomPoint, start);    // находим все сцепленные ребра
			BuildPath(startNeighbours, start);
		}

		/// <summary>
		/// Выстраивает путь, основываясь на конечной точке предыдущего ребра
		/// </summary>
		private void BuildPath(List<Edge> neighbours, Edge prevEdge) {
			Edge next = neighbours[Random.Range(0, neighbours.Count)];  // выбираем рандомное
			i--;
			TargetPath.Add(next);
			// окончание предыдущего ребра (endPoint) - это начало следующего
			// поэтому для поиска следующего ребра мы находим окончание нового ребра,
			// и рекурсивно вызываем тот же метод до тех пор, пока не выстроится путь
			if (i <= 0) return; // в данном случае путь построен
			Vector2 samePoint = next.TheSamePoint(prevEdge);    // ищем точку соединения с предыдущим ребром
			Vector2 oppositePointOfNext = next.GetOppositePoint(samePoint); // конец следующего ребра - это точка, противоположная той, что соединяла его с предыдущим
			List<Edge> nextNeighbours = GetEdgesFromPoint(oppositePointOfNext, next);    // есть ли у следующего ребра соседи (есть ли куда двигаться?)
			if (i > 0 && nextNeighbours.Count > 0)    // если путь еще не выстроен и есть куда продолжать
				BuildPath(nextNeighbours, next);   // продолжаем построение пути
			else if (i > 0 && nextNeighbours.Count <= 0)   // если путь еще не выстроен, а двигаться некуда
				ReturnToPrevEdge(neighbours, next, prevEdge); // возвращаемся на шаг назад и ищем новое ребро без учета загнавшего нас в тупик
		}

		private void ReturnToPrevEdge(List<Edge> neighbours, Edge next, Edge prevEdge) {
			i++;
			neighbours.Remove(next);    // сокращаем выбор, убирая ребро, зашедшее в тупик
			TargetPath.Remove(next);
			if (neighbours.Count <= 0) {        // если у предыдущего ребра не осталось соседних
				i++;
				TargetPath.Remove(prevEdge);
				Edge preprevEdge = TargetPath[TargetPath.Count - 1];
				Vector2 samePoint = preprevEdge.TheSamePoint(prevEdge);
				List<Edge> newNeighbours = GetEdgesFromPoint(samePoint, preprevEdge);
				newNeighbours.Remove(prevEdge);
				BuildPath(newNeighbours, preprevEdge);
				return;
			}
			BuildPath(neighbours, prevEdge);
		}

		/// <summary>
		/// Возвращает список всех сцепленных с данной точкой ребер
		/// </summary>
		/// <param name="point">Точка, от которой идут искомые ребра</param>
		/// <returns></returns>
		private List<Edge> GetEdgesFromPoint(Vector2 point, Edge notCount) {
			List<Edge> neighbours = new List<Edge>();   // список прилежащих ребер
			foreach (var e in allEdges) {       // для каждого ребра
												//if (e == notCount) continue;    // не берем в расчет ребро, соседей которого мы ищем
				if (TargetPath.Contains(e)) continue;   // не берем в расчет ребра пути
				if (e.StartPoint == point || e.EndPoint == point)   // если один из концов ребра имеет данные координаты
					if (!Intersection(e, notCount, point))      // проверяем, не пересекается ли он с любым другим ребром из имеющегося пути
						neighbours.Add(e);
			}
			return neighbours;
		}

		/// <summary>
		/// Проверяет искомое ребро на пересечение с ребрами пути
		/// </summary>
		/// <param name="e">Искомое ребро</param>
		/// <param name="notCount">Ребро, из которого получили искомое</param>
		/// <param name="point">Точка пересечения ребер notCount и e</param>
		/// <returns></returns>
		private bool Intersection(Edge e, Edge notCount, Vector2 point) {
			//Vector2 oppositePoint = e.GetOppositePoint(point);	// ищем крайнюю точку предполагаемого пути
			foreach (var edge in TargetPath) {  // проверяем для всех ребер пути
				if (edge == notCount) continue;     // кроме предпоследнего
													//if (e.TheSamePoint(edge) == oppositePoint) // проверяем на пересечение
				if (e.HasSamePoint(edge))
					return true;
			}
			return false;
		}
	}
}
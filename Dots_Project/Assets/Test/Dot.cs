using System.Collections.Generic;
using UnityEngine;

namespace Test
{
	public class Dot : MonoBehaviour
	{
		/// <summary>
		/// Координаты точки
		/// </summary>
		public Vector2 Position { get; private set; }

		/// <summary>
		/// Было ли касание данной точки?
		/// </summary>
		public bool IsTouched { get; set; }

		private void Awake() {
			Position = transform.position;  // кэшируем координаты точки
		}

		/// <summary>
		/// Возвращает соседние точки от текущей (не включая диагональные)
		/// </summary>
		public List<Dot> GetNeighbours(List<Dot> allDots) {
			List<Dot> neighbours = new List<Dot>();
			float distance = MinDistance(allDots);
			foreach (var dot in allDots)
				if (Vector3.Distance(dot.Position, Position) <= distance)
					neighbours.Add(dot);
			return neighbours;
		}

		/// <summary>
		/// Определяет кратчайшую дистанцию между текущей точкой и соседними
		/// </summary>
		/// <param name="allDots">Массив точек, в котором будем искать кратчайшую дистанцию</param>
		private float MinDistance(List<Dot> allDots) {
			float minDistance = float.MaxValue;
			foreach (var dot in allDots) {
				if (dot != this) {
					float distance = Vector3.Distance(dot.Position, Position);
					if (distance < minDistance)
						minDistance = distance;
				}
			}
			return minDistance;
		}
	}
}
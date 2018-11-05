using System.Collections.Generic;
using UnityEngine;

namespace Test
{
	public class BuildPath : MonoBehaviour
	{
		public GameObject dotsParent;   // GameObject, который содержит в себе все точки

		private Dot[] allDots;  // список всех точек
		private List<Dot> pathDots;     // список точек, из которых будет построен путь
										//private int pathLength = 3;		// изначальная длина пути (кол-во точек)

		private void Start() {
			allDots = dotsParent.GetComponentsInChildren<Dot>();

			int length = 9;
			Build(length);
		}

		private void Build(int length) {
			List<Dot> remains = new List<Dot>(allDots);

			Dot start = allDots[Random.Range(0, allDots.Length)];
			if (length > allDots.Length - 1)
				if (start.Position.x == 0 || start.Position.y == 0)
					foreach (var dot in allDots)
						if (Mathf.Abs(dot.Position.x) > 0 && Mathf.Abs(dot.Position.y) > 0)
							start = dot;

			Color clr = Color.green;
			start.GetComponent<SpriteRenderer>().color = clr;
			Debug.LogFormat("1 = {0}", start.Position);

			for (int i = 0; i < length - 1; i++) {
				remains.Remove(start);
				start = GetNextDot(start, remains);
				clr = Color.Lerp(Color.green, Color.white, 1 / (i + 1));
				start.GetComponent<SpriteRenderer>().color = clr;
				Debug.LogFormat("{0} = {1}", i + 2, start.Position);
			}
		}

		private Dot GetNextDot(Dot start, List<Dot> allDots) {
			var startNeighbours = start.GetNeighbours(allDots);     // соседи предыдущей точки

			// не соседи предыдущей точки
			List<Dot> notStartNeighbours = allDots;
			for (int i = 0; i < startNeighbours.Count; i++)
				if (notStartNeighbours.Contains(startNeighbours[i]))
					notStartNeighbours.Remove(startNeighbours[i]);

			Dot result = startNeighbours[0];
			int minCount = startNeighbours.Count;
			// ищем количество соседей у соседних точек. Отдаем приоритет тем, у которых соседей меньше
			foreach (var dot in startNeighbours) {
				int neighboursCount = dot.GetNeighbours(notStartNeighbours).Count;
				if (neighboursCount <= minCount) {
					minCount = neighboursCount;
					result = dot;
				}
			}
			return result;
		}
	}
}
using UnityEngine;

namespace Test
{
	public class Edge : MonoBehaviour
	{
		[SerializeField]
		private Dot startPoint;
		[SerializeField]
		private Dot endPoint;

		/// <summary>
		/// Начальная точка ребра
		/// </summary>
		public Vector2 StartPoint { get { return startPoint.Position; } }
		/// <summary>
		/// Конечная точка ребра
		/// </summary>
		public Vector2 EndPoint { get { return endPoint.Position; } }

		/// <summary>
		/// Возвращает противоположный конец ребра
		/// </summary>
		public Vector2 GetOppositePoint(Vector2 edgePoint) {
			if (edgePoint == StartPoint) return EndPoint;
			return StartPoint;
		}

		/// <summary>
		/// Возвращает точку на стыке двух ребер: текущего и other
		/// </summary>
		public Vector2 TheSamePoint(Edge other) {
			Vector2 otherStart = other.StartPoint;
			Vector2 otherEnd = other.EndPoint;
			if (otherStart == StartPoint || otherEnd == StartPoint) return StartPoint;
			else if (otherStart == EndPoint || otherEnd == EndPoint) return EndPoint;
			return new Vector2(0.13f, 0.21f);       // надо придумать, что сюда вставить вместо null
		}

		/// <summary>
		/// Имеет ли данное ребро общие точки с другим ребром?
		/// </summary>
		public bool HasSamePoint(Edge other) {
			Vector2 samePoint = TheSamePoint(other);
			return samePoint == StartPoint || samePoint == EndPoint;
		}
	}
}
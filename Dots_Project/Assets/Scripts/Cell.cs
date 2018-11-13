using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// Класс содержит логику работы с отдельной клеткой игрового поля
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class Cell : MonoBehaviour
	{
		/// <summary>
		/// Номер строки, в которой располагается клетка (отсчет: снизу - вверх)
		/// </summary>
		public int I { get; set; }
		/// <summary>
		/// Номер столбца, в котором располагается клетка (отсчет: слева - направо)
		/// </summary>
		public int J { get; set; }
		/// <summary>
		/// Список соседних клеток
		/// </summary>
		public List<Cell> Neighbours { get; private set; }
		/// <summary>
		/// Коллайдер клетки
		/// </summary>
		public Collider2D Collider2D { get { return col2D; } }
		private Collider2D col2D;

		private SpriteRenderer childRenderer;
		private Color baseColor;

		private void Awake() {
			col2D = GetComponent<Collider2D>();     // заранее кэшируем коллайдер
			childRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
			baseColor = childRenderer.color;    // кэшируем изначальный цвет клетки
		}

		/// <summary>
		/// Меняет цвет клетки
		/// </summary>
		/// <param name="changeColor">Задайте новый цвет клетки</param>
		public void ChangeColor(Color changeColor) {
			childRenderer.color = changeColor;
		}

		/// <summary>
		/// Меняет цвет клетки на её изначальный цвет
		/// </summary>
		public void ResetColor() {
			childRenderer.color = baseColor;
		}

		/// <summary>
		/// Инициализирует соседние клетки (в т.ч. по диагонали)
		/// </summary>
		public void InitNeighbours() {
			Neighbours = GetNeighbours(Field.Instance.GameField);
		}

		/// <summary>
		/// Возвращает все соседние клетки поля
		/// </summary>
		private List<Cell> GetNeighbours(Cell[,] field) {
			List<Cell> neighbours = new List<Cell>(8);
			var colliders = Physics2D.OverlapBoxAll(col2D.bounds.center, transform.localScale * 1.5f, 0);
			foreach (var item in colliders)
				if (item != col2D)
					neighbours.Add(item.GetComponent<Cell>());
			return neighbours;
		}
	}
}
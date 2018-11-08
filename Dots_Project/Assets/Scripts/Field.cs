﻿using UnityEngine;

namespace Game
{
	/// <summary>
	/// Класс, отвечающий за создание игрового поля
	/// </summary>
	public class Field : MonoBehaviour
	{
		/// <summary>
		/// Предоставляет доступ к данному классу (который имеет единственный экземпляр)
		/// </summary>
		public static Field Instance { get; private set; }

		[SerializeField]
		private Cell cellPrefab;

		private Cell[,] field;
		/// <summary>
		/// Игровое поле из клеток
		/// </summary>
		public Cell[,] GameField { get { return field; } }

		[SerializeField]
		private int size = 3;

		private void Awake() {
			// реализуем паттерн Singleton - создаем единственный экземпляр данного класса, т.к. игровое поле у нас одно
			if (Instance) DestroyImmediate(this);
			else Instance = this;

			// генерируем игровое поле
			field = new Cell[size, size];
			SpawnField();
			InitNeighbours();

			#region Test
			//foreach (var n in field[0, 0].Neighbours)
			//	n.ChangeColor(Color.blue);
			#endregion
		}

		/// <summary>
		/// Создает квадратное поле из клеток
		/// </summary>
		private void SpawnField() {
			Vector2 screenInPixels = new Vector2(Screen.width, Screen.height);  // координаты правого верхнего угла экрана в пикселях
			Vector2 screenInUnits = Camera.main.ScreenToWorldPoint(screenInPixels);  // координаты правого верхнего угла экрана в юнитах
			Vector2 zeroInUnit = Camera.main.ScreenToWorldPoint(Vector2.zero);  // координаты левого нижнего угла экрана в юнитах
			Vector2 screenScale = screenInUnits - zeroInUnit;   // масштаб экрана в юнитах

			// определяем минимальную сторону экрана, и получаем по ней размер клетки
			float minSide = screenScale.x < screenScale.y ? screenScale.x : screenScale.y;
			float cellSize = minSide / size;
			bool isHorizontal = false;  // ориентация экрана
			if (minSide == screenScale.y) isHorizontal = true;

			// определяем размер отступов от краёв экрана
			float indent = Mathf.Abs((screenScale.x - screenScale.y) / 2);

			// заполняем и отрисовываем массив клеток
			GameObject root = new GameObject("Root");
			for (int i = 0; i < field.GetLength(0); i++) {  // по вертикали
				for (int j = 0; j < field.GetLength(1); j++) {  // по горизонтали
					field[i, j] = Instantiate(cellPrefab, root.transform);
					field[i, j].I = i;
					field[i, j].J = j;
					float horizontal = cellSize / 2 + cellSize * j;     // позиция клетки по горизонтали
					float vertical = cellSize / 2 + cellSize * i;   // позиция клетки по вертикали
					// перед спавном клетки учитываем отступ и смещение от центра экрана в левый нижний угол
					if (isHorizontal) {
						horizontal += indent + zeroInUnit.x;
						vertical += zeroInUnit.y;
					} else {
						vertical += indent + zeroInUnit.y;
						horizontal += zeroInUnit.x;
					}
					field[i, j].transform.position = new Vector2(horizontal, vertical);
					field[i, j].transform.localScale = new Vector3(cellSize, cellSize, cellSize);
					field[i, j].name = string.Format("Cell ({0}, {1})", i, j);
				}
			}
		}

		/// <summary>
		/// Для каждой клетки поля инициализирует соседние ей клетки
		/// </summary>
		private void InitNeighbours() {
			foreach (var cell in field)
				cell.InitNeighbours();
		}

		///// <summary>
		///// Возвращает все соседние клетки от имеющейся
		///// </summary>
		///// <param name="cell">Клетка, соседей которой нужно найти</param>
		//private List<Cell> NeighbourCells(Cell cell) {
		//	List<Cell> neighbours = new List<Cell>();
		//	int i = cell.I, j = cell.J;
		//	if (i - 1 >= 0 && j - 1 >= 0) neighbours.Add(field[i - 1, j - 1]);  // слева снизу
		//	if (j - 1 >= 0) neighbours.Add(field[i, j - 1]);    // слева
		//	if (i + 1 < field.GetLength(0) && j - 1 >= 0) neighbours.Add(field[i + 1, j - 1]);  // слева сверху
		//	if (i + 1 < field.GetLength(0)) neighbours.Add(field[i + 1, j]);    // сверху
		//	if (i + 1 < field.GetLength(0) && j + 1 < field.GetLength(1)) neighbours.Add(field[i + 1, j + 1]);  // справа сверху
		//	if (j + 1 < field.GetLength(1)) neighbours.Add(field[i, j + 1]);    // справа
		//	if (i - 1 >= 0 && j + 1 < field.GetLength(1)) neighbours.Add(field[i - 1, j + 1]);  // справа снизу
		//	if (i - 1 >= 0) neighbours.Add(field[i - 1, j]);    // снизу
		//	return neighbours;
		//}
	}
}
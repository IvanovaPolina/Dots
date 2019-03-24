using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
	/// <summary>
	/// Класс содержит в себе логику повтора линии игроком
	/// </summary>
	public class RepeatLine : MonoBehaviour
	{
		#region Events

		/// <summary>
		/// Событие передаёт состояние игры, можно ли отображать взаимодействие с экраном
		/// </summary>
		public static event Action<bool> FingerCouldPress;
		/// <summary>
		/// Событие принимает в качестве аргумента значение, указывающее, коснулся ли игрок одной из точек линии.
		/// Вторым аргументом является данная клетка
		/// </summary>
		public static event Action<bool, Cell> RightCellClicked;
		/// <summary>
		/// Событие срабатывает при сбросе внешнего вида клеток до стандартного
		/// </summary>
		public static event Action<List<Cell>> CellsStatesReset;

		#endregion

		/// <summary>
		/// Правильно ли повторил игрок линию?
		/// </summary>
		public bool IsLineRepeated { get; private set; }
		/// <summary>
		/// Закончил ли игрок повторять линию?
		/// </summary>
		public bool IsFinishRepeating { get; private set; }

		private bool timeIsOver;
		private State currentState = State.None;

		private Cell currentCell;		// клетка, выбранная игроком на данный момент
		private List<Cell> restOfPath = new List<Cell>(9);		// сколько точек из линии осталось повторить
		private List<Cell> selectedCells = new List<Cell>(9);	// все точки, выделенные игроком
		
		/// <summary>
		/// Режим игры
		/// </summary>
		private enum State
		{
			/// <summary>
			/// Режим бездействия
			/// </summary>
			None,
			/// <summary>
			/// Режим ожидания повтора линии
			/// </summary>
			Waiting,
			/// <summary>
			/// Режим повтора линии
			/// </summary>
			Draw
		}

		private void Update() {
			if (timeIsOver) return;
			switch (currentState) {
				case State.None:
					NoneUpdate();
					break;
				case State.Waiting:
					WaitingUpdate();
					break;
				case State.Draw:
					DrawUpdate();
					break;
			}
		}

		#region ---------------- Выполняется каждый кадр ---------------------

		private void NoneUpdate() {

		}
		
		private void WaitingUpdate() {
			if (!CrossplatformInput.IsPressedDown && !CrossplatformInput.IsPressed) return;
			if (IsClickOnUI() || !IsClickOnTheCell()) return;
			SetState(State.Draw);
		}

		private void DrawUpdate() {
			if (CrossplatformInput.IsPressedUp || restOfPath.Count == 0) {
				IsLineRepeated = restOfPath.Count == 0;
				SetState(State.None);
				return;
			}
			if (!IsClickOnTheCell()) return;

			bool isRightCell = currentCell == restOfPath[0];
			if (RightCellClicked != null) RightCellClicked.Invoke(isRightCell, currentCell);
			currentCell.Collider2D.enabled = false;
			selectedCells.Add(currentCell);
			if (isRightCell) restOfPath.Remove(currentCell);
			else {
				IsLineRepeated = false;
				SetState(State.None);
			}
			currentCell = null;
		}

		#endregion --------------------------------------------------------------

		#region ---------------- Выполняется один раз ------------------------

		private void Awake()
		{
			Timer.TimerIsOver += GameOver;
		}

		private void OnDestroy()
		{
			Timer.TimerIsOver -= GameOver;
		}

		private void OnceNone() {
			IsFinishRepeating = true;
			if (FingerCouldPress != null) FingerCouldPress.Invoke(false);
			if (selectedCells.Count > 0) {
				if (CellsStatesReset != null) CellsStatesReset.Invoke(selectedCells);
				foreach (var cell in selectedCells)
					cell.Collider2D.enabled = true;
				selectedCells.Clear();
			}
		}

		private void OnceWait() {

		}

		private void OnceDraw() {
			if(FingerCouldPress != null) FingerCouldPress.Invoke(true);
		}

		#endregion --------------------------------------------------------------

		/// <summary>
		/// Устанавливает текущий режим игры (режим повтора линии, режим бездействия и т.д.)
		/// </summary>
		private void SetState(State state) {
			if (currentState == state) return;
			switch (state) {
				case State.None:
					OnceNone();
					break;
				case State.Waiting:
					OnceWait();
					break;
				case State.Draw:
					OnceDraw();
					break;
			}
			currentState = state;
		}

		/// <summary>
		/// Запускает режим повтора линии. Позволяет игроку начать повторять линию
		/// </summary>
		/// <param name="path">Список точек, по которым нужно построить линию</param>
		public void Repeat(List<Cell> path) {
			IsFinishRepeating = false;
			restOfPath = new List<Cell>(path);
			SetState(State.Waiting);
		}

		/// <summary>
		/// Перехватывает нажатие на элементах UI, отмеченных как Raycast Target
		/// </summary>
		private bool IsClickOnUI() {
			return EventSystem.current.currentSelectedGameObject != null;
		}

		/// <summary>
		/// Распознаёт нажатие по клетке игрового поля
		/// </summary>
		private bool IsClickOnTheCell() {
			Ray ray = Camera.main.ScreenPointToRay(CrossplatformInput.CurrentPosition);
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Cell"));
			currentCell = hit ? hit.collider.GetComponent<Cell>() : null;
			return hit;
		}

		/// <summary>
		/// Прерывает повтор линии, если таймер игры истек
		/// </summary>
		private void GameOver()
		{
			IsFinishRepeating = true;
			timeIsOver = true;
			if (FingerCouldPress != null)
				FingerCouldPress.Invoke(false);
		}
	}
}
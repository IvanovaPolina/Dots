using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game
{
	/// <summary>
	/// Класс содержит в себе логику повтора линии игроком
	/// </summary>
	public class RepeatLine : MonoBehaviour
	{
		/// <summary>
		/// Делегат принимает в качестве аргумента значение, указывающее, коснулся ли игрок одной из точек линии.
		/// Вторым аргументом является данная клетка
		/// </summary>
		public static Action<bool, Cell> OnRightCellClicked;
		/// <summary>
		/// Делегат срабатывает при сбросе внешнего вида клеток до стандартного
		/// </summary>
		public static Action<List<Cell>> OnCellsStatesReset;
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
			if (Timer.Instance.RestTime <= 0) {
				IsFinishRepeating = true;
				timeIsOver = true;
			}
		}

// ---------------- Выполняется каждый кадр ---------------------
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
			if (OnRightCellClicked != null) OnRightCellClicked.Invoke(isRightCell, currentCell);
			currentCell.Collider2D.enabled = false;
			selectedCells.Add(currentCell);
			if (isRightCell) restOfPath.Remove(currentCell);
			else {
				IsLineRepeated = false;
				SetState(State.None);
			}
			currentCell = null;
		}
// --------------------------------------------------------------

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

// ---------------- Выполняется один раз ------------------------
		private void OnceNone() {
			IsFinishRepeating = true;
			if(selectedCells.Count > 0) {
				if (OnCellsStatesReset != null) OnCellsStatesReset.Invoke(selectedCells);
				foreach (var cell in selectedCells)
					cell.Collider2D.enabled = true;
				selectedCells.Clear();
			}
		}

		private void OnceWait() {

		}

		private void OnceDraw() {

		}
// --------------------------------------------------------------

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
	}
}
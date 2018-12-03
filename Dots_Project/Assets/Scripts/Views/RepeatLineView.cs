using System.Collections.Generic;
using UnityEngine;

namespace Game.Views
{
	/// <summary>
	/// Класс отвечает за отображение действий игрока при повторе линии
	/// </summary>
	public class RepeatLineView : MonoBehaviour
	{
		[SerializeField]
		private AudioClip rightClick, wrongClick;

		private AudioSource audioSource;

		private void Awake() {
			audioSource = GetComponent<AudioSource>();
			RepeatLine.OnRightCellClicked += ChangeCellState;
			RepeatLine.OnCellsStatesReset += ResetCellsStates;
		}

		private void OnDestroy() {
			RepeatLine.OnRightCellClicked -= ChangeCellState;
			RepeatLine.OnCellsStatesReset -= ResetCellsStates;
		}

		/// <summary>
		/// Меняет внешний вид клетки в зависимости от того, правильная она или нет
		/// </summary>
		/// <param name="state">Является ли выбранная игроком клетка правильной</param>
		/// <param name="cell">Клетка, выбранная игроком</param>
		private void ChangeCellState(bool state, Cell cell) {
			if (state) {
				audioSource.PlayOneShot(rightClick);
				cell.ChangeColor(Color.green);
			} else {
				audioSource.PlayOneShot(wrongClick);
				cell.ChangeColor(Color.red);
			}
		}

		/// <summary>
		/// Возвращает всем клеткам их стандартный вид
		/// </summary>
		private void ResetCellsStates(List<Cell> cells) {
			if (cells == null || cells.Count == 0) return;
			foreach (var cell in cells)
				cell.ResetColor();
		}
	}
}
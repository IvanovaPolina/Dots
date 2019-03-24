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
		private AudioClip rightClick;
		[SerializeField]
		private AudioClip wrongClick;
		[SerializeField]
		private TrailRenderer fingerTrail;

		private AudioSource audioSource;

		private void Awake() {
			audioSource = GetComponent<AudioSource>();
			RepeatLine.RightCellClicked += ChangeCellState;
			RepeatLine.CellsStatesReset += ResetCellsStates;
			fingerTrail = Instantiate(fingerTrail);
			CreateTrail(false);
			RepeatLine.FingerCouldPress += CreateTrail;
		}

		private void Update() {
			if (!fingerTrail.enabled) return;
			Vector2 pos = Camera.main.ScreenToWorldPoint(CrossplatformInput.CurrentPosition);
			fingerTrail.transform.position = pos;
		}

		private void OnDestroy() {
			RepeatLine.RightCellClicked -= ChangeCellState;
			RepeatLine.CellsStatesReset -= ResetCellsStates;
			RepeatLine.FingerCouldPress -= CreateTrail;
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

		/// <summary>
		/// Активирует/деактивирует отображение следа при взаимодействии с экраном
		/// </summary>
		private void CreateTrail(bool emitting) {
			if (emitting) {
				Vector2 pos = Camera.main.ScreenToWorldPoint(CrossplatformInput.CurrentPosition);
				fingerTrail.transform.position = pos;
				fingerTrail.Clear();
				fingerTrail.enabled = true;
			} else fingerTrail.enabled = false;
		}
	}
}
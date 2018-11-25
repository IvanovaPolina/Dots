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
		}

		private void OnDestroy() {
			RepeatLine.OnRightCellClicked -= ChangeCellState;
		}
		
		private void ChangeCellState(bool state, Cell cell) {
			if(state) {
				audioSource.PlayOneShot(rightClick);
				cell.ChangeColor(Color.green);
				return;
			}
			audioSource.PlayOneShot(wrongClick);
			cell.ChangeColor(Color.red);
		}
	}
}
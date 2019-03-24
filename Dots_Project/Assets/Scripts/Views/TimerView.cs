using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
	/// <summary>
	/// Класс отвечает за отображение игрового времени в форме слайдера
	/// </summary>
	[RequireComponent(typeof(Slider))]
	public class TimerView : MonoBehaviour
	{
		private Slider slider;

		private void Awake() {
			Timer.TimerValueChanged += TimerValue;
			slider = GetComponent<Slider>();
		}

		private void OnDestroy() {
			Timer.TimerValueChanged -= TimerValue;
		}

		/// <summary>
		/// Меняет заполнение слайдера в зависимости от передаваемого значения таймера
		/// </summary>
		private void TimerValue(float value) {
			slider.value = value;
		}
	}
}
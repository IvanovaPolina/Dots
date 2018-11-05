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
			Timer.OnTimerValueChanged += TimerValue;    // подписываемся на событие, отвечающее за значение таймера
			slider = GetComponent<Slider>();
		}

		private void OnDestroy() {
			Timer.OnTimerValueChanged -= TimerValue;    // не забываем отписываться от события
		}

		/// <summary>
		/// Меняет заполнение слайдера в зависимости от передаваемого значения таймера
		/// </summary>
		/// <param name="value">Текущее значение таймера</param>
		private void TimerValue(float value) {
			slider.value = value;
		}
	}
}
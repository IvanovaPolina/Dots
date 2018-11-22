using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
	/// <summary>
	/// Класс содержит в себе логику работы с таймером
	/// </summary>
	public class Timer : MonoBehaviour
	{
		[Tooltip("Максимальное значение шкалы времени на протяжении игры")]
		[Range(10f, 30f)] [SerializeField]
		private float maxTime = 30f;    // максимальное значение шкалы времени на протяжении игры
		[Tooltip("Часть от максимального времени (1/2, 1/3 ... 1/PartOfTheMaxTime), которая будет дана при старте")]
		[Range(1f, 5f)] [SerializeField]
		private float partOfTheMaxTime = 4f; // часть максимального времени при старте игры (здесь: 1/4)

		/// <summary>
		/// Делегат отвечает за передачу измененного значения таймера
		/// </summary>
		public static UnityAction<float> OnTimerValueChanged;
		/// <summary>
		/// Делегат отвечает за передачу значения превышенного набранного времени от максимально доступного
		/// </summary>
		public static UnityAction<float> OnBonusValueChanged;
		/// <summary>
		/// Доступ к данному классу
		/// </summary>
		public static Timer Instance { get; private set; }
		/// <summary>
		/// Оставшееся время до конца игры
		/// </summary>
		public float RestTime { get; private set; }
		/// <summary>
		/// Время, набранное игроком сверх максимального
		/// </summary>
		public float Bonus { get; private set; }
		/// <summary>
		/// Показатель оставшегося времени (в %)
		/// </summary>
		private float Proportion { get { return RestTime / maxTime; } }
		/// <summary>
		/// Время, потраченное на прохождение уровня
		/// </summary>
		public float LevelTime { get; private set; }

		private float frequency = 1f;       // частота убывания времени (единиц в секунду)
		private float penalty = 1f;     // время, отнимаемое за неправильно повторенную линию

		private void Awake() {
			// создаем единственный экземпляр данного класса, т.к. время у нас одно на протяжении всей игры
			// и доступ к данному классу необходим без создания его экземпляров
			if (Instance) DestroyImmediate(this);
			else Instance = this;

			RestTime = maxTime / partOfTheMaxTime;
		}

		private void Start() {
			if (OnTimerValueChanged != null) OnTimerValueChanged.Invoke(Proportion);
		}

		/// <summary>
		/// Начинает/возобновляет отсчет таймера
		/// </summary>
		public void StartCounting() {
			LevelTime = RestTime;
			StartCoroutine("StartCount");
		}

		/// <summary>
		/// Останавливает отсчет таймера
		/// </summary>
		public void StopCounting() {
			LevelTime -= RestTime;
			StopCoroutine("StartCount");
		}

		/// <summary>
		/// Запускает таймер "на убывание"
		/// </summary>
		private IEnumerator StartCount() {
			while (true) {
				if (RestTime > 0) {
					RestTime -= frequency;
					if (OnTimerValueChanged != null) OnTimerValueChanged.Invoke(Proportion);
				} else yield break;
				//Debug.Log(RestTime);
				yield return new WaitForSeconds(1f);
			}
		}

		/// <summary>
		/// Удваивает оставшееся время
		/// </summary>
		public void MultTime() {
			RestTime *= 2f;
			if (RestTime > maxTime) {
				Bonus = RestTime - maxTime;
				if(OnBonusValueChanged != null) OnBonusValueChanged.Invoke(Bonus);
				RestTime = maxTime;
			}
			if(OnTimerValueChanged != null) OnTimerValueChanged.Invoke(Proportion);
		}

		/// <summary>
		/// Убавляет оставшееся время
		/// </summary>
		public void ReduceTime() {
			RestTime -= penalty;
			if (OnTimerValueChanged != null) OnTimerValueChanged.Invoke(Proportion);
		}
	}
}
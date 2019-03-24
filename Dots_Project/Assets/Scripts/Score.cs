using System;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// Класс содержит логику начисления набранных игроком очков (другими словами, за счет)
	/// </summary>
	public class Score : MonoBehaviour
	{
		/// <summary>
		/// Доступ к данному классу
		/// </summary>
		public static Score Instance { get; private set; }
		/// <summary>
		/// Текущий счет
		/// </summary>
		public float CurrentScore { get { return score; } }

		private float score = 0;

		#region Events

		/// <summary>
		/// Событие отвечает за передачу измененного значения счета
		/// </summary>
		public static event Action<float> ScoreValueChanged;
		/// <summary>
		/// Событие отвечает за передачу значения текущих бонусных очков
		/// </summary>
		public static event Action<float> BonusValueChanged;

		#endregion

		private void Awake() {
			// создаем единственный экземпляр данного класса
			if (Instance) DestroyImmediate(this);
			else Instance = this;

			Timer.BonusValueChanged += BonusChange;
		}

		private void OnDestroy() {
			Timer.BonusValueChanged -= BonusChange;
		}

		/// <summary>
		/// Прибавляет количество очков к общему счету
		/// </summary>
		/// <param name="time">Время, потраченное на прохождение уровня</param>
		/// <param name="dotsCount">Количество соединенных точек</param>
		public void Plus(float time, int dotsCount) {
			score += time * dotsCount;
			if (ScoreValueChanged != null) ScoreValueChanged.Invoke(score);
		}

		/// <summary>
		/// Рассчитывает бонусные очки в зависимости от переданного времени
		/// </summary>
		/// <param name="bonusTime">Время, набранное игроком сверх максимального</param>
		private void BonusChange(float bonusTime) {
			float tempBonus = bonusTime / 2f;
			score += tempBonus;
			if(BonusValueChanged != null) BonusValueChanged.Invoke(tempBonus);
		}
	}
}
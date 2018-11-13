using UnityEngine;
using UnityEngine.Events;

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
		/// Делегат отвечает за передачу измененного значения счета
		/// </summary>
		public static UnityAction<float> OnScoreValueChanged;
		/// <summary>
		/// Делегат отвечает за передачу значения текущих бонусных очков
		/// </summary>
		public static UnityAction<float> OnBonusValueChanged;
		/// <summary>
		/// Текущий счет
		/// </summary>
		public float CurrentScore { get { return score; } }

		private float score = 0;

		private void Awake() {
			// создаем единственный экземпляр данного класса
			if (Instance) DestroyImmediate(this);
			else Instance = this;

			Timer.OnBonusValueChanged += BonusChange;
		}

		private void OnDestroy() {
			Timer.OnBonusValueChanged -= BonusChange;
		}

		/// <summary>
		/// Прибавляет количество очков к общему счету
		/// </summary>
		/// <param name="time">Время, потраченное на прохождение уровня</param>
		/// <param name="dotsCount">Количество соединенных точек</param>
		public void Plus(float time, int dotsCount) {
			score += time * dotsCount;
			if (OnScoreValueChanged != null) OnScoreValueChanged.Invoke(score);
		}

		/// <summary>
		/// Рассчитывает бонусные очки в зависимости от переданного времени
		/// </summary>
		/// <param name="bonusTime">Время, набранное игроком сверх максимального</param>
		private void BonusChange(float bonusTime) {
			float tempBonus = bonusTime / 2f;
			score += tempBonus;
			if(OnBonusValueChanged != null) OnBonusValueChanged.Invoke(tempBonus);
		}
	}
}
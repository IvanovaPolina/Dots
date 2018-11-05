using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
	/// <summary>
	/// Класс отвечает за отображение в текстовом поле набранных игроком очков
	/// </summary>
	[RequireComponent(typeof(Text))]
	public class ScoreView : MonoBehaviour
	{
		[SerializeField]
		private Text bonusScore;
		[SerializeField]
		private Animator bonusAnimator;

		private Text mainScore;

		private void Awake() {
			Score.OnScoreValueChanged += ChangeScore;
			Score.OnBonusValueChanged += DisplayBonus;
			mainScore = GetComponent<Text>();
		}

		private void OnDestroy() {
			Score.OnScoreValueChanged -= ChangeScore;
			Score.OnBonusValueChanged -= DisplayBonus;
		}

		/// <summary>
		/// Обновляет показатель счета и отображает его в текстовом поле
		/// </summary>
		/// <param name="score">Набранный счет</param>
		private void ChangeScore(float score) {
			mainScore.text = Mathf.RoundToInt(score).ToString();
		}

		/// <summary>
		/// Отображает бонусные очки
		/// </summary>
		/// <param name="bonus">Значение, которое следует отобразить</param>
		private void DisplayBonus(float bonus) {
			bonusScore.text = "+" + Mathf.RoundToInt(bonus).ToString();
			bonusAnimator.SetTrigger("Bonus");
		}
	}
}
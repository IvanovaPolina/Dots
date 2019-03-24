using System;
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
		[SerializeField]
		private AudioSource audioSource;
		[SerializeField]
		private AudioClip bonusClip;

		private Text mainScore;

		private void Awake() {
			Score.ScoreValueChanged += ChangeScore;
			Score.BonusValueChanged += DisplayBonus;
			mainScore = GetComponent<Text>();
		}

		private void OnDestroy() {
			Score.ScoreValueChanged -= ChangeScore;
			Score.BonusValueChanged -= DisplayBonus;
		}

		/// <summary>
		/// Обновляет показатель счета и отображает его в текстовом поле
		/// </summary>
		private void ChangeScore(float score) {
			mainScore.text = Math.Round(score).ToString();
		}

		/// <summary>
		/// Отображает бонусные очки
		/// </summary>
		private void DisplayBonus(float bonus) {
			if (bonus <= 0) return;
			bonusScore.text = "+" + Math.Round(bonus).ToString();
			bonusAnimator.SetTrigger("Bonus");
			audioSource.PlayOneShot(bonusClip);
		}
	}
}
using System.Collections;
using UnityEngine;
using Game.Menu;
using Game.Menu.Records;

namespace Game
{
	/// <summary>
	/// Класс содержит основную логику игры
	/// </summary>
	public class GameController : MonoBehaviour
	{
		private GenerateLine generateLine;  // скрипт, который генерирует линию
		private RepeatLine repeatLine;      // скрипт, который позволяет игроку повторить линию
		private Timer timer;
		private Score score;
		
		[Tooltip("Изначальная длина линии при старте игры")]
		[Range(2,9)] [SerializeField]
		private int lineStartLength = 3;    // изначальная длина линии
		private int lineMaxLength;      // максимальная длина линии

		[SerializeField]
		private TrailRenderer trailPrefab;   // префаб будущей линии
		private TrailRenderer trail;    // ссылка на заспавненную линию
		[Tooltip("Изначальная скорость отрисовки линии")]
		[Range(1f, 10f)] [SerializeField]
		private float drawSpeed = 2f;   // скорость отрисовки линии
		
		private AudioSource audioSource;

		private void Start() {
			generateLine = gameObject.AddComponent<GenerateLine>();
			repeatLine = gameObject.AddComponent<RepeatLine>();
			timer = Timer.Instance;
			score = gameObject.AddComponent<Score>();
			lineMaxLength = Field.Instance.GameField.Length;
			drawSpeed = drawSpeed / 100f;
			trail = Instantiate(trailPrefab);
			trail.enabled = false;
			audioSource = GetComponent<AudioSource>();
			StartCoroutine(Game());
		}

		private IEnumerator Game() {
			generateLine.NewLine(lineStartLength);
			while (timer.RestTime > 0) {
				yield return new WaitForSeconds(1f);    // задержка между уровнями
				if (repeatLine.IsLineRepeated) generateLine.NewLine(lineStartLength);
				yield return StartCoroutine(generateLine.DrawLine(trail, drawSpeed, 1f));   // ждем отрисовки линии
				timer.StartCounting();
				repeatLine.Repeat(generateLine.Path);
				yield return new WaitUntil(() => repeatLine.IsFinishRepeating); // ждем, пока игрок повторит её
				timer.StopCounting();
				CheckLineRepeating();
				yield return null;
			}
			audioSource.Play();
			TableData.Instance.SaveRecord(score.CurrentScore);
			GameMenu.Instance.DisplayPanel(GameMenu.Panel.Lose);
		}

		/// <summary>
		/// Проверяет повторение линии игроком, и в зависимости от этого начисляет бонусы/штрафы
		/// </summary>
		private void CheckLineRepeating() {
			if (repeatLine.IsLineRepeated) {
				timer.MultTime();  // удваиваем время за правильную линию
				score.Plus(timer.LevelTime, lineStartLength); // прибавляем набранные очки
				if (lineStartLength < lineMaxLength) lineStartLength++;
			} else timer.ReduceTime();    // убавляем время за неправильную линию
		}
	}
}
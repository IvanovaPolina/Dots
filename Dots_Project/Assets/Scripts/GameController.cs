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
		private AudioSource audioSource;

		[Tooltip("Изначальная длина линии при старте игры")]
		[Range(2, 9)]
		[SerializeField]
		private int lineStartLength = 3;
		private int lineMaxLength;      // максимальная длина линии
		[SerializeField]
		private TrailRenderer lineTrail;    // ссылка на объект, которым будем рисовать линию
		[Tooltip("Изначальная скорость отрисовки линии")]
		[Range(1f, 10f)]
		[SerializeField]
		private float drawSpeed = 2f;

		private bool isGameOver = false;

		private void Awake()
		{
			Timer.TimerIsOver += GameOver;
		}

		private void Start()
		{
			generateLine = gameObject.AddComponent<GenerateLine>();
			repeatLine = gameObject.AddComponent<RepeatLine>();
			timer = Timer.Instance;
			score = gameObject.AddComponent<Score>();
			lineMaxLength = Field.Instance.GameField.Length;
			drawSpeed = drawSpeed / 100f;
			lineTrail = Instantiate(lineTrail);
			lineTrail.enabled = false;
			audioSource = GetComponent<AudioSource>();
			StartCoroutine(Game());
		}

		private IEnumerator Game()
		{
			generateLine.NewLine(lineStartLength);
			while (!isGameOver)
			{
				if (repeatLine.IsLineRepeated)
				{
					yield return new WaitForSeconds(1f);    // задержка между уровнями
					generateLine.NewLine(lineStartLength);
				}
				yield return StartCoroutine(generateLine.DrawLine(lineTrail, drawSpeed, 1f));   // ждем отрисовки линии
				timer.StartCounting();
				repeatLine.Repeat(generateLine.Path);
				yield return new WaitUntil(() => repeatLine.IsFinishRepeating || isGameOver); // ждем, пока игрок повторит её
				timer.StopCounting();
				CheckLineRepeating();
				yield return null;
			}
		}

		/// <summary>
		/// Проверяет повторение линии игроком, и в зависимости от этого начисляет бонусы/штрафы
		/// </summary>
		private void CheckLineRepeating()
		{
			if (isGameOver) return;

			if (repeatLine.IsLineRepeated)
			{
				timer.MultTime();  // удваиваем время за правильную линию
				score.Plus(timer.LevelTime, lineStartLength); // прибавляем набранные очки
				if (lineStartLength < lineMaxLength) lineStartLength++;
			}
			else timer.ReduceTime();    // убавляем время за неправильную линию
		}

		private void GameOver()
		{
			isGameOver = true;
			audioSource.Play();
			TableData.Instance.SaveRecord(score.CurrentScore);
			GameMenu.Instance.DisplayPanel(GameMenu.Panel.Lose);
		}

		private void OnDestroy()
		{
			if (!isGameOver)
				TableData.Instance.SaveRecord(score.CurrentScore);
			Timer.TimerIsOver -= GameOver;
		}
	}
}
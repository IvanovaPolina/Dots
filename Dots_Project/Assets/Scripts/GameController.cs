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
		

		[SerializeField]
		[Range(2, 9)]
		private int lineStartLength = 3;    // изначальная длина линии
		private int lineEndLength;      // максимальная длина линии

		[SerializeField]
		private TrailRenderer trailPrefab;   // префаб будущей линии (компонент Trail Renderer)
		private TrailRenderer trail;    // ссылка на заспавненную линию
		[SerializeField]
		[Range(1f, 10f)]
		private float drawSpeed = 2f;   // скорость отрисовки линии

		private void Start() {
			generateLine = gameObject.AddComponent<GenerateLine>();
			repeatLine = gameObject.AddComponent<RepeatLine>();
			timer = Timer.Instance;
			score = Score.Instance;
			lineEndLength = Field.Instance.GameField.Length;
			drawSpeed = drawSpeed / 100f;
			trail = Instantiate(trailPrefab);
			trail.enabled = false;
			StartCoroutine(Game());
		}

		private IEnumerator Game() {
			generateLine.NewLine(lineStartLength);
			while (timer.RestTime > 0) {
				yield return new WaitForSeconds(1f);    // задержка между уровнями
				if (repeatLine.IsLineRepeated) generateLine.NewLine(lineStartLength);
				yield return StartCoroutine(generateLine.DrawLine(trail, drawSpeed, 1f));   // ждем отрисовки линии
				timer.StartCounting();
				yield return StartCoroutine(repeatLine.GetLine(generateLine.Path)); // ждем, пока игрок повторит её
				timer.StopCounting();
				CheckLineRepeating();
				ResetCellsState();
				yield return null;
			}
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
				if (lineStartLength < lineEndLength) lineStartLength++;
			} else {
				timer.ReduceTime();    // убавляем время за неправильную линию
			}
		}

		/// <summary>
		/// Сбрасывает состояние клеток до первоначального
		/// </summary>
		/// <param name="path">Список необходимых клеток</param>
		private void ResetCellsState() {
			foreach (var cell in generateLine.Path) {
				cell.Collider2D.enabled = true;
				cell.ResetColor();
			}
		}
	}
}
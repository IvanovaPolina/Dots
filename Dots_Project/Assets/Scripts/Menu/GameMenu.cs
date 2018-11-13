using Game.Menu.Records;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Menu
{
	/// <summary>
	/// Класс игрового меню
	/// </summary>
	public class GameMenu : MonoBehaviour
	{
		[SerializeField]
		private GameObject gamePanel, pausePanel, recordsPanel, losePanel;
		/// <summary>
		/// Доступ к данному классу
		/// </summary>
		public static GameMenu Instance { get; private set; }

		[SerializeField]
		private AudioSource audioSource;
		[SerializeField]
		private AudioClip click;

		/// <summary>
		/// Список панелей игрового меню
		/// </summary>
		public enum Panel
		{
			Game,
			Pause,
			Records,
			Lose
		}

		private void Awake() {
			// создаем единственный экземпляр класса, т.к. игровое меню у нас одно
			// а также есть необходимость обращаться к нему из других классов, не создавая экземпляр
			if (Instance) DestroyImmediate(this);
			else Instance = this;

			gameObject.AddComponent<TableData>();   // класс TableData загружает данные о поставленных рекордах
			Time.timeScale = 1f;
		}

		private void Start() {
			DisplayPanel(Panel.Game);
		}

		/// <summary>
		/// Отображает необходимую игровую панель
		/// </summary>
		public void DisplayPanel(Panel panel) {
			gamePanel.SetActive(panel == Panel.Game);
			pausePanel.SetActive(panel == Panel.Pause);
			recordsPanel.SetActive(panel == Panel.Records);
			losePanel.SetActive(panel == Panel.Lose);
		}

		/// <summary>
		/// Для кнопки паузы
		/// </summary>
		public void Pause() {
			audioSource.PlayOneShot(click);
			Time.timeScale = 0;
			DisplayPanel(Panel.Pause);
		}

		/// <summary>
		/// Для кнопки "Продолжить"
		/// </summary>
		public void Continue() {
			audioSource.PlayOneShot(click);
			Time.timeScale = 1f;
			DisplayPanel(Panel.Game);
		}

		/// <summary>
		/// Для кнопки "Начать заново"
		/// </summary>
		public void Restart() {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			Time.timeScale = 1f;
		}

		/// <summary>
		/// Для кнопки "Рекорды"
		/// </summary>
		public void Records() {
			audioSource.PlayOneShot(click);
			Time.timeScale = 0;
			DisplayPanel(Panel.Records);
		}

		/// <summary>
		/// Для кнопки "Назад" в панели "Рекорды"
		/// </summary>
		public void Records_Back() {
			audioSource.PlayOneShot(click);
			DisplayPanel(Panel.Pause);
		}

		/// <summary>
		/// Для кнопки "Меню"
		/// </summary>
		public void ToMainMenu() {
			SceneManager.LoadScene(0);
			Time.timeScale = 1f;
		}
	}
}
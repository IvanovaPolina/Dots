using Game.Menu.Records;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Menu
{
	/// <summary>
	/// Класс главного меню
	/// </summary>
	public class MainMenu : MonoBehaviour
	{
		[SerializeField]
		private GameObject menuPanel, recordsPanel;

		private void Awake() {
			gameObject.AddComponent<TableData>();   // класс TableData загружает данные о поставленных рекордах
		}

		private void Start() {
			DisplayPanel(menuPanel);
		}

		/// <summary>
		/// Отображает на экране необходимую игровую панель
		/// </summary>
		private void DisplayPanel(GameObject panel) {
			menuPanel.SetActive(menuPanel == panel);
			recordsPanel.SetActive(recordsPanel == panel);
		}

		/// <summary>
		/// Для кнопки "Начать игру"
		/// </summary>
		public void StartGame() {
			SceneManager.LoadScene(1);
		}

		/// <summary>
		/// Для кнопки "Рекорды"
		/// </summary>
		public void Records() {
			DisplayPanel(recordsPanel);
		}

		/// <summary>
		/// Для кнопки "Назад" в панели "Рекорды"
		/// </summary>
		public void Records_Back() {
			DisplayPanel(menuPanel);
		}

		/// <summary>
		/// Для кнопки "Выход"
		/// </summary>
		public void ExitGame() {
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
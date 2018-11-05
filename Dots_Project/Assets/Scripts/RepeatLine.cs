using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
	/// <summary>
	/// Класс содержит в себе логику повтора линии игроком
	/// </summary>
	public class RepeatLine : MonoBehaviour
	{
		/// <summary>
		/// Повторил ли игрок линию?
		/// </summary>
		public bool IsLineRepeated { get { return rest != null && rest.Count < 1; } }

		private List<Cell> rest;  // сколько точек по линии осталось провести
		private Cell mistake;       // клетка, которая не содержится в линии, и которую задел игрок
		private bool isRepeated;	// флаг, прерывающий повторение игроком линии в случае его ошибки

		private GraphicRaycaster graphicRaycaster;   // компонент на Canvas, отвечающий за raycast'ы
		private EventSystem eventSystem;     // без него UI элементы не будут интерактивными
		private PointerEventData eventData;     // регистратор события нажатия кнопки мыши

		private void Start() {
			graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
			eventSystem = FindObjectOfType<EventSystem>();
		}

		/// <summary>
		/// Перехватывает нажатие на элементах UI
		/// </summary>
		bool IsClickOnUI() {
			eventData = new PointerEventData(eventSystem);  // нажатие ЛКМ будем проверять на UI элементах
			eventData.position = Input.mousePosition;   // регистрируем в событии текущую позицию мыши
			List<RaycastResult> results = new List<RaycastResult>();    // для вывода результатов об объектах, в которые мы попали
			graphicRaycaster.Raycast(eventData, results);   // пускаем луч в соотв. с настройками eventData и заносим результаты в список
			if (results.Count > 0) return true; // если список содержит хоть один результат - мы попали в UI
			return false;
		}

		/// <summary>
		/// Проверяет, повторил ли игрок линию
		/// </summary>
		public IEnumerator GetLine(List<Cell> path) {
			rest = new List<Cell>(path.Count);
			rest.AddRange(path);
			isRepeated = false;
			mistake = null;
#if UNITY_ANDROID
			// ждем нажатия пальца по клетке, либо истечения времени
			yield return new WaitUntil(() => Input.touchCount > 0 && !IsClickOnUI() || Timer.Instance.RestTime <= 0);
			while (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Ended) {
#else
			// ждем нажатия ЛКМ по клетке, либо истечения времени
			yield return new WaitUntil(() => Input.GetMouseButtonDown(0) && !IsClickOnUI() || Timer.Instance.RestTime <= 0);
			while (Input.GetMouseButton(0)) {
#endif
				if (Timer.Instance.RestTime <= 0) break;
				if (rest.Count == 0) break;    // если повторили все клетки, выходим из цикла while и проверяем результаты
				isRepeated = IsRepeated();
				if (!isRepeated) break;  // если допустили ошибку в повторении, выходим из цикла
				yield return null;  // пока нажата ЛКМ - делаем проверки в каждом кадре
			}
			// останавливаемся, когда игрок отпускает ЛКМ, либо линию повторили правильно
			yield return new WaitForSeconds(0.5f);
			if (!isRepeated && mistake != null) mistake.ResetColor();
		}

		/// <summary>
		/// Проверяет, повторил ли игрок линию и не допустил ли ошибку
		/// </summary>
		private bool IsRepeated() {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // пускаем луч через экран
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
			if (hit) {      // если задели коллайдер
				if (hit.collider.gameObject == rest[0].gameObject) { // и этот коллайдер принадлежит первой клетке линии
					rest[0].ChangeColor(Color.green); // меняем цвет этой клетки
					rest.Remove(rest[0]);   // удаляем из списка эту клетку, чтобы на её место встала следующая
					hit.collider.enabled = false;   // исключаем возможность попадания в тот же коллайдер
				} else {
					mistake = hit.collider.GetComponent<Cell>();
					mistake.ChangeColor(Color.red);
					return false;
				}
			}
			return true;
		}
	}
}
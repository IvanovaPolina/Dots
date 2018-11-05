using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	/// <summary>
	/// Класс содержит в себе логику повтора линии игроком
	/// </summary>
	public class RepeatLine : MonoBehaviour
	{
		private List<Cell> rest;  // сколько точек по линии осталось провести
		private Cell mistake;       // клетка, которая не содержится в линии, и которую задел игрок
		private bool isRepeated;

		/// <summary>
		/// Повторил ли игрок линию?
		/// </summary>
		public bool IsLineRepeated { get { return rest != null && rest.Count < 1; } }

		/// <summary>
		/// Проверяет, повторил ли игрок линию
		/// </summary>
		public IEnumerator GetLine(List<Cell> path) {
			rest = new List<Cell>(path.Count);
			rest.AddRange(path);
			isRepeated = false;
			mistake = null;
			yield return new WaitUntil(() => Input.GetMouseButton(0) || Timer.Instance.RestTime <= 0);	// ждем нажатия ЛКМ
			while (Input.GetMouseButton(0)) {
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
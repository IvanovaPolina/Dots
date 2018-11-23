using UnityEngine;

namespace Game
{
	/// <summary>
	/// Класс отвечает за поддержание ввода и с телефона, и с ПК
	/// </summary>
	public static class CrossplatformInput
	{
		/// <summary>
		/// Возвращает true, если было зарегистрировано нажатие ЛКМ или касание экрана пальцем
		/// </summary>
		public static bool IsPressedDown { get { return Input.touchCount > 0 || Input.GetMouseButtonDown(0); } }

		/// <summary>
		/// Возвращает true, если в данный момент продолжается нажатие на экран
		/// </summary>
		public static bool IsPressed {
			get {
				bool isTouchMoved = Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary);
				return Input.GetMouseButton(0) || isTouchMoved;
			}
		}

		/// <summary>
		/// Возвращает текущую позицию мыши/тача
		/// </summary>
		public static Vector3 CurrentPosition { get { return Input.mousePosition; } }
	}
}
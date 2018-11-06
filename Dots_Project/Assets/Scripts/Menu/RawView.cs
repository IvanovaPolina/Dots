using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Menu.Records
{
	/// <summary>
	/// Класс отвечает за отображение строки с рекордом в текстовом формате
	/// </summary>
	public class RawView : MonoBehaviour
	{
		[SerializeField]
		private Text number;
		[SerializeField]
		private Text dateTime;
		[SerializeField]
		private Text score;

		/// <summary>
		/// Порядковый номер рекорда (1 место, 2 место и т.п.)
		/// </summary>
		public int Number
		{
			get {
				int result;
				if (int.TryParse(number.text, out result))
					return result;
				else return -1;
			}
			set {
				if (value > 0) number.text = value.ToString();
				else number.text = string.Empty;
			}
		}

		/// <summary>
		/// Дата и время поставленного рекорда
		/// </summary>
		public DateTime DateTime
		{
			get {
				DateTime result;
				if (DateTime.TryParse(dateTime.text, out result))
					return result;
				else return DateTime.MinValue;
			}
			set { dateTime.text = value.ToString("dd.MM.yyyy HH:mm"); }
		}

		/// <summary>
		/// Рекордный счет
		/// </summary>
		public float Score
		{
			get {
				float result;
				if (float.TryParse(score.text, out result))
					return result;
				else return -1;
			}
			set {
				if (value > 0) score.text = value.ToString();
				else score.text = string.Empty;
			}
		}
	}
}
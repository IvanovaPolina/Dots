using System;

namespace Game.Menu.Records
{
	/// <summary>
	/// Класс, содержащий данные строки из таблицы рекордов
	/// </summary>
	public class Raw
	{
		private int number;
		private DateTime dateTime;
		private float score;

		public Raw() { }

		/// <summary>
		/// Создает новую строку с рекордом
		/// </summary>
		/// <param name="number">Порядковый номер рекорда (1 место, 2 место и т.п.)</param>
		/// <param name="dateTime">Дата и время поставленного рекорда</param>
		/// <param name="score">Рекордный счет</param>
		public Raw(int number, DateTime dateTime, float score) {
			this.number = number;
			this.dateTime = dateTime;
			this.score = score;
		}

		public int Number { get { return number; } set { number = value; } }
		public DateTime DateTime { get { return dateTime; } set { dateTime = value; } }
		public float Score { get { return score; } set { score = value; } }
	}
}
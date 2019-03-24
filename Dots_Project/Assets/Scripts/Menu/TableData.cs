using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Menu.Records
{
	/// <summary>
	/// Класс отвечает за работу с данными из таблицы рекордов
	/// </summary>
	public class TableData : MonoBehaviour
	{
		/// <summary>
		/// Доступ к данному классу
		/// </summary>
		public static TableData Instance { get; private set; }
		/// <summary>
		/// Событие передает текущую таблицу рекордов
		/// </summary>
		public static event Action<List<Raw>> RecordsChanged;

		private RecordsData recordsData;
		private List<Raw> records;      // ранее установленные рекорды
		private int maxCount = 5;	// максимальное количество рекордов для сохранения

		private void Awake() {
			// создаем единственный экземпляр данного класса
			if (Instance) DestroyImmediate(this);
			else Instance = this;
		}

		private void Start() {
			recordsData = new RecordsData();
			records = recordsData.Load();   // при старте игры сразу подгружаем все рекорды, если таковые имеются
			if (RecordsChanged != null) RecordsChanged.Invoke(records);
		}

		/// <summary>
		/// Определяет, был ли поставлен новый рекорд. Если да - сохраняет его
		/// </summary>
		/// <param name="score">Набранный счет</param>
		public void SaveRecord(float score) {
			if (score <= 0) return;

			// если рекордов больше максимального (например, мы уменьшали переменную maxCount), убираем лишние рекорды
			if (records.Count > maxCount)
				records.RemoveRange(maxCount, records.Count - maxCount);

			// проверяем, лучше ли этот score, чем самый последний. Если нет - return
			if (records.Count == maxCount) {        // если таблица рекордов уже полностью заполнена
				Raw lastRaw = records[records.Count - 1];   // и счет последнего рекорда >= набранного счета
				if (lastRaw.Score >= score) return;     // оставляем рекорды без изменений
				records.Remove(lastRaw);    // иначе - удаляем последний рекорд, чтобы добавить в список новый
			}
			records.Add(new Raw(0, DateTime.Now, score));   // вставляем в таблицу новый рекорд
			records = records.OrderByDescending(e => e.Score).ToList(); // сортируем рекорды по убыванию Score
			// при этом каждому Number'у присваиваем значение занятого места (i + 1)
			for (int i = 0; i < records.Count; i++)
				records[i].Number = i + 1;

			if (RecordsChanged != null) RecordsChanged.Invoke(records);
			recordsData.Save(records);
		}
	}
}
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Game.Menu.Records
{
	/// <summary>
	/// Класс отвечает за загрузку рекордов из файла и их сохранение в файл
	/// </summary>
	public class RecordsData
	{
		private string path; // куда сохраняем и откуда загружаем сохранения
		private string filename = "saves.dat";

		public RecordsData() {
			path = Path.Combine(Application.dataPath, filename);
		}
		
		/// <summary>
		/// Сохраняет все рекорды
		/// </summary>
		public void Save(List<Raw> records) {
			if (records == null || records.Count == 0) {
				Debug.LogWarning("Рекорды для сохранения не найдены, т.к. передана ссылка на пустой объект");
				return;
			}
			using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate))) {
				for (int i = 0; i < records.Count; i++) {
					bw.Write(records[i].Number);
					bw.Write(records[i].DateTime.ToString());
					bw.Write(records[i].Score);
				}
			}
		}

		/// <summary>
		/// Загружает все рекорды
		/// </summary>
		public List<Raw> Load() {
			List<Raw> list = new List<Raw>();
			if (!File.Exists(path)) {
				Debug.Log("Файл загрузки не найден");
				return list;
			}
			using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open))) {
				try {
					while (br.PeekChar() > -1) { // пока в файле есть что прочесть - считываем данные и записываем
						list.Add(new Raw() {
							Number = br.ReadInt32(),
							DateTime = DateTime.Parse(br.ReadString()),
							Score = br.ReadSingle()
						});
					}
				}
				catch (Exception e) {	// оборачиваем в try catch, т.к. используем DateTime.Parse вместо TryParse
					Debug.LogWarning(e.Message);
				}
			}
			return list;
		}
	}
}
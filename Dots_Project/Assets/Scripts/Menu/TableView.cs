using System.Collections.Generic;
using UnityEngine;

namespace Game.Menu.Records
{
	/// <summary>
	/// Класс, содержащий логику работы с таблицей рекордов
	/// </summary>
	public class TableView : MonoBehaviour
	{
		[SerializeField]
		private RawView rawPrefab;

		private List<RawView> table;

		private void Awake() {
			TableData.OnRecordsChanged += DisplayTable;
			table = new List<RawView>();
		}
		
		private void OnDestroy() {
			TableData.OnRecordsChanged -= DisplayTable;
		}

		/// <summary>
		/// Отображает таблицу рекордов на экране
		/// </summary>
		public void DisplayTable(List<Raw> records) {
			if (records == null || records.Count == 0) return;
			for (int i = 0; i < records.Count; i++) {
				if(table.Count <= i)
					table.Add(Instantiate(rawPrefab, transform));
				table[i].Number = records[i].Number;
				table[i].DateTime = records[i].DateTime;
				table[i].Score = records[i].Score;
			}
			//RectTransform rectTransform = GetComponent<RectTransform>();
			//rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, 0);
		}
	}
}
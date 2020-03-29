
using System.Collections.Generic;

namespace prog {
	/// <summary>
	/// Класс описывающий дополнительные методы для работы со списками. 
	/// </summary>
	internal static class VectorHelper {
		/// <summary>
		/// Метод заполняющий список заданым значением.
		/// </summary>
		/// <param name="list"> Список, который надо заполнить. </param>
		/// <param name="newSize"> Размер списка. </param>
		/// <param name="value"> Значение,если не задано, то берётся значение по умолчанию для типа. </param>
		public static void Resize<T>(this List<T> list, int newSize, T value = default(T)) {
			if (list.Count > newSize)
				list.RemoveRange(newSize, list.Count - newSize);
			else if (list.Count < newSize) {
				for (int i = list.Count; i < newSize; i++) {
					list.Add(value);
				}
			}
		}

		/// <summary>
		/// Метод инициализирует вектор заданым значением. 
		/// </summary>
		/// <param name="size"> Размер вектора. </param>
		/// <param name="value"> Значение, которыми будет заполнен вектор. </param>
		/// <returns> Возвращает проинициализированный вектор. </returns>
		public static List<T> InitializedList<T>(int size, T value) {
			List<T> temp = new List<T>();
			for (int count = 1; count <= size; count++) {
				temp.Add(value);
			}

			return temp;
		}

		/// <summary>
		/// Метод заполняющий матрицу заданым значением.
		/// </summary>
		/// <param name="outerSize"> Количество строк. </param>
		/// <param name="innerSize"> Количество столбцов. </param>
		/// <param name="value"> Значение, которым заполнять. </param>
		/// <returns> Возварщает заполненныую матрицу. </returns>
		public static List<List<T>> NestedList<T>(int outerSize, int innerSize, T value) {
			List<List<T>> temp = new List<List<T>>();
			for (int count = 1; count <= outerSize; count++) {
				temp.Add(InitializedList(innerSize, value));
			}

			return temp;
		}
	}
}
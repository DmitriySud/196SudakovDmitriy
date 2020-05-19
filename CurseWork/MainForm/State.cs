using System;
using System.Collections.Generic;
using System.Windows;

namespace MainForm {
	/// <summary type="" dos="">
	/// Структура отображающая расположение вершин графа на холсте и его информативность.
	/// </summary>
	struct State : IComparable<State> {
		/// <summary type="int" dos="public">
		/// Количество пересечений линий рёбер. 
		/// </summary>
		public int lineInt;
		/// <summary type="int" dos="public">
		/// Количество пересечений рёбер с вершинами.
		/// </summary>
		public int circleInt;
		/// <summary type="double" dos="public">
		/// Суммарная длина рёбер. 
		/// </summary>
		public double sumDist;
		/// <summary type="double" dos="public">
		/// Идельная длина рёбер.
		/// </summary>
		public static double ideal;

		/// <summary type="List{Point}" dos="public">
		/// Список координат вершин.
		/// </summary>
		public List<Point> lstVertex;

		/// <summary type="" dos="public">
		/// Конструктор для инициализации основных параметров.
		/// </summary>
		/// <param name="lineInt"> Количество пересечений линий рёбер. </param>
		/// <param name="circleInt"> Количество пересечений рёбер с вершинами. </param>
		/// <param name="sumDist"> Суммарная длина рёбер. </param>
		public State(int lineInt, int circleInt, double sumDist) {
			this.lineInt = lineInt;
			this.circleInt = circleInt;
			this.sumDist = sumDist;
			ideal = 0;
			lstVertex = new List<Point>();
		}

		/// <summary type="int" dos="public">
		/// Метод сравнения информативности 2-х сосотяний.
		/// </summary>
		/// <param name="obj"> Объект, сравниваемый с текущим. </param>
		/// <returns> Возвращает 0, если объеты равны, 1 если текущий объект больше,-1 если текущий объект меньше. </returns>
		public int CompareTo(State obj) {
			int r1 = circleInt.CompareTo(obj.circleInt);
			if (r1 == 0) {
				int r2 = lineInt.CompareTo(obj.lineInt);
				if (r2 == 0)
					return Math.Abs(sumDist - ideal).CompareTo(Math.Abs(obj.sumDist - ideal));
				return r2;
			}
			return r1;
		}
	}
}

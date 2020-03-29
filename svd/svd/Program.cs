using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace prog {

	class program {
		public static int test = 10;

		static void Main() {
			do {
				var m = new Matrix(0, 0);
				m.ReadMatrix("..\\..\\input.txt");
				m.Print();
				var l = new List<List<double>>();
				var r = new List<List<double>>();
				var v = new List<double>();
				GlobalMembers.svd(ref m, ref l, ref r, ref v);
				Console.WriteLine("left singular vectors : ");
				Print(l);
				Console.WriteLine("right singular vectors : ");
				Print(r);
				Console.WriteLine("singular values : ");
				Print(v);
				Console.WriteLine("matr : ");
				var res = new Matrix(m.get_rows_number, m.get_columns_number);
				for (int i = 0; i < v.Count; i++) {
					var a = new Matrix(l[i]);
					a.transpose();
					var b = new Matrix(r[i]);
					res = res + v[i] * (a * b);
				}
				Print(res);
				Console.WriteLine("Enter to repeat");
			} while (Console.ReadKey(true).Key == ConsoleKey.Enter);
		}

		/// <summary>
		/// Метод вывода двумерного списка в консоль.
		/// </summary>
		public static void Print(List<List<double>> ls) {
			for (int i = 0; i < ls.Count; ++i, Console.WriteLine())
				for (int j = 0; j < ls[i].Count; ++j)
					Console.Write("{0:f3} ", ls[i][j]);
			Console.WriteLine();
		}

		/// <summary>
		/// Метод выводв матрицы в консоль.
		/// </summary>
		public static void Print(Matrix m) {
			m.Print();
		}
		/// <summary>
		/// Метод вывода списка в консоль.
		/// </summary>
		/// <param name="ls"></param>
		public static void Print(List<double> ls) {
			for (int j = 0; j < ls.Count; ++j)
				Console.Write("{0:f3} ", ls[j]);
			Console.WriteLine();
			Console.WriteLine();
		}

	}
}

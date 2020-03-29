using System;
using System.Collections.Generic;
using System.IO;

namespace prog {
	/// <summary>
	/// Класс описывающий 2хмерную матрицу размера n*m.
	/// </summary>
	public class Matrix {
		/// <summary>
		/// Количество строк.
		/// </summary>
		private int n;
		/// <summary>
		/// Количество столбцов.
		/// </summary>
		private int m;
		/// <summary>
		/// 2-х мерный список, хранящий значения матрицы.
		/// </summary>
		private List<List<double>> data = new List<List<double>>();

		/// <summary>
		/// Конструктор для создания матрицы из нулей.
		/// </summary>
		/// <param name="rows_number"> Количество строк.</param>
		/// <param name="columns_number"> Количество столбцов. </param>
		public Matrix(int rows_number, int columns_number) {
			this.n = rows_number;
			this.m = columns_number;
			this.data = VectorHelper.NestedList(rows_number, columns_number, 0.0);
		}
		/// <summary>
		/// Конструктор создающий матрицу по двумерному списку.
		/// </summary>
		/// <param name="lst"> Список значений. </param>
		public Matrix(List<List<double>> lst) {
			this.n = lst.Count;
			this.m = lst[0].Count;
			this.data = lst;
		}
		/// <summary>
		/// Конструктор, создающий матрицу состоящую из одного вескора строки.
		/// </summary>
		/// <param name="lst"> Значения вектора. </param>
		public Matrix(List<double> lst) {
			n = 1;
			m = lst.Count;
			data.Add(new List<double>());
			foreach (var item in lst) {
				data[0].Add(item);
			}
		}

		/// <summary>
		/// Метод транспонирования матрицы.
		/// </summary>
		public void transpose() {

			List<List<double>> result = VectorHelper.NestedList(m, n, 0.0);

			for (int i = 0; i < n; ++i) {
				for (int j = 0; j < m; ++j) {
					result[j][i] = data[i][j];
				}
			}

			data = result;
			Swap(ref n, ref m);
		}

		private void Swap<T>(ref T n, ref T m) {
			T temp = n;
			n = m;
			m = temp;
		}

		
		/// <summary>
		/// Метод находит Евклидово расстояние между векторам.
		/// </summary>
		/// <param name="a"> Первый вектор. </param>
		/// <param name="b"> Второй вектор. </param>
		/// <returns> Возвращает целое число -  вычестленное расстояние. </returns>
		public double distance_to_aXb(List<double> a, List<double> b) {

			double result = 0;

			for (int i = 0; i < a.Count; ++i) {
				for (int j = 0; j < b.Count; ++j) {
					result += Math.Pow((a[i] * b[j] - data[i][j]), 2);
				}
			}

			return result;
		}

		/// <summary>
		/// Вычитает из данной матрицы, матрицу a^(T)*b.
		/// </summary>
		/// <param name="a"> Вектор столбец. </param>
		/// <param name="b"> вектор строка. </param>
		public void substract_aXb(List<double> a, List<double> b) {

			for (int i = 0; i < n; ++i) {
				for (int j = 0; j < m; ++j) {
					data[i][j] -= a[i] * b[j];
				}
			}
		}

		/// <summary>
		/// Индексатор для обращения к значениям матрицы, минуя поле data.
		/// </summary>
		/// <param name="i"> Номер строки. </param>
		/// <returns> Возвращает заданую строку матрицы. </returns>
		public List<double> this[int i] {
			get {
				return data[i];
			}
		}

		/// <summary>
		/// Метод для считывания матрицы из заданного файла.
		/// </summary>
		/// <param name="path"> Путь к файлу, в котором лежит матрица. </param>
		public void ReadMatrix(string path) {
			StreamReader file = new StreamReader(path);
			string s = file.ReadToEnd();
			s = s.Replace('.', ',');
			file.Close();
			string[] row = s.Split('\n');

			n = int.Parse(row[0].Split(' ')[0]);
			m = int.Parse(row[0].Split(' ')[1]);
			for (int i = 0; i < row.Length-1; i++) 
				row[i] = row[i + 1];
			Array.Resize(ref row, row.Length - 1);

			data = new List<List<double>>();
			for (int i = 0; i < n; i++) {
				data.Add(new List<double>());
				string[] col = row[i].Split(' ');
				for (int j = 0; j < m; j++) {
					data[i].Add(Convert.ToDouble(col[j]));
					Console.Write(" {0}", data[i][j]);
				}
				Console.WriteLine();
			}
		}

		/// <summary>
		/// Метод читает матрицу из бинарного файла.
		/// </summary>
		/// <param name="path"> Путь к файлу. </param>
		public void ReadFromBin(string path) {
			using (var file = new BinaryReader(new FileStream(path, FileMode.Open))) {
				n = file.ReadInt32();
				m = file.ReadInt32();

				data = new List<List<double>>();
				for (int i = 0; i < n; i++) {
					data.Add(new List<double>());
					for (int j = 0; j < m; j++) {
						data[i].Add(file.ReadDouble());
					}
				}
			}
		}

		/// <summary>
		/// Метод печатаем матрицу в консоль.
		/// </summary>
		public void Print() {
			for (int i = 0; i < n; ++i, Console.WriteLine())
				for (int j = 0; j < m; ++j)
					Console.Write("{0:f3} ", data[i][j]);
			Console.WriteLine();
		}


		/// <summary>
		/// Свойство для обращения к количеству строк матрицы.
		/// </summary>
		public int get_rows_number {
			get => n;
		}

		/// <summary>
		/// Свойство для обращения к количеству столбцов матрицы.
		/// </summary>
		public int get_columns_number {
			get => m;
		}

		/// <summary>
		/// Операция перемножения 2х матриц.
		/// </summary>
		public static Matrix operator *(Matrix m1, Matrix m2) {
			if (m1.get_columns_number != m2.get_rows_number) throw new Exception("Матрицы нельзя перемножить");
			List<List<double>> r = new List<List<double>>();

			for (int i = 0; i < m1.get_rows_number; i++) {
				r.Add(new List<double>());
				for (int j = 0; j < m2.get_columns_number; j++) {
					r[i].Add(0);
					for (int k = 0; k < m2.get_rows_number; k++) {
						r[i][j] += m1[i][k] * m2[k][j];
					}
				}
			}
			return new Matrix(r);
		}
		/// <summary>
		/// Операция умножения матрицы на число. 
		/// </summary>
		public static Matrix operator *(double m, Matrix m2) {
			List<List<double>> r = new List<List<double>>();

			for (int i = 0; i < m2.get_rows_number; i++) {
				r.Add(new List<double>());
				for (int j = 0; j < m2.get_columns_number; j++) {
					r[i].Add(m2[i][j]*m);
				}
			}
			return new Matrix(r);
		}
		/// <summary>
		/// Операция сложения 2х матриц.
		/// </summary>
		public static Matrix operator+ (Matrix m1, Matrix m2) {
			if (m1.get_columns_number != m2.get_columns_number || m1.get_rows_number != m2.get_rows_number)
				throw new ArithmeticException("Матрицы невозможно перемножить");
			var res = new List<List<double>>();
			for (int i = 0; i < m1.get_rows_number; i++) {
				res.Add(new List<double>());
				for (int j = 0; j < m1.get_columns_number; j++) {
					res[i].Add( m1[i][j] + m2[i][j]);
				}
			}
			return new Matrix(res);
		}
	}
}
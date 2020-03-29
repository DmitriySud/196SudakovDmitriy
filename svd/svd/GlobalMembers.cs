using System;
using System.Collections.Generic;

namespace prog {
	/// <summary>
	/// Класс с описанием основных математических методов.
	/// </summary>
	public static class GlobalMembers {
		/// <summary>
		/// Генератор случайных чисел.
		/// </summary>
		private static Random rnd = new Random();

		/// <summary>
		/// Метод высчитывающий приближение правого сингулярого вектора из левого.
		/// </summary>
		/// <param name="a"> Левый вектор. </param>
		/// <param name="b"> Правый вектор. </param>
		/// <param name="mtrx"> Матрица для которой ищется разложение. </param>
		public static void compute_b(ref List<double> a, ref List<double> b, Matrix mtrx) {
			double scalar;
			double length;
			for (int i = 0; i < mtrx.get_columns_number; ++i) {
				scalar = 0;
				length = 0;
				for (int k = 0; k < mtrx.get_rows_number; ++k) {

					scalar += mtrx[k][i] * a[k];
					length += a[k] * a[k];
				}
				b[i] = scalar / length;
			}
		}

		/// <summary>
		/// Метод высчитывающий приближение левого сингулярого вектора из правого.
		/// </summary>
		/// <param name="a"> Левый вектор. </param>
		/// <param name="b"> Правый вектор. </param>
		/// <param name="mtrx"> Матрица для которой ищется разложение. </param>
		public static void compute_a(ref List<double> a, ref List<double> b, Matrix mtrx) {
			double scalar;
			double length;
			for (int i = 0; i < mtrx.get_rows_number; ++i) {
				scalar = 0;
				length = 0;
				for (int k = 0; k < mtrx.get_columns_number; ++k) {
					scalar += mtrx[i][k] * b[k];
					length += b[k] * b[k];
				}
				a[i] = scalar / length;
			}
		}

		/// <summary>
		/// Метод вычисляет ненормализованные сингулярные вектора.
		/// </summary>
		/// <param name="left_singular_vectors"> Список, где будут левые сингулярные вектора. </param>
		/// <param name="right_singular_vectors"> Список, где будут правые сингулярные вектора. </param>
		/// <param name="mtrx"> Матрица, для которой вычисляются сингулярные вектора. </param>
		public static void compute_singular_vectors(ref List<List<double>> left_singular_vectors, ref List<List<double>> right_singular_vectors, ref Matrix mtrx) {
			// int n = mtrx.get_rows_number(), m = mtrx.get_columns_number();
			for (int l = 0; l < mtrx.get_rows_number; ++l) {
				List<double> a = new List<double>();
				a.Resize(mtrx.get_rows_number);
				List<double> b = new List<double>();
				b.Resize(mtrx.get_columns_number);
				fill_random_vector(a);
				normalize_vector(a);
				double distance_before = 1.0;
				double distance_after = 0.1;
				while ((distance_before - distance_after) / distance_after > 0.0001) {
					compute_b(ref a, ref b, mtrx);
					distance_before = mtrx.distance_to_aXb(a, b);
					compute_a(ref a, ref b, mtrx);
					distance_after = mtrx.distance_to_aXb(a, b);
				}
				left_singular_vectors.Add(a);
				right_singular_vectors.Add(b);
				mtrx.substract_aXb(a, b);
			}
		}

		/// <summary>
		/// Метод нормализующий сингулярные вектора и вычисляющий сингулярные значения.
		/// </summary>
		/// <param name="left_singular_vectors"> Массив левых сингулярных векторов. </param>
		/// <param name="right_singular_vectors"> Массив правых сингулярных векторов. </param>
		/// <param name="singular_values"> Список, где будут лежать сингулярные значения. </param>
		/// <param name="n"> Количество строк матрицы. </param>
		/// <param name="m"> Количество столбцов матрицы. </param>
		public static void compute_singular_values(ref List<List<double>> left_singular_vectors, ref List<List<double>> right_singular_vectors, ref List<double> singular_values, int n, int m) {

			List<double> a = new List<double>();
			List<double> b = new List<double>();
			double new_value;
			double a_length;
			double b_length;

			for (int i = 0; i < n; ++i) {

				a = new List<double>(left_singular_vectors[i]);

				b = new List<double>(right_singular_vectors[i]);

				a_length = Math.Sqrt(scalar_composition(a, a));
				b_length = Math.Sqrt(scalar_composition(b, b));

				new_value = a_length * b_length;

				for (int k = 0; k < n; ++k) {
					left_singular_vectors[i][k] = left_singular_vectors[i][k] / a_length;
				}
				for (int k = 0; k < m; ++k) {
					right_singular_vectors[i][k] = right_singular_vectors[i][k] / b_length;
				}

				singular_values.Add(new_value);
			}
		}

		/// <summary>
		/// Метод выполняет SVD-разложение. 
		/// </summary>
		/// <param name="mtrx"> Раскладываемая матрица. </param>
		/// <param name="left_singular_vectors"> Список, где будут левые сингулярные вектора. </param>
		/// <param name="right_singular_vectors"> Список где будут правые сингулярные вектора. </param>
		/// <param name="singular_values"> Список, где будут сингулярные числа. </param>
		public static void svd(ref Matrix mtrx, ref List<List<double>> left_singular_vectors, ref List<List<double>> right_singular_vectors, ref List<double> singular_values) {
			bool is_transposed = false;
			if (mtrx.get_rows_number > mtrx.get_columns_number) {
				mtrx.transpose(); // now n <= m (matrix is n*m)
				is_transposed = true;
			}
			compute_singular_vectors(ref left_singular_vectors, ref right_singular_vectors, ref mtrx);
			compute_singular_values(ref left_singular_vectors, ref right_singular_vectors, ref singular_values, mtrx.get_rows_number, mtrx.get_columns_number);
			if (is_transposed) {
				Swap(ref right_singular_vectors, ref left_singular_vectors);
			}
		}

		/// <summary>
		/// Метод нормализует вектор.
		/// </summary>
		/// <param name="v"> Вектор заданый списком значений. </param>
		public static void normalize_vector(List<double> v) {

			double sum = 0;

			foreach (double elem in v) {
				sum += elem * elem;
			}
			sum = Math.Sqrt(sum);
			for (int i = 0; i < v.Count; i++)
				v[i] /= sum;
		}
		/// <summary>
		/// Метод заполняет вектор случайными числами. 
		/// </summary>
		/// <param name="v"> Вектор, который необзодимо звполнить. </param>
		public static void fill_random_vector(List<double> v) {

			for (int i = 0; i < v.Count; i++) {
				v[i] = rnd.Next() % 1000; 
			}
		}

		/// <summary>
		/// Метод осуществляющий скалярное произведение векторов. 
		/// </summary>
		/// <param name="a"> Первый вектор. </param>
		/// <param name="b"> Второй вектор. </param>
		/// <returns> Возвращает целое число, равное скалярному произведению векторов. </returns>
		public static double scalar_composition(List<double> a, List<double> b) {

			double result = 0;

			for (int i = 0; i < a.Count; ++i) {
				result += a[i] * b[i];
			}

			return result;
		}

		private static void Swap<T>(ref T n, ref T m) {
			T temp = n;
			n = m;
			m = temp;
		}


	}

}

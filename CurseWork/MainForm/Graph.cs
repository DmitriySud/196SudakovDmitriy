using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.Serialization.Formatters.Binary;

namespace MainForm {
	/// <summary type="" dos="">
	/// Класс описывающий граф для моделирования алгоритмов и визуализации.
	/// </summary>
	[Serializable]
	class Graph : DrawableObject {
		/// <summary type="double" dos="private">
		/// Ширина окна в котором отрисовывается граф.
		/// </summary>
		double width;
		/// <summary type="double" dos="private">
		/// Высота окна в котором отрисовывается граф.
		/// </summary>
		double height;
		/// <summary type="int" dos="private">
		/// Количество вершин графа.
		/// </summary>
		int n;

		/// <summary type="List{Vertex}" dos="private">
		/// Список вершин графа.
		/// </summary>
		List<Vertex> vertices;
		/// <summary type="List{Edge}" dos="private">
		/// Список рёбер графа.
		/// </summary>
		private List<Edge> edges;
		/// <summary type="Random" dos="private">
		/// Генератор случайных чисел.
		/// </summary>
		static Random rnd = new Random(10);

		/// <summary type="" dos="public">
		/// Конструктор для создания графа из списка инцидентности.
		/// </summary>
		/// <param name="incList"> Список инцидентности. </param>
		/// <param name="width"> Ширина окна, на котором рисуется граф. </param>
		/// <param name="height"> Высота окна, на котором рисуется граф. </param>
		/// <param name="canvas"> Холст, на котором отображается граф. </param>
		public Graph(List<List<int>> incList, double width, double height, Canvas canvas) : base(canvas) {
			Width = width;
			Height = height;
			n = incList.Count;

			CreateVertexList(incList);

			PositionedVertexes();
		}

		/// <summary type="void" dos="private">
		/// Метод выбора координат вершин.
		/// </summary>
		private void PositionedVertexes() {
			var minState = new State(int.MaxValue, int.MaxValue, double.MinValue);
			State.ideal = 2 * ((Height + Width) / 2) / Edges.Count;
			int iterationAmt = 10000;

			for (int i = 0; i < iterationAmt; i++) {
				ThrowVertices();
				State curState = CalcState();
				if (minState.CompareTo(curState) > 0)
					minState = curState;
			}
			for (int i = 0; i < n; i++) {
				Vertices[i].Position = minState.lstVertex[i];
			}
		}

		/// <summary type="State" dos="private">
		/// Посчитать характеристики структуры State для текущего расположения вершин.
		/// </summary>
		/// <returns> Возвращает структуру с записанными характеристиками и списком координат вершин. </returns>
		private State CalcState() {
			var result = new State(0, 0, 0);
			double rad = Math.Sqrt(Height * Width / n / Math.PI);

			for (int i = 0; i < Edges.Count; i++) {
				for (int j = 0; j < i; j++) {
					var e1 = Edges[i];
					var e2 = Edges[j];
					if (e1.VertexA == e2.VertexA || e1.VertexA == e2.VertexB
						|| e1.VertexB == e2.VertexA || e1.VertexB == e2.VertexB)
						continue;

					result.lineInt += IntersectLine(Edges[i].VertexA.Position, Edges[i].VertexB.Position,
						Edges[j].VertexA.Position, Edges[j].VertexB.Position) ? 1 : 0;
				}

				for (int j = 0; j < n; j++) {
					if (Vertices[j] == Edges[i].VertexA || Vertices[j] == Edges[i].VertexB)
						continue;
					result.circleInt += IntersectCircle(Edges[i].VertexA.Position, Edges[i].VertexB.Position,
						Vertices[j].Position, rad / 2) ? 1 : 0;
				}
				result.sumDist += Distance(Edges[i].VertexA.Position, Edges[i].VertexB.Position);
			}
			foreach (var item in Vertices) {
				result.lstVertex.Add(item.Position);
			}
			return result;
		}

		/// <summary type="void" dos="private">
		/// Метод для распределения вершинам случайных координат.
		/// </summary>
		private void ThrowVertices() {
			double rad = Math.Sqrt(Height * Width / n / Math.PI);

			for (int i = 0; i < n; i++) {
				var cur = Vertices[i];
				var pos = cur.Position;
				bool norm;

				do {
					pos.X = rnd.Next((int)cur.Size, (int)(Width - cur.Size));
					pos.Y = rnd.Next((int)cur.Size, (int)(Height - cur.Size));

					norm = true;
					int j = 0;
					while (norm && j < i) {
						norm = Distance(Vertices[j].Position, pos) > rad;
						j++;
					}
				} while (!norm);
				cur.Position = pos;
			}

		}

		/// <summary type="double" dos="private">
		/// Метод назождения расстояния между точками.
		/// </summary>
		/// <param name="a"> Первая точка. </param>
		/// <param name="b"> Вторая точка. </param>
		/// <returns> Расстояние вычисленное по теореме пифагора. </returns>
		private double Distance(Point a, Point b) => a.Distance(b);

		/// <summary type="double" dos="private">
		/// Метод вычисление направленной площади треугольника.
		/// </summary>
		/// <param name="a"> Первая вершина. </param>
		/// <param name="b"> Вторая вершина. </param>
		/// <param name="c"> Третья вершина. </param>
		/// <returns> </returns>
		private double Area(Point a, Point b, Point c) =>
			(b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

		/// <summary type="bool" dos="private">
		/// Вспомогательный метод для вычисления пересечения отрезков.
		/// </summary>
		/// <param name="a"> Координата X первой точки первого отрезка. </param>
		/// <param name="b"> Координата X второй точки первого отрезка. </param>
		/// <param name="c"> Координата X первой точки второй отрезка. </param>
		/// <param name="d"> Координата X второй точки второй отрезка. </param>
		/// <returns> Возвращает вычисленную вспомогательную характеристику. </returns>
		private bool Intersect1(double a, double b, double c, double d) {
			if (a > b) Swap(ref a, ref b);
			if (c > d) Swap(ref c, ref d);
			return Math.Max(a, c) <= Math.Min(b, d);
		}

		/// <summary type="bool" dos="private">
		/// Метод определения пересечения 2-х отрезков. 
		/// </summary>
		/// <param name="a"> Первая точка первого отрезка. </param>
		/// <param name="b"> Вторая точка первого отрезка. </param>
		/// <param name="c"> Первая точка второго отрезка. </param>
		/// <param name="d"> Вторая точка второго отрезка. </param>
		/// <returns> Возвращает логическую переменную, отвечающую на вопрос, пересекаются ли отрезки. </returns>
		private bool IntersectLine(Point a, Point b, Point c, Point d) =>
			Intersect1(a.X, b.X, c.X, d.X) && Intersect1(a.Y, b.Y, c.Y, d.Y) &&
			Area(a, b, c) * Area(a, b, d) <= 0 && Area(c, d, a) * Area(c, d, b) <= 0;

		/// <summary type="bool" dos="private">
		/// Метод определения пересечения отрезка и окружности.
		/// </summary>
		/// <param name="a"> Первая точка отрезка. </param>
		/// <param name="b"> Вторая точка отрезка. </param>
		/// <param name="c"> Центр окружности. </param>
		/// <param name="r"> Радиус окружности. </param>
		/// <returns> Возвращает логическую переменную, отвечающую на вопрос, пересекаются ли отрезок и окружность. </returns>
		private bool IntersectCircle(Point a, Point b, Point c, double r) {
			a.X -= c.X;
			a.Y -= c.Y;
			b.X -= c.X;
			b.Y -= c.Y;

			double dx = b.X - a.X;
			double dy = b.Y - a.Y;

			double aa = dx * dx + dy * dy;
			double bb = 2 * (a.X * dx + a.Y * dy);
			double cc = a.X * a.X + a.Y * a.Y - r * r;

			if (-bb < 0)
				return cc < 0;
			if (-bb < 2 * aa)
				return 4 * aa * cc - bb * bb < 0;
			return aa + bb + cc < 0;
		}

		/// <summary type="void" dos="private">
		/// Метод меняет значения 2-х переменных местами. 
		/// </summary>
		private void Swap<T>(ref T a, ref T b) {
			T temp = a;
			a = b;
			b = temp;
		}

		/// <summary type="void" dos="private">
		/// Метод формирует список вершин графа и устанавливает между ними связи по списку инцидентности.
		/// </summary>
		/// <param name="incList"> Список инцидентности графа. </param>
		private void CreateVertexList(List<List<int>> incList) {
			Vertices = new List<Vertex>();
			Edges = new List<Edge>();

			for (int i = 0; i < n; i++) Vertices.Add(new Vertex(new Point(0, 0), 10, i, Canvas));
			for (int i = 0; i < n; i++) {
				foreach (var j in incList[i]) {
					if (j >= n)
						throw new GraphException($"В графе нет вершины с номером {j}");

					var from = Vertices[i];
					var to = Vertices[j];
					if (from.IsConnected(to))
						continue;

					from.AddEdgeTo(to);
					Edges.Add(from.Edges.Last());
				}
			}
		}

		/// <summary type="double" dos="public">
		/// Свойство для обращения к ширине холста.
		/// </summary>
		public double Width { get => width; set => width = value; }
		/// <summary type="double" dos="public">
		/// Свойство для обращения к высоте холста. 
		/// </summary>
		public double Height { get => height; set => height = value; }
		/// <summary type="List{Vertex}" dos="public">
		/// Свойство для обращения к списку вершин графа.
		/// </summary>
		public List<Vertex> Vertices { get => vertices; set => vertices = value; }
		/// <summary type="List{Edge}" dos="public">
		/// Свойство для обращения к списку рёбер графа.
		/// </summary>
		public List<Edge> Edges { get => edges; set => edges = value; }

		/// <summary type="void" dos="public">
		/// Метод отрисовки графа.
		/// </summary>
		public override void Draw() {
			Canvas.Children.Clear();
			foreach (var item in Edges) {
				item.Draw();
			}
			foreach (var item in Vertices) {
				item.Draw();
			}
		}

		/// <summary type="void" dos="public">
		/// Метод перерисовывает граф в исходном состоянии. 
		/// </summary>
		public override void Refresh() {
			Canvas.Children.Clear();
			foreach (var item in Edges) {
				item.Refresh();
			}
			foreach (var item in Vertices) {
				item.Refresh();
			}
			Draw();
		}

		/// <summary type="void" dos="public">
		/// Метод выбирает новое положение для точек и перерисовывает их.
		/// </summary>
		public void Redraw() {
			Canvas.Children.Clear();
			PositionedVertexes();
			Draw();
		}

		/// <summary type="void" dos="public">
		/// Статический метод сериализует граф в заданый файл.
		/// </summary>
		/// <param name="g"> Сохраняемый граф. </param>
		/// <param name="path"> Путь к файлу для сохранения. </param>
		public static void Serialize(Graph g, string path) {
			try {
				using (var fs = new FileStream(path, FileMode.Create)) {
					var formater = new BinaryFormatter();
					formater.Serialize(fs, g);
				}
			}
			catch (Exception ex) {
				MessageBox.Show("File Error. " + ex.Message);
			}

		}

		/// <summary type="void" dos="public">
		/// Статический метод десериализует граф из заданого файла.
		/// </summary>
		/// <param name="g"> Переменная в которую происходит распаковка. </param>
		/// <param name="path"> Путь к файлу для загрузки. </param>
		public static void Deserialize(ref Graph g, string path) {
			try {
				using (var fs = new FileStream(path, FileMode.Open)) {
					var formater = new BinaryFormatter();
					g = (Graph)formater.Deserialize(fs);
				}
			}
			catch (Exception ex) {
				MessageBox.Show("File Error. " + ex.Message);
			}
		}
	}
}

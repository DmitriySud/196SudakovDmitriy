using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media.Animation;

namespace MainForm {
	/// <summary type="" dos="">
	/// Класс, описывающий вершину графа.
	/// </summary>
	[Serializable]
	public class Vertex : DrawableObject {
		/// <summary type="Color" dos="public">
		/// Цвет ещё не тронутой вершины.
		/// </summary>
		static public Color notUsedColor = Colors.Blue;
		/// <summary type="Color" dos="public">
		/// Цвет вершины, помеченной, как использованной. 
		/// </summary>
		static public Color usedColor = Colors.Red;
		/// <summary type="Color" dos="public">
		/// Цвет текста рядом с вершиной по умолчанию.
		/// </summary>
		static public Color textColor = Colors.RosyBrown;
		/// <summary type="Color" dos="public">
		/// Цвет подстветки текста 1.
		/// </summary>
		static public Color textHighlightedColor1 = Colors.DarkRed;
		/// <summary type="Color" dos="public">
		/// Цвет подстветки текста 2.
		/// </summary>
		static public Color textHighlightedColor2 = Colors.DarkOliveGreen;
		/// <summary type="Color" dos="public">
		/// Цвет вершины, рассматриваемой в данный момент. 
		/// </summary>
		static public Color cutVertexColor = Colors.Green;

		/// <summary type="double" dos="private">
		/// Расстояние от вершины до Label-ов рядом с веришной.
		/// </summary>
		static private double labelDistance = 15;
		/// <summary type="double" dos="private">
		/// Высота Label-ов рядом с веришной.
		/// </summary>
		static private double labelHeight = 37;
		/// <summary type="double" dos="private">
		/// Ширина Label-ов рядом с веришной.
		/// </summary>
		static private double labelWidth = 52;

		/// <summary type="string" dos="public">
		/// Шаблон для Label-а с временем входа.
		/// </summary>
		static public string tinTemplate = "tin[{0}] = {1}";
		/// <summary type="string" dos="public">
		/// Шаблон для Label-а с характеристикой fup.
		/// </summary>
		static public string fupTemplate = "fup[{0}] = {1}";

		/// <summary type="List{Edge}" dos="private">
		/// Список рёбер, выходящих из вершины. 
		/// </summary>
		List<Edge> edges;
		/// <summary type="ALabel" dos="private">
		/// Ссылка на Label с характеристикой tin.
		/// </summary>
		ALabel labelTin = new ALabel();
		/// <summary type="ALabel" dos="private">
		/// Ссылка на Label с характеристикой fup.
		/// </summary>
		ALabel labelFup = new ALabel();


		/// <summary type="int" dos="private">
		/// Время входа в вершину.
		/// </summary>
		int tin;
		/// <summary type="int" dos="private">
		/// максимальная вершина в дереве поиска в глубину до которой можэно добраться. 
		/// </summary>
		int fup;
		/// <summary type="bool" dos="private">
		/// Пометка уже пройденой вершины поиском в глубину.
		/// </summary>
		bool used;
		/// <summary type="bool" dos="private">
		/// Флаг, отвечающий является ли вершина точкой сочленения.
		/// </summary>
		bool isCutVertex = false;
		/// <summary type="double" dos="private">
		/// Размер вершины. 
		/// </summary>
		double size;
		/// <summary type="int" dos="private">
		/// Номер вершины в списке инцидентности.
		/// </summary>
		int num;

		/// <summary type="" dos="public">
		/// Конструктори для инициализации основных полей Вершины.
		/// </summary>
		/// <param name="position"> Координаты на холсте. </param>
		/// <param name="size"> радиус окружности. </param>
		/// <param name="num"> Номер вершины. </param>
		/// <param name="canvas"> Холст на котором отображается вершина. </param>
		public Vertex(Point position, double size, int num, Canvas canvas) : base(canvas) {
			Position = position;
			Size = size;
			Num = num;
			Tin = -1;
			Fup = -1;
			Used = false;
			edges = new List<Edge>();
			InitLabel(LabelTin);
			InitLabel(LabelFup);
		}

		/// <summary type="void" dos="private">
		/// Метод для инициализации label-ов рядом с веришной.
		/// </summary>
		/// <param name="label"></param>
		private void InitLabel(Label label) {
			label.Foreground = new SolidColorBrush(textColor);
			label.FontSize = 10;
			//label.Height = 15;
			//label.Width = 50;
		}

		/// <summary type="int" dos="public">
		/// Свойство для обращения к tin.
		/// </summary>
		public int Tin { 
			get => tin;
			set { 
				tin = value;
				LabelTin.Content = String.Format(tinTemplate, Num, tin);
			}
		}
		/// <summary type="int" dos="public">
		/// Свойство для обращения к fup.
		/// </summary>
		public int Fup { 
			get => fup;
			set {
				fup = value;
				LabelFup.Content = String.Format(tinTemplate, Num, fup);
			}
		}
		/// <summary type="bool" dos="public">
		/// Свойство для обращения к прлю used.
		/// </summary>
		public bool Used {
			get => used;
			set {
				if (value == true)
					Col = usedColor;
				else
					Col = notUsedColor;
				used = value;
			}
		}
		/// <summary type="double" dos="public">
		/// Свойство для обращения к размеру вершины.
		/// </summary>
		public double Size { get => size; set => size = value; }
		/// <summary type="List{Edge}" dos="public">
		/// Свойство для обращения к списку рёбер, исходящих из вершины. 
		/// </summary>
		public List<Edge> Edges { get => edges; set => edges = value; }
		/// <summary type="int" dos="public">
		/// Свойство для обращению к номеру вершины.
		/// </summary>
		public int Num { get => num; set => num = value; }
		/// <summary type="ALabel" dos="public">
		/// Свойство для обращения к Label-у отображаюзему tin.
		/// </summary>
		public ALabel LabelTin { get => labelTin; set => labelTin = value; }
		/// <summary type="ALabel" dos="public">
		/// Свойство для обращения к Label-у отображаюзему fup`.
		/// </summary>
		public ALabel LabelFup { get => labelFup; set => labelFup = value; }
		/// <summary type="bool" dos="public">
		/// Свойство для обращения к флагу, отвечающему за статус точки сочленения.
		/// </summary>
		public bool IsCutVertex { get => isCutVertex; set => isCutVertex = value; }

		/// <summary type="void" dos="public">
		/// Метод отрисовки вершины на холсте.
		/// </summary>
		public override void Draw() {
			if (!(Me is null))
				Canvas.Children.Remove(Me);
			Me = CreateEllipse();
			SetLabels();
		}

		/// <summary type="void" dos="private">
		/// Метод устанавливает координаты Label-ов с дополнительной информацией.
		/// </summary>
		private void SetLabels() {
			//LabelTin.Content = $"tin[{Num}] = {Tin}";
			//LabelFup.Content = $"fup[{Num}] = {Fup}";

			Point vec = new Point();
			foreach (var edge in Edges) {
				vec.X += edge.GetOther(this).Position.X - Position.X;
				vec.Y += edge.GetOther(this).Position.Y - Position.Y;
			}

			double norm = vec.Distance(new Point(0, 0));
			vec.X = -(vec.X* labelDistance) / norm;
			vec.Y = -(vec.Y* labelDistance) / norm;
			if (vec.X < 0) vec.X -= labelWidth;
			if (vec.Y < 0) vec.Y -= labelHeight;

			vec.X = Math.Min(Math.Max(0, Position.X + vec.X), Canvas.Width - labelWidth);
			vec.Y = Math.Min(Math.Max(0, Position.Y + vec.Y), Canvas.Height - labelHeight);

			Canvas.SetLeft(LabelTin, vec.X);
			Canvas.SetTop(LabelTin, vec.Y);

			Canvas.SetLeft(LabelFup, vec.X);
			Canvas.SetTop(LabelFup, vec.Y + 15);

			Canvas.Children.Add(LabelTin);
			Canvas.Children.Add(LabelFup);

		}

		/// <summary type="Ellips" dos="private">
		/// Метод создаёт Ellipse, инициализирует и привязывает его к холсту.
		/// </summary>
		/// <returns> Возвращает инициализированный Ellipse</returns>
		private Ellipse CreateEllipse() {
			var result = new Ellipse();
			result.Height = Size * 2;
			result.Width = Size * 2;

			result.Stroke = new SolidColorBrush(Col);
			result.Fill = new SolidColorBrush(Col);

			Canvas.Children.Add(result);
			Canvas.SetTop(result, Position.Y - Size);
			Canvas.SetLeft(result, Position.X - Size);


			return result;
		}

		/// <summary type="void" dos="public">
		/// Метод добавляет ребро ведущее к переданой вершине.
		/// </summary>
		/// <param name="v"> Вершины к которой ведёт новое ребро. </param>
		public void AddEdgeTo(Vertex v) {
			var edge = new Edge(this, v, Canvas);
			Edges.Add(edge);
			v.Edges.Add(edge);
		}

		/// <summary type="bool" dos="public">
		/// Метод проверяет связаны ли вершины.
		/// </summary>
		/// <param name="v"> Вершина с которой проверяется связь. </param>
		/// <returns> Возвращает логическую переменную отвечающую на посталвенный вопрос.</returns>
		public bool IsConnected(Vertex v) {
			bool result = false;
			for (int i = 0; !result && i < Edges.Count; i++) {
				result = Edges[i].GetOther(this) == v;
			}

			return result;
		}

		/// <summary type="void" dos="public">
		/// Метод для перерисовки вершины в исходном состоянии.
		/// </summary>
		public override void Refresh() {
			isCutVertex = false;
			Col = notUsedColor;
		}

		/// <summary type="Color" dos="public">
		/// Свойство для обращения к цвету вершины.
		/// </summary>
		public override Color Col {
			get => col;
			set {
				isCutVertex |= (value == Vertex.cutVertexColor);
				col = value;
			}
		}
	}
}

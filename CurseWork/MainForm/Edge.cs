using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MainForm {
	/// <summary type="" dos="">
	/// Класс для описания ребра графа.
	/// </summary>
	[Serializable]
	public class Edge : DrawableObject {
		/// <summary type="Color" dos="public">
		/// Цвет ребра которое не является мостом. Он же цвет по умолчанию.
		/// </summary>
		static public Color notBridgeColor = Colors.Black;
		/// <summary type="Color" dos="public">
		/// Цвет ребра, которое является мостом.
		/// </summary>
		static public Color bridgeColor = Colors.Lime;
		/// <summary type="Color" dos="public">
		/// Цвет ребра, которе является ребром дерева поиска в глубину.
		/// </summary>
		static public Color dfsTreeColor = Colors.Magenta;

		/// <summary type="Vertex" dos="private">
		/// Первая вершина, которую соединяет ребро.
		/// </summary>
		Vertex vertexA;
		/// <summary type="Vertex" dos="private">
		/// Вторая вершина, которую соединяет ребро.
		/// </summary>
		Vertex vertexB;
		/// <summary type="bool" dos="">
		/// Флаг, моста.
		/// </summary>
		bool isBridge;

		/// <summary type="" dos="public">
		/// Конструкотр для инициализации ребра.
		/// </summary>
		/// <param name="vertexA"> Первая вершина. </param>
		/// <param name="vertexB"> Вторая вершина. </param>
		/// <param name="canvas"> Холст для отрисовки. </param>
		public Edge(Vertex vertexA, Vertex vertexB, Canvas canvas) : base(canvas) {
			VertexA = vertexA;
			VertexB = vertexB;
			IsBridge = false;
			Col = notBridgeColor;
		}

		/// <summary type="Vertex" dos="public">
		/// Свойство для обращения к первой вершине.
		/// </summary>
		public Vertex VertexA { get => vertexA; set => vertexA = value; }
		/// <summary type="" dos="public">
		/// Свойство для обращение ко второй вершине.
		/// </summary>
		public Vertex VertexB { get => vertexB; set => vertexB = value; }
		/// <summary type="bool" dos="public">
		/// Свойство для обращения к флагу, который помечает ребро как мост и меняет цвет.
		/// </summary>
		public bool IsBridge {
			get => isBridge;
			set {
				if (value == true)
					Col = bridgeColor;
				else
					Col = notBridgeColor;
				isBridge = value;
			}
		}

		/// <summary type="void" dos="public">
		/// Метод отрисовки ребра графа.
		/// </summary>
		public override void Draw() {
			if (!(Me is null))
				Canvas.Children.Remove(Me);
			Me = CreateLine();
		}

		/// <summary type="Line" dos="public">
		/// Метод создания линии, которая будет нарисована.
		/// </summary>
		/// <returns> Возвращает инициализированную и добавленную на холс линию. </returns>
		public Line CreateLine() {
			var result = new Line();

			result.X1 = VertexA.Position.X;
			result.Y1 = VertexA.Position.Y;
			result.X2 = VertexB.Position.X;
			result.Y2 = VertexB.Position.Y;

			result.Stroke = new SolidColorBrush(Col);
			result.StrokeThickness = 3;
			Canvas.Children.Add(result);

			return result;
		}

		/// <summary type="Vertex" dos="public">
		/// Метод возвращает вторую вершину ребра.
		/// </summary>
		/// <param name="cur"> первая вершина ребра. </param>
		public Vertex GetOther(Vertex cur) {
			return vertexA == cur ? vertexB : vertexA;
		}

		/// <summary type="void" dos="public">
		/// Метод возвращения ребра к исходному состоянию.
		/// </summary>
		public override void Refresh() {
			Col = notBridgeColor;
		}
	}
}
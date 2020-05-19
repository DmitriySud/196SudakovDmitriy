using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MainForm {
	/// <summary type="" dos="">
	/// Класс от которого будут наследоваться все рисуемые на холсте объекты.
	/// </summary>
	[Serializable]
	public abstract class DrawableObject {
		/// <summary type="Color" dos="public">
		/// Цвет которым будет подсвечиваться текущий объект.
		/// </summary>
		/// <type> Color </type>
		static public Color currentColor = Colors.Yellow;


		/// <summary type="void" dos="public">
		/// Метод рисования объекта.
		/// </summary>
		public abstract void Draw();
		/// <summary type="void" dos="public">
		/// Метод перерисовки объекта в начальном состоянии.
		/// </summary>
		public abstract void Refresh();

		/// <summary type="void" dos="private">
		/// Ссылка на объект который отрисовывается на холсте.
		/// </summary>
		[NonSerialized]
		private Shape me;
		/// <summary type="Point" dos="private">
		/// Кооотдинаты объекта.
		/// </summary>
		Point position;
		/// <summary type="Canvas" dos="private">
		/// Холст на котором будет отображаться объект.
		/// </summary>
		/// <type> Color </type>
		[NonSerialized]
		Canvas canvas;
		/// <summary type="Color" dos="Protected">
		/// Цвет объекта.
		/// </summary>
		protected Color col;

		/// <summary type="" dos="">
		/// Storyboard для неприрывной анимации.
		/// </summary>
		[NonSerialized]
		Storyboard storyboard = new Storyboard();

		/// <summary type="" dos="protected">
		/// Конструктор для инициализации холста.
		/// </summary>
		/// <param name="canvas"> Холст, на котором будет отображаться объект. </param>
		protected DrawableObject(Canvas canvas) {
			Canvas = canvas;
		}


		/// <summary type="Canvas" dos="public">
		/// Свойство для обращения к холсту.
		/// </summary>
		public Canvas Canvas { get => canvas; set => canvas = value; }
		/// <summary type="Point" dos="public">
		/// Свойство для обращения к координатам объекта.
		/// </summary>
		public Point Position { get => position; set => position = value; }
		/// <summary type="Shape" dos="public">
		/// Свойство для обращения к отображаемому объекту.
		/// </summary>
		public Shape Me { get => me; set => me = value; }
		/// <summary type="Storyboard" dos="public">
		/// Свойство для обращения Storyboard.
		/// </summary>
		public Storyboard Storyboard { get => storyboard; set => storyboard = value; }

		/// <summary type="Color" dos="public">
		/// Свойство для обращения к цвету.
		/// </summary>
		public virtual Color Col {
			get => col;
			set {
				col = value;
			}
		}
	}
}

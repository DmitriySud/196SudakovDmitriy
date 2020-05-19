using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MainForm {
	/// <summary type="" dos="">
	/// Контрол Label с дополнительным Storyboard для анимации.
	/// </summary>
	public class ALabel : Label {
		/// <summary type="Storyboard" dos="private">
		/// Storyboard для неприрывной анимации.
		/// </summary>
		Storyboard storyboard = new Storyboard();

		/// <summary type="" dos="public">
		/// Пустой конструктор для инициализации полей Label-а.
		/// </summary>
		public ALabel() : base() { }

		/// <summary type="Storyboard" dos="public">
		/// Свойство для обращения к полю storyboard.
		/// </summary>
		public Storyboard Storyboard { get => storyboard; set => storyboard = value; }
	}
}

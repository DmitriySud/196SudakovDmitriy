using System.Windows;

namespace MainForm {
	/// <summary>
	/// Логика взаимодействия для MenuWindow.xaml
	/// </summary>
	public partial class MenuWindow : Window {
		public MenuWindow() {
			InitializeComponent();
		}

		private void Button_Bridge_Click(object sender, RoutedEventArgs e) {
			Window w = new MainWindow(0);
			w.ShowDialog();
		}
		private void Button_CutVertex_Click(object sender, RoutedEventArgs e) {
			Window w = new MainWindow(1);
			w.ShowDialog();
		}
	}
}

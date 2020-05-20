using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainForm {
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private Graph g;
		bool drawn = false;
		bool animated = false;
		private int mode = 0;
		private int delta = 1000;
		private int numerator = -1;
		private List<Act> dfsLinesNum = new List<Act>();
		private List<Act> acts;
		private List<Act> backActs = new List<Act>();


		internal List<Act> DfsLinesNum { get => dfsLinesNum; set => dfsLinesNum = value; }

		public MainWindow(int mode) {
			InitializeComponent();
			this.mode = mode;

			foreach (var item in mode == 0 ? GlobalMembers.bridgeAlgorithm : GlobalMembers.cutVertexAlgorithm) {
				var t = new ListViewItem();
				t.Foreground = new SolidColorBrush(Colors.Black);
				t.Content = item;
				codeLines.Items.Add(t);
			}

			if (mode == 1) {
				childrenLabel.Content = "children = -1";
				GlobalMembers.childrenLabel = childrenLabel;
			}

			g = new Graph((new List<int>[] {
				new int[] { 1 }.ToList(),
				new int[] { 0,2,6 }.ToList(),
				new int[] { 1,3 }.ToList(),
				new int[] { 2,4,5 }.ToList(),
				new int[] { 3,5 }.ToList(),
				new int[] { 4,3 }.ToList(),
				new int[] { 1,7,9 }.ToList(),
				new int[] { 6,8 }.ToList(),
				new int[] { 7,9 }.ToList(),
				new int[] { 6,8 }.ToList(),
			}).ToList(), canv.Width, canv.Height, canv);
		}

		private void Button_Draw_Click(object sender, RoutedEventArgs e) {
			Refresh();
			if (drawn)
				g.Redraw();
			else 
				g.Draw();

			drawn = true;
		}

		private void Button_Animate_Click(object sender, RoutedEventArgs e) {
			if (!drawn) {
				MessageBox.Show("Чтобы запустить анимацию сначала нарисуйте граф.");
				return;
			}
			if (animated) {
				Refresh();
				GlobalMembers.StopAnimation(g, this);
				animated = false;
			}
			else {
				Refresh();
				var lst = GlobalMembers.GetActsList(g.Vertices, mode);
				GlobalMembers.Animate(g, lst, this, delta);
				animated = true;
			}
		}

		private void Button_Back_Click(object sender, RoutedEventArgs e) {
			if (!drawn) {
				MessageBox.Show("Чтобы запустить анимацию сначала нарисуйте граф.");
				return;
			}
			if (animated) {
				MessageBox.Show("Остановите анимацию или перерисуйте граф.");
				return;
			}
			if (numerator < 1) {
				MessageBox.Show("Сначала промоделируйте хотя бы 2 шага.");
				return;
			}

			GlobalMembers.AnimateAct(backActs[numerator--], this, delta);
			backActs.Remove(backActs.Last());
		}

		private void Button_Next_Click(object sender, RoutedEventArgs e) {
			if (!drawn) {
				MessageBox.Show("Чтобы запустить анимацию сначала нарисуйте граф.");
				return;
			}
			if (animated) {
				MessageBox.Show("Остановите анимацию или перерисуйте граф.");
				return;
			}

			if (acts is null) {
				acts = GlobalMembers.GetActsList(g.Vertices, mode);
				numerator = -1;
			}

			if (++numerator < acts.Count) {
				backActs.Add(acts[numerator].GetReverseAct());
				GlobalMembers.AnimateAct(acts[numerator], this, delta);
			}
			else {
				MessageBox.Show("конец");
				numerator = -1;
			}
		}

		private static string path = "temp.bin";

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			if (!drawn) {
				MessageBox.Show("Чтобы сохранить граф сначала нарисуйте его.");
				return;
			}

			Graph.Serialize(g, path);

			MessageBox.Show($"Граф сохранён в файл {path}");
		}
		private void Button_Create_Click(object sender, RoutedEventArgs e) {
			var builder = new GraphBuilderWindow();
			builder.ShowDialog();

			if (!(builder.result is null))
				g = new Graph(builder.result, canv.Width, canv.Height, canv);
			drawn = false;
			canv.Children.Clear();
		}

		private void Button_Load_Click(object sender, RoutedEventArgs e) {
			if (!File.Exists(path)) {
				MessageBox.Show("Граф не был сохранён.");
				return;
			}

			Graph.Deserialize(ref g, path);
			g.Refresh();

			MessageBox.Show($"Граф сохранён в файл {path}");
		}

		public void DfsLines_Item_Clicked(object sender, RoutedEventArgs e) {
			int numder = dfsLines.SelectedIndex;
			var act = DfsLinesNum[numder];
			Refresh();
			numerator = -1;
			while (numerator == -1 || acts[numerator] != act) {
				numerator++;
				GlobalMembers.AnimateAct(acts[numerator], this, delta);
			}
		}

		private void Refresh() {
			canv.Children.Clear();
			acts = null;
			numerator = 0;
			backActs.Clear();
			g.Refresh();
			animated = false;


			foreach (var item in codeLines.Items) {
				((ListViewItem)item).Foreground = new SolidColorBrush(Colors.Black);
			}

			DfsLinesNum.Clear();
			dfsLines.Items.Clear();
		}

		private void RadioButton1x_Checked(object sender, RoutedEventArgs e) {
			delta = 1000;
		}
		private void RadioButton2x_Checked(object sender, RoutedEventArgs e) {
			delta = 500;
		}
		private void RadioButton5x_Checked(object sender, RoutedEventArgs e) {
			delta = 250;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MainForm {
	/// <summary type="" dos="">
	/// Логика взаимодействия для GraphBuilderWindow.xaml
	/// </summary>
	public partial class GraphBuilderWindow : Window {
		/// <summary type="int" dos="private">
		/// Количество вершин графа. По умолчанию 3.
		/// </summary>
		private int vertexNum = 3;
		/// <summary type="List{List{int}}" dos="public">
		/// Сформированный список инцидентности. 
		/// </summary>
		public List<List<int>> result;

		/// <summary type="" dos="public">
		/// Конструктор для инициализации графа.
		/// </summary>
		public GraphBuilderWindow() {
			InitializeComponent();
		}


		private void Button_AddVertex_Click(object sender, RoutedEventArgs e) {
			if (vertexNum == 10) {
				MessageBox.Show("Максимальное количество вершин - 10");
				return;
			}

			var newItem = new ListViewItem();
			newItem.Content = $"{vertexNum} -> ";
			verticesList.Items.Add(newItem);

			vertexNum++;
			verticesLabel.Content = vertexNum;
		}

		private void Button_RemoveVertex_Click(object sender, RoutedEventArgs e) {
			if (vertexNum == 3) {
				MessageBox.Show("Минимальное количество вершин - 3");
				return;
			}

			--vertexNum;
			verticesList.Items.RemoveAt(vertexNum);

			RemoveNums(vertexNum);
			verticesLabel.Content = vertexNum;
		}

		private void RemoveNums(int num) {
			foreach (ListViewItem item in verticesList.Items) {
				int idx = ((string)item.Content).IndexOf($"{num} ");
				if (idx > 4)
					item.Content = ((string)item.Content).Remove(idx, num >= 10 ? 3 : 2);
			}
		}

		private void Button_AddEdge_Click(object sender, RoutedEventArgs e) {
			fromTextBox.Text = fromTextBox.Text == "" ? "0" : fromTextBox.Text;
			toTextBox.Text = toTextBox.Text == "" ? "0" : toTextBox.Text;
			int fromNum = int.Parse(fromTextBox.Text),
				toNum = int.Parse(toTextBox.Text);
			if (fromNum >= vertexNum || toNum >= vertexNum) {
				MessageBox.Show($"Невозможно добавить ребро {fromTextBox.Text} -> {toTextBox.Text}, так как одной из вершин не существует.");
				return;
			}
			if (((string)((ListViewItem)verticesList.Items[fromNum]).Content).IndexOf(toTextBox.Text) != -1) {
				MessageBox.Show($"Такое ребро уже существует.");
				return;
			}

			((ListViewItem)verticesList.Items[fromNum]).Content += $"{toNum} ";
			((ListViewItem)verticesList.Items[toNum]).Content += $"{fromNum} ";
		}

		private void Button_RemoveEdge_Click(object sender, RoutedEventArgs e) {
			fromTextBox.Text = fromTextBox.Text == "" ? "0" : fromTextBox.Text;
			toTextBox.Text = toTextBox.Text == "" ? "0" : toTextBox.Text;
			int fromNum = int.Parse(fromTextBox.Text),
				toNum = int.Parse(toTextBox.Text);
			var fromItem = ((ListViewItem)verticesList.Items[fromNum]);
			var toItem = ((ListViewItem)verticesList.Items[toNum]);

			if (fromNum >= vertexNum || toNum >= vertexNum) {
				MessageBox.Show($"Невозможно убрать ребро {fromTextBox.Text} -> {toTextBox.Text}, так как одной из вершин не существует.");
				return;
			}
			if (((string)fromItem.Content).IndexOf(toTextBox.Text) == -1) {
				MessageBox.Show($"Такого ребра не существует.");
				return;
			}

			fromItem.Content = ((string)fromItem.Content).Remove(((string)fromItem.Content).IndexOf($"{toNum} "), toNum > 10 ? 3 : 2);
			toItem.Content = ((string)toItem.Content).Remove(((string)toItem.Content).IndexOf($"{fromNum} "), fromNum > 10 ? 3 : 2);
		}

		private void Button_Create_Click(object sender, RoutedEventArgs e) {
			List<List<int>> arr = new List<List<int>>();
			try {
				foreach (ListViewItem item in verticesList.Items) {
					arr.Add(((string)item.Content).Split('>')[1].Trim(' ').Split(' ').ToList().ConvertAll(x => int.Parse(x)));
				}
				result = arr;
				Close();
			}
			catch {
				MessageBox.Show("Невозможно создать такой граф");
			}
		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
			if (!Char.IsDigit(e.Text, 0)) {
				e.Handled = true;
			}
		}
	}
}

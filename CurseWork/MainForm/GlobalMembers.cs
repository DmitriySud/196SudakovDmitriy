using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml;
using System.Runtime.CompilerServices;

namespace MainForm {
	/// <summary type="" dos="">
	/// Статический класс с основномными методами визуализации.
	/// </summary>
	static class GlobalMembers {
		/// <summary type="List{Act}" dos="private">
		/// Список событий анимации, промодеоированный для отображённого графа.
		/// </summary>
		private static List<Act> acts;
		/// <summary type="bool[]" dos="private">
		/// Массив пометок для вершин графа.
		/// </summary>
		private static bool[] used;
		/// <summary type="int" dos="private">
		/// Количество вершин графа.
		/// </summary>
		private static int n;
		/// <summary type="List{Vertex}" dos="private">
		/// Список вершин графа.
		/// </summary>
		private static List<Vertex> vertices;
		/// <summary type="int" dos="private">
		/// Переменная таймер для поиска в глубину.
		/// </summary>
		private static int timer;
		/// <summary type="int[]" dos="private">
		/// Мессив времени входа в каждую вершину.
		/// </summary>
		private static int[] tin;
		/// <summary type="int[]" dos="private">
		/// Массив дополнительной характеристики для каждой вершины для поиска в глубину.
		/// </summary>
		private static int[] fup;
		/// <summary type="ALabel" dos="public">
		/// Ссылка на Lable отображающий количество детей для текущей вершины.
		/// </summary>
		public static ALabel childrenLabel;
		/// <summary type="int" dos="private">
		/// Режим. 0 - поиск мостов, 1 - поиск точек сочленения.
		/// </summary>
		private static int mode = 0;
		/// <summary type="int" dos="public">
		/// Номер предыдущей линии кода.
		/// </summary>
		public static int prevLine = 0;
		/// <summary type="List{Storyboard}" dos="private">
		/// Массив Storyboard-ов для каждой строки кода алгоритма.
		/// </summary>
		private static List<Storyboard> codeStoryboards;


		/// <summary type="string[]" dos="public">
		/// Массив подсказок на каждую строку для алгоритма поиска мостов.
		/// </summary>
		public static readonly string[] bridgesHint = new string[]{
			"Запустим поиск в глубину из данной вершины v, со ссылкой на родителя par." + Environment.NewLine +
			"v, par - номера соответствующих вершин.",

			"Чтобы в последующих итерациях не рассматривать данную вершину пометим её " + Environment.NewLine +
			"как использованную." + Environment.NewLine +
			"Пометки осуществляются при помощи массива bool[] used." + Environment.NewLine+
			"Если used[i] = true, то i-ая вершина использована.",

			"Зафиксируем время входа в вершину." + Environment.NewLine +
			"А так же инициализируем fup текущим значением таймера.",

			"Рассмотрим все рёбра, идущие из данной вершины. " + Environment.NewLine +
			"edge - текущее ребро.",

			"Пусть to - номер вершины, в которую ведёт ребро edge.",

			"Если текущее ребро ведёт в родителя, то есть to == par," + Environment.NewLine +
			"тогда рассматривать данное ребро нет смысла." + Environment.NewLine +
			"Так как мы ищем максимально высокую в дереве поиска в глубину вершину," + Environment.NewLine +
			"в которю мы можем добраться, минуя родителя",

			"Перейдём к следующему ребру.",

			"Если ребро ведёт в вершину, которая уже помечена как использованная," + Environment.NewLine+
			"значит она уже принадлежит дереву поиска в глубину. " + Environment.NewLine +
			"Тогда если через рассматриваемую вершину мы можем" + Environment.NewLine +
			"подняться выше(в вершину с меньшим номером), то запомним эту информацию.",

			"Самая высокая вершина (с минимальным номером) - минимум из той, " +Environment.NewLine+
			"в которую мы уже можем добраться и той, в которую мы можем попасть " + Environment.NewLine +
			"через вершину to.",

			"Если вершина ещё не была использованна, то в неё надо зайти",

			"Запустим dfs из этой вершины." +Environment.NewLine +
			"И передадим текущую вершину как родителя для новой.",

			"Теперь данная вершина уже обработана и мы можем узнать, " + Environment.NewLine+
			"способны ли мы подняться выше через неё." +Environment.NewLine+
			"Аналогичными рассуждениями получаем, что : " + Environment.NewLine+
			"Самая высокая вершина (с минимальным номером) - минимум из той," + Environment.NewLine+
			"в которую мы уже можем добраться и той, в которую мы можем попасть " + Environment.NewLine +
			"через вершину to.",

			"Наконец, если через вершину to мы не сможем подняться в дереве поиска " + Environment.NewLine+

			"в глубину выше, чем вершина v" + Environment.NewLine +

			"При этом минуя саму вершину v, то ребро to-v это мост.",

			"Отметим ребро как мост."
		};

		/// <summary type="string[]" dos="public">
		/// Массив подсказок на каждую строку для алгоритма поиска точек сочленения.
		/// </summary>
		public static readonly string[] cutVerticesHint = new string[]{
			"Запустим поиск в глубину из данной вершины v, со ссылкой на родителя par." + Environment.NewLine +
			"v, par - номера соответствующих вершин.",

			"Чтобы в последующих итерациях не рассматривать данную вершину пометим её " + Environment.NewLine +
			"как использованную." + Environment.NewLine +
			"Пометки осуществляются при помощи массива bool[] used." + Environment.NewLine+
			"Если used[i] = true, то i-ая вершина использована.",

			"Зафиксируем время входа в вершину." + Environment.NewLine +
			"А так же инициализируем fup текущим значением таймера.",

			"Заведём переменную, для подстчёта количества детей вершины в дереве " + Environment.NewLine +
			"поиска в глубину.",  

			"Рассмотрим все рёбра, идущие из данной вершины. " + Environment.NewLine +
			"edge - текущее ребро.",

			"Пусть to - номер вершины, в которую ведёт ребро edge.",

			"Если текущее ребро ведёт в родителя, то есть to == par," + Environment.NewLine +
			"тогда рассматривать данное ребро нет смысла." + Environment.NewLine +
			"Так как мы ищем максимально высокую в дереве поиска в глубину вершину," + Environment.NewLine +
			"в которю мы можем добраться, минуя родителя",

			"Перейдём к следующему ребру.",

			"Если ребро ведёт в вершину, которая уже помечена как использованная," + Environment.NewLine+
			"значит она уже принадлежит дереву поиска в глубину. " + Environment.NewLine +
			"Тогда если через рассматриваемую вершину мы можем" + Environment.NewLine +
			"подняться выше(в вершину с меньшим номером), то запомним эту информацию.",

			"Самая высокая вершина (с минимальным номером) - минимум из той, " +Environment.NewLine+
			"в которую мы уже можем добраться и той, в которую мы можем попасть " + Environment.NewLine +
			"через вершину to.",

			"Если вершина ещё не былаx использованна, то в неё надо зайти.",

			"Увеличим количество детей в дереве поиска в глубину.",

			"Запустим dfs из этой вершины." +Environment.NewLine +
			"И передадим текущую вершину как родителя для новой.",

			"Теперь данная вершина уже обработана и мы можем узнать, " + Environment.NewLine+
			"способны ли мы подняться выше через неё." +Environment.NewLine+
			"Аналогичными рассуждениями получаем, что : " + Environment.NewLine+
			"Самая высокая вершина (с минимальным номером) - минимум из той," + Environment.NewLine+
			"в которую мы уже можем добраться и той, в которую мы можем попасть " + Environment.NewLine +
			"через вершину to.",

			"Наконец, если через вершину to мы не сможем подняться в дереве поиска " + Environment.NewLine+
			"в глубину выше, чем вершина v," + Environment.NewLine +
			"При этом минуя саму вершину v, то v - точка сочленения.",

			"Отметим вершину как точку сочленения.",

			"Если это наша стартовая вершина то она будет точкой сочленения только " + Environment.NewLine + 
			"тогда, когда у неё больше 1-го ребёнка.",

			"Отметим вершину как точку сочленения.",
			};


		/// <summary type="string[]" dos="public">
		/// Массив строк кода алгоритма поиска мостов.
		/// </summary>
		public static string[] bridgeAlgorithm = {
			"dfs(int v, int p)",
			"    пометить вершину, как использованную",
			"    tin[v]=fup[v]=timer++" ,
			"    переберём все рёбра edge:" ,
			"        to = вершина в которую ведёт edge" ,
			"        Если ребро в родителя : " ,
			"            Пропустим его" ,
			"        Если ребро в использованную вершину :" ,
			"            fup[v] = Min(fup[v], fup[to]);" ,
			"        Иначе :" ,
			"            dfs(to, v)",
			"            fup[v] = Min(fup[v], fup[to]);" ,
			"            Если fup[to] > tin[v]:" ,
			"                edge - мост"
		};

		/// <summary type="string[]" dos="public">
		/// Массив строк кода алгоритма поиска точек сочленения.
		/// </summary>
		public static string[] cutVertexAlgorithm = {
			"dfs(int v, int p)",
			"    пометить вершину, как использованную",
			"    tin[v]=fup[v]=timer++" ,
			"    children = 0" ,
			"    переберём все рёбра edge:" ,
			"        to = вершина в которую ведёт edge" ,
			"        Если ребро в родителя : " ,
			"            Пропустим его" ,
			"        Если ребро в использованную вершину :" ,
			"            fup[v] = Min(fup[v], fup[to]);" ,
			"        Иначе :" ,
			"            children++",
			"            dfs(to, v)",
			"            fup[v] = Min(fup[v], fup[to]);" ,
			"            Если (fup[to] >= tin[v]) и (p != -1):" ,
			"                v - точка сочленения",
			"    Если (p == 1) и (children > 1)",
			"        v - точка сочленения",
		};

		/// <summary type="string" dos="public">
		/// Шаблон для Label-а отображающего количество детей.
		/// </summary>
		public static readonly string childrenTemplate = "children = {0}";

		/// <summary type="List{Act}" dos="public">
		/// Метод моделирует алгоритм на графе.
		/// </summary>
		/// <param name="v"> Список вершин графа. </param>
		/// <param name="mode"> Режим. 0 - поиск мостов, 1 - поиск точек сочлененеия. </param>
		/// <returns> Возвращает список анимационных действий типа Act. </returns>
		public static List<Act> GetActsList(List<Vertex> v, int mode) {
			acts = new List<Act>();
			vertices = v;
			n = v.Count;
			used = new bool[n];
			timer = 0;
			tin = new int[n];
			fup = new int[n];
			GlobalMembers.mode = mode;

			for (int i = 0; i < n; i++) {
				if (!used[i])
					if (mode == 0)
						dfsBridge(vertices[i]);
					else
						dfsCutVertex(vertices[i]);
			}
			return acts;
		}

		/// <summary type="void" dos="private">
		/// Алгоритм поиска в глубину для поиска точек сочленения. 
		/// </summary>
		/// <param name="v"> Текущая вершина. </param>
		/// <param name="par"> Вершина предка. </param>
		private static void dfsCutVertex(Vertex v, Vertex par = null) {
			acts.Add(new Act(
				ActTypes.highlightCodeLine |
				ActTypes.createNewDfsLine |
				ActTypes.updateChildren,
				v, new Act.ActData(lineNum: 0, parentNum: (par is null ? -1 : par.Num), newChildren: -1)));

			used[v.Num] = true;
			acts.Add(new Act(
				ActTypes.highlightVertexUsed |
				ActTypes.highlightCodeLine, v, new Act.ActData(lineNum: 1)));

			tin[v.Num] = fup[v.Num] = timer++;
			acts.Add(new Act(
				ActTypes.highlightVertexCurrent |
				ActTypes.updateTin |
				ActTypes.updateFup |
				ActTypes.highlightLabels |
				ActTypes.highlightCodeLine,
				v, v.LabelTin, v.LabelFup, new Act.ActData(tin[v.Num], fup[v.Num], 2)));

			int children = 0;
			acts.Add(new Act(
				ActTypes.updateChildren |
				ActTypes.highlightCodeLine,
				v, new Act.ActData(lineNum: 3, newChildren: 0)));


			acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 4)));
			foreach (var edge in v.Edges) {
				acts.Add(new Act(
					ActTypes.highlightEdgeCurrent |
					ActTypes.highlightCodeLine, edge, new Act.ActData(lineNum: 5)));
				var to = edge.GetOther(v);

				acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 6)));
				if (to == par) {
					acts.Add(new Act(
						ActTypes.highlightCodeLine |
						ActTypes.highlightEdgeAsDfsTree, edge, new Act.ActData(lineNum: 7)));
					continue;
				}

				acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 8)));
				if (used[to.Num]) {
					fup[v.Num] = Math.Min(fup[v.Num], tin[to.Num]);
					acts.Add(new Act(
						ActTypes.highlightLabels |
						ActTypes.updateFup |
						ActTypes.highlightCodeLine |
						ActTypes.highlightEdgeNotCurrent,
						v, v.LabelFup, to.LabelTin, edge, new Act.ActData(newFup: fup[v.Num], lineNum: 9)));
				}
				else {
					acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 10)));

					++children;
					acts.Add(new Act(ActTypes.highlightCodeLine|
						ActTypes.updateChildren, new Act.ActData(lineNum: 11, newChildren:children)));


					acts.Add(new Act(
						ActTypes.highlightVertexUsed |
						ActTypes.highlightEdgeAsDfsTree |
						ActTypes.highlightCodeLine,
						v, edge, new Act.ActData(lineNum: 12)));
					dfsCutVertex(to, v);
					acts.Add(new Act(
						ActTypes.highlightVertexCurrent |
						ActTypes.highlightEdgeCurrent |
						ActTypes.updateChildren
						, v, edge, new Act.ActData(newChildren: children)));

					fup[v.Num] = Math.Min(fup[v.Num], fup[to.Num]);
					acts.Add(new Act(
						ActTypes.highlightLabels |
						ActTypes.updateFup |
						ActTypes.highlightCodeLine,
						v, v.LabelFup, to.LabelFup, new Act.ActData(newFup: fup[v.Num], lineNum: 13)));

					acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 14)));
					if (fup[to.Num] >= tin[v.Num] && !(par is null)) {
						acts.Add(new Act(
							ActTypes.highlightCutVertex |
							ActTypes.highlightCodeLine,
							v, new Act.ActData(lineNum: 15)));
						// v - cut Vertex!!!
					}
					acts.Add(new Act(ActTypes.highlightCodeLine | ActTypes.highlightEdgeAsDfsTree, edge, new Act.ActData(lineNum: 4)));
				}
			}

			acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 16)));
			if (par is null && children > 1) {
				acts.Add(new Act(
							ActTypes.highlightCutVertex |
							ActTypes.highlightCodeLine,
							v, new Act.ActData(lineNum: 17)));
				// v - cut Vertex
			}

			acts.Add(new Act(ActTypes.highlightVertexUsed | ActTypes.removeNewDfsLine,
				v, new Act.ActData(parentNum: par is null ? -1 : par.Num)));
		}

		/// <summary type="void" dos="private">
		/// Алгоритм поиска в глубину для поиска мостов. 
		/// </summary>
		/// <param name="v"> Текущая вершина. </param>
		/// <param name="par"> Вершина предка. </param>
		private static void dfsBridge(Vertex v, Vertex par = null) {
			acts.Add(new Act(
				ActTypes.highlightCodeLine |
				ActTypes.createNewDfsLine,
				v, new Act.ActData(lineNum: 0, parentNum: (par is null ? -1 : par.Num))));

			used[v.Num] = true;
			acts.Add(new Act(
				ActTypes.highlightVertexUsed |
				ActTypes.highlightCodeLine, v, new Act.ActData(lineNum: 1)));

			tin[v.Num] = fup[v.Num] = timer++;
			acts.Add(new Act(
				ActTypes.highlightVertexCurrent |
				ActTypes.updateTin |
				ActTypes.updateFup |
				ActTypes.highlightLabels |
				ActTypes.highlightCodeLine,
				v, v.LabelTin, v.LabelFup, new Act.ActData(tin[v.Num], fup[v.Num], 2)));

			acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 3)));
			foreach (var edge in v.Edges) {
				acts.Add(new Act(
					ActTypes.highlightEdgeCurrent |
					ActTypes.highlightCodeLine, edge, new Act.ActData(lineNum: 4)));
				var to = edge.GetOther(v);

				acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 5)));
				if (to == par) {
					acts.Add(new Act(
						ActTypes.highlightCodeLine |
						ActTypes.highlightEdgeAsDfsTree, edge, new Act.ActData(lineNum: 6)));
					continue;
				}

				acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 7)));
				if (used[to.Num]) {
					fup[v.Num] = Math.Min(fup[v.Num], tin[to.Num]);
					acts.Add(new Act(
						ActTypes.highlightLabels |
						ActTypes.updateFup |
						ActTypes.highlightCodeLine |
						ActTypes.highlightEdgeNotCurrent,
						v, v.LabelFup, to.LabelTin, edge, new Act.ActData(newFup: fup[v.Num], lineNum: 8)));
				}
				else {
					acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 9)));

					acts.Add(new Act(
						ActTypes.highlightVertexUsed |
						ActTypes.highlightEdgeAsDfsTree |
						ActTypes.highlightCodeLine,
						v, edge, new Act.ActData(lineNum: 10)));
					dfsBridge(to, v);

					acts.Add(new Act(ActTypes.highlightVertexCurrent | ActTypes.highlightEdgeCurrent
						, v, edge));
					fup[v.Num] = Math.Min(fup[v.Num], fup[to.Num]);
					acts.Add(new Act(
						ActTypes.highlightLabels |
						ActTypes.updateFup |
						ActTypes.highlightCodeLine,
						v, v.LabelFup, to.LabelFup, new Act.ActData(newFup: fup[v.Num], lineNum: 11)));

					acts.Add(new Act(ActTypes.highlightCodeLine, new Act.ActData(lineNum: 12)));
					if (fup[to.Num] > tin[v.Num]) {
						acts.Add(new Act(
							ActTypes.highlightEdgeAsBridge |
							ActTypes.highlightCodeLine,
							edge, new Act.ActData(lineNum: 13)));
						// item - bridge!!!
					}
					else {
						acts.Add(new Act(ActTypes.highlightEdgeAsDfsTree, edge, new Act.ActData(lineNum: 3)));
					}
					acts.Add(new Act(ActTypes.highlightCodeLine, edge, new Act.ActData(lineNum: 3)));
				}
			}
			acts.Add(new Act(ActTypes.highlightVertexUsed | ActTypes.removeNewDfsLine,
				v, new Act.ActData(parentNum: par is null ? -1 : par.Num)));
		}

		/// <summary type="void" dos="public">
		/// Метод для запуска непрерывной анимации на главном окне.
		/// </summary>
		/// <param name="g"> Граф, на котором можелируется алгоритм. </param>
		/// <param name="lst"> Список анимируемых действий типа Act. </param>
		/// <param name="window"> Ссылка на окно на котором происходит анимация. </param>
		/// <param name="delta"> Задержка между действиями. </param>
		public static void Animate(Graph g, List<Act> lst, MainWindow window, int delta) {

			codeStoryboards = new List<Storyboard>();
			InitStoryboards(lst, window, codeStoryboards, delta);
			//myStoryBoard.Begin(lst[0].Obj.Me);
			if (mode == 1) childrenLabel.Storyboard.Begin(window);
			foreach (var edge in g.Edges) {
				edge.Storyboard.Begin(window);
			}
			foreach (var item in codeStoryboards) {
				item.Begin(window);
			}
			foreach (var vertex in g.Vertices) {
				vertex.Storyboard.Begin(window);
				vertex.LabelTin.Storyboard.Begin(window);
				vertex.LabelFup.Storyboard.Begin(window);
			}
		}

		/// <summary type="void" dos="public">
		/// Метод останавливает неприрывную анимацию. 
		/// </summary>
		/// <param name="g"> Граф, на котором можелируется алгоритм. </param>
		/// <param name="window"> Ссылка на окно на котором происходит анимация. </param>
		public static void StopAnimation(Graph g, MainWindow window) {
			//myStoryBoard.Begin(lst[0].Obj.Me);
			if (mode == 1) childrenLabel.Storyboard.Stop(window);
			foreach (var edge in g.Edges) {
				edge.Storyboard.Stop(window);
			}
			foreach (var item in codeStoryboards) {
				item.Stop(window);
			}
			foreach (var vertex in g.Vertices) {
				vertex.Storyboard.Stop(window);
				vertex.LabelTin.Storyboard.Stop(window);
				vertex.LabelFup.Storyboard.Stop(window);
			}
		}

		/// <summary type="void" dos="public">
		/// Метод анимирует одно действие типа Act.
		/// </summary>
		/// <param name="action"> Анимируемое действие. </param>
		/// <param name="window"> Ссылка на окно на котором происходит анимация. </param>
		/// <param name="delta"> Задержка между действиями. </param>
		public static void AnimateAct(Act action, MainWindow window, int delta) {
			var itemu = (ListViewItem)window.codeLines.Items.GetItemAt((int)prevLine);
			var animu = RepaintCodeLine(window, 0, delta, itemu, Colors.Black, false);
			itemu.Foreground = new SolidColorBrush();
			itemu.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animu, HandoffBehavior.Compose);

			if ((uint)(ActTypes.highlightEdgeCurrent & action.ActType) != 0) {
				var anim = RepaintEdge(0, delta, action, DrawableObject.currentColor, false);
				action.edge.Me.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, anim);
				action.edge.Col = DrawableObject.currentColor;
			}
			if ((uint)(ActTypes.highlightEdgeNotCurrent & action.ActType) != 0) {
				var anim = RepaintEdge(0, delta, action, Edge.notBridgeColor, false);
				action.edge.Me.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, anim);
				action.edge.Col = Edge.notBridgeColor;
			}
			if ((uint)(ActTypes.highlightVertexUsed & action.ActType) != 0) {
				var anim = RepaintVertex(0, delta, action, Vertex.usedColor);
				action.vertex.Me.Fill.BeginAnimation(SolidColorBrush.ColorProperty, anim);
				action.vertex.Col = Vertex.usedColor;
			}
			if ((uint)(ActTypes.highlightCutVertex & action.ActType) != 0) {
				var anim = RepaintVertex(0, delta, action, Vertex.cutVertexColor);
				action.vertex.Me.Fill.BeginAnimation(SolidColorBrush.ColorProperty, anim);
				action.vertex.Col = Vertex.cutVertexColor;
			}
			if ((uint)(ActTypes.highlightVertexCurrent & action.ActType) != 0) {
				var anim = RepaintVertex(0, delta, action, DrawableObject.currentColor);
				action.vertex.Me.Fill.BeginAnimation(SolidColorBrush.ColorProperty, anim);
				action.vertex.Col = DrawableObject.currentColor;
			}
			if ((uint)(ActTypes.highlightVertexDef & action.ActType) != 0) {
				var anim = RepaintVertex(0, delta, action, Vertex.notUsedColor);
				action.vertex.Me.Fill.BeginAnimation(SolidColorBrush.ColorProperty, anim);
				action.vertex.Col = Vertex.notUsedColor;
			}
			if ((uint)(ActTypes.highlightEdgeAsBridge & action.ActType) != 0) {
				var anim = RepaintEdge(0, delta, action, Edge.bridgeColor, false);
				action.edge.Me.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, anim);
				action.edge.Col = Edge.bridgeColor;
			}
			if ((uint)(ActTypes.highlightEdgeAsDfsTree & action.ActType) != 0) {
				var anim = RepaintEdge(0, delta, action, Edge.dfsTreeColor, false);
				action.edge.Me.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, anim);
				action.edge.Col = Edge.dfsTreeColor;
			}
			if ((uint)(ActTypes.highlightLabels & action.ActType) != 0) {
				foreach (var label in action.labels) {
					var newColor = (label == action.vertex.LabelTin || label == action.vertex.LabelFup) ?
									Vertex.textHighlightedColor1 :
									Vertex.textHighlightedColor2;

					var anim = RepaintLabel(0, delta, newColor, label);
					label.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, anim);
				}
			}
			if ((uint)(ActTypes.updateTin & action.ActType) != 0) {
				var anim = ChangeLabelContent(0, delta, action.vertex.LabelTin,
					String.Format(Vertex.tinTemplate, action.vertex.Num, action.data.newTin));
				action.vertex.LabelTin.BeginAnimation(Label.ContentProperty, anim, HandoffBehavior.Compose);
			}
			if ((uint)(ActTypes.updateFup & action.ActType) != 0) {
				var anim = ChangeLabelContent(0, delta, action.vertex.LabelFup,
					String.Format(Vertex.fupTemplate, action.vertex.Num, action.data.newFup));
				action.vertex.LabelFup.BeginAnimation(Label.ContentProperty, anim, HandoffBehavior.Compose);
			}
			if ((uint)(ActTypes.updateChildren & action.ActType) != 0) {
				var anim = ChangeLabelContent(0, delta, childrenLabel,
					String.Format(childrenTemplate, action.data.newChildren));
				childrenLabel.BeginAnimation(Label.ContentProperty, anim, HandoffBehavior.Compose);
			}
			if ((uint)(ActTypes.highlightCodeLine & action.ActType) != 0) {
				var item = (ListViewItem)window.codeLines.Items.GetItemAt((int)action.data.lineNum);
				var anim = RepaintCodeLine(window, 0, delta, item, DrawableObject.currentColor, false);
				item.Foreground = new SolidColorBrush();
				item.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, anim, HandoffBehavior.Compose);
				prevLine = (int)action.data.lineNum;
				window.HintBlock.Text = mode == 0 ? bridgesHint[(int)action.data.lineNum] : cutVerticesHint[(int)action.data.lineNum];
			}
			if ((uint)(ActTypes.createNewDfsLine & action.ActType) != 0) {
				var line = new ListViewItem();
				line.Content = $"dfs({action.vertex.Num},  {action.data.parentNum})";
				line.MouseDoubleClick += window.DfsLines_Item_Clicked;
				window.DfsLinesNum.Add(action);
				window.dfsLines.Items.Add(line);
			}
			if ((uint)(ActTypes.removeNewDfsLine & action.ActType) != 0) {
				window.dfsLines.Items.RemoveAt(window.dfsLines.Items.Count - 1);
				window.DfsLinesNum.RemoveAt(window.DfsLinesNum.Count - 1);
			}
		}

		/// <summary type="void" dos="private">
		/// Метод для инициализации всех Storyboard-ов для неприрывной анимации.
		/// </summary>
		/// <param name="lst"> Список анимаируемых действий. </param>
		/// <param name="window"> Ссылка на окно на котором происходит анимация. </param>
		/// <param name="delta"> Задержка между действиями. </param>
		/// <param name="codeStoryboards"> Список Storyboard-ов для строк кода алгоритма. </param>
		private static void InitStoryboards(List<Act> lst, MainWindow window, List<Storyboard> codeStoryboards, int delta) {
			int seconds = 0;
			bool doubleDelta;
			foreach (var item in window.codeLines.Items) {
				codeStoryboards.Add(new Storyboard());
			}
			foreach (var item in lst) {
				doubleDelta = false;
				if ((uint)(ActTypes.highlightEdgeCurrent & item.ActType) != 0) {
					item.edge.Storyboard.Children.Add(RepaintEdge(seconds, delta, item, DrawableObject.currentColor));
					doubleDelta = true;
				}
				if ((uint)(ActTypes.highlightVertexUsed & item.ActType) != 0) {
					item.vertex.Storyboard.Children.Add(RepaintVertex(seconds, delta, item, Vertex.usedColor));
					item.vertex.Col = Vertex.usedColor;
				}
				if ((uint)(ActTypes.highlightVertexCurrent & item.ActType) != 0) {
					item.vertex.Storyboard.Children.Add(RepaintVertex(seconds, delta, item, DrawableObject.currentColor));
					item.vertex.Col = DrawableObject.currentColor;
				}
				if ((uint)(ActTypes.highlightEdgeAsBridge & item.ActType) != 0) {
					item.edge.Storyboard.Children.Add(RepaintEdge(seconds, delta, item, Edge.bridgeColor, false));
					item.edge.Col = Edge.bridgeColor;
				}
				if ((uint)(ActTypes.highlightEdgeAsDfsTree & item.ActType) != 0) {
					item.edge.Storyboard.Children.Add(RepaintEdge(seconds, delta, item, Edge.dfsTreeColor, false));
					item.edge.Col = Edge.dfsTreeColor;
				}
				if ((uint)(ActTypes.highlightLabels & item.ActType) != 0) {
					doubleDelta = true;
					foreach (var label in item.labels) {
						var newColor = (label == item.vertex.LabelTin || label == item.vertex.LabelFup) ?
										Vertex.textHighlightedColor1 :
										Vertex.textHighlightedColor2;
						label.Storyboard.Children.Add(RepaintLabel(seconds, delta, newColor, label));
					}
				}
				if ((uint)(ActTypes.updateTin & item.ActType) != 0) {
					item.vertex.LabelTin.Storyboard.Children.Add(
						ChangeLabelContent(seconds, delta, item.vertex.LabelTin,
						String.Format(Vertex.tinTemplate, item.vertex.Num, item.data.newTin)));
				}
				if ((uint)(ActTypes.updateFup & item.ActType) != 0) {
					item.vertex.LabelFup.Storyboard.Children.Add(
						ChangeLabelContent(seconds, delta, item.vertex.LabelFup,
						String.Format(Vertex.fupTemplate, item.vertex.Num, item.data.newFup)));
				}
				if ((uint)(ActTypes.updateChildren & item.ActType) != 0) {
					childrenLabel.Storyboard.Children.Add(
						ChangeLabelContent(seconds, delta, childrenLabel,
						String.Format(childrenTemplate, item.data.newChildren)));
				}
				if ((uint)(ActTypes.highlightCodeLine & item.ActType) != 0) {
					doubleDelta = true;
					var itemu = (ListViewItem)window.codeLines.Items.GetItemAt((int)item.data.lineNum);
					codeStoryboards[(int)item.data.lineNum].Children.Add(RepaintCodeLine(window, seconds, delta, itemu, DrawableObject.currentColor));
				}

				seconds += delta * (doubleDelta ? 2 : 1);

			}
		}

		/// <summary type="AnimationTimeline" dos="private">
		/// Метод для перекраски строки кода алгоритма. 
		/// </summary>
		/// <param name="window"> Ссылка на окно на котором происходит анимация. </param>
		/// <param name="delta"> Задержка между действиями. </param>
		/// <param name="seconds"> Текущий счётчик секунд. </param>
		/// <param name="line"> Ссылка на строку кода, которую надо подсветить. </param>
		/// <param name="newColor"> Новый цвет строки кода. </param>
		/// <param name="reverse"> Флаг, отвечающий должно ли действие автоматически отмениться. </param>
		/// <returns> Возвращает анимационное событие типа AnimationTimeLine, привязаное к переданой строке. </returns>
		private static AnimationTimeline RepaintCodeLine(MainWindow window, int seconds, int delta, ListViewItem line, Color newColor, bool reverse = true) {
			var anim = new ColorAnimation(
				fromValue: Colors.Black,
				toValue: newColor,
				duration: TimeSpan.FromMilliseconds(delta));
			anim.AutoReverse = reverse;
			anim.BeginTime = TimeSpan.FromMilliseconds(seconds);

			Storyboard.SetTarget(anim, line);
			Storyboard.SetTargetProperty(anim, new PropertyPath($"({ListViewItem.ForegroundProperty}).({SolidColorBrush.ColorProperty})"));

			return anim;
		}

		/// <summary type="AnimationTimeline" dos="private">
		/// Метод для изменения контента Lable-а
		/// </summary>
		/// <param name="delta"> Задержка между действиями. </param>
		/// <param name="seconds"> Текущий счётчик секунд. </param>
		/// <param name="label"> Ссылка на объект, который будет анимироваться. </param>
		/// <param name="newValue"> Новое занчение для свойства контент. </param>
		/// <returns> Возвращает анимационное событие типа AnimationTimeLine, привязаное к переданому Label-у. </returns>
		private static AnimationTimeline ChangeLabelContent(int seconds, int delta, ALabel label, string newValue) {
			var anim = new StringAnimationUsingKeyFrames();
			anim.KeyFrames.Add(new DiscreteStringKeyFrame(newValue,
				TimeSpan.FromMilliseconds(seconds + delta)));

			Storyboard.SetTarget(anim, label);
			Storyboard.SetTargetProperty(anim, new PropertyPath(Label.ContentProperty));
			return anim;
		}

		/// <summary type="AnimationTimeline" dos="private">
		/// Метод для изменения цвета Lable-а
		/// </summary>
		/// <param name="newColor"> Новый цвет Label-а. </param>
		/// <param name="reverse"> Флаг, отвечающий должно ли действие автоматически отмениться. </param>
		/// <param name="delta"> Задержка между действиями. </param>
		/// <param name="seconds"> Текущий счётчик секунд. </param>
		/// <param name="label"> Ссылка на объект, который будет анимироваться. </param>
		/// <returns> Возвращает анимационное событие типа AnimationTimeLine, привязаное к переданому Label-у. </returns>
		private static AnimationTimeline RepaintLabel(int seconds, int delta, Color newColor, ALabel label, bool reverse = true) {
			var anim = new ColorAnimation(
				fromValue: Vertex.textColor,
				toValue: newColor,
				duration: TimeSpan.FromMilliseconds(delta)
			);
			anim.AutoReverse = reverse;
			anim.BeginTime = TimeSpan.FromMilliseconds(seconds);

			Storyboard.SetTarget(anim, label);
			Storyboard.SetTargetProperty(anim, new PropertyPath($"({Label.ForegroundProperty}).({SolidColorBrush.ColorProperty})"));

			return anim;
		}

		/// <summary type="AnimationTimeline" dos="private">
		/// Метод для изменения цвета ребра.
		/// </summary>
		/// <param name="newColor"> Новый цвет ребра. </param>
		/// <param name="reverse"> Флаг, отвечающий должно ли действие автоматически отмениться. </param>
		/// <param name="delta"> Задержка между действиями. </param>
		/// <param name="seconds"> Текущий счётчик секунд. </param>
		/// <param name="item"> Ссылка на ребро, которое будет анимировано. </param>
		/// <returns> Возвращает анимационное событие типа AnimationTimeLine, привязаное к переданому ребру. </returns>
		private static AnimationTimeline RepaintEdge(int seconds, int delta, Act item, Color newColor, bool reverse = true) {
			var ed = item.edge;
			var anim = new ColorAnimation(
				fromValue: ed.Col,
				toValue: newColor,
				duration: TimeSpan.FromMilliseconds(delta)
			);
			anim.AutoReverse = reverse;
			anim.BeginTime = TimeSpan.FromMilliseconds(seconds);

			Storyboard.SetTarget(anim, item.edge.Me);
			Storyboard.SetTargetProperty(anim, new PropertyPath($"({Line.StrokeProperty}).({SolidColorBrush.ColorProperty})"));

			return anim;
		}

		/// <summary type="AnimationTimelineAnimationTimeline" dos="private">
		/// Метод для изменения цвета вершины.
		/// </summary>
		/// <param name="newColor"> Новый цвет вершины. </param>
		/// <param name="delta"> Задержка между действиями. </param>
		/// <param name="seconds"> Текущий счётчик секунд. </param>
		/// <param name="item"> Ссылка на вершину, которая будет анимирована. </param>
		/// <returns> Возвращает анимационное событие типа AnimationTimeLine, привязаное к переданой вершине. </returns>
		private static AnimationTimeline RepaintVertex(int seconds, int delta, Act item, Color newColor) {
			var v = item.vertex as Vertex;
			var anim = new ColorAnimation(
				fromValue: v.Col,
				toValue: v.IsCutVertex? Vertex.cutVertexColor : newColor,
				duration: TimeSpan.FromMilliseconds(delta));


			anim.BeginTime = TimeSpan.FromMilliseconds(seconds);

			Storyboard.SetTarget(anim, item.vertex.Me);
			Storyboard.SetTargetProperty(anim, new PropertyPath($"({Ellipse.FillProperty}).({SolidColorBrush.ColorProperty})"));

			return anim;
		}


	}

	/// <summary type="" dos="">
	/// Класс расширения точки.
	/// </summary>
	public static class PointExtention {
		/// <summary type="double" dos="public">
		/// Метод возвращает расстояние между точками.
		/// </summary>
		/// <param name="a"> Первая точка. </param>
		/// <param name="b"> Вторая точка. </param>
		/// <returns> Возвращает расстояние вычисленное по теореме пофигора. </returns>
		public static double Distance(this Point a, Point b) =>
			Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
	}
}
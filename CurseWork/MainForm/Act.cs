using System.Collections.Generic;
using System.Windows.Controls;

namespace MainForm {
	/// <summary type="" dos="">
	/// Перечисление с типами анимируемых действий. Каждый элемент - уникальный флаг-бит. 
	/// </summary>
	public enum ActTypes {
		highlightEdgeCurrent = 0b0000000000000001,
		highlightEdgeNotCurrent = 0b0000000000000010,
		highlightVertexUsed = 0b0000000000000100,
		highlightVertexCurrent = 0b0000000000001000,
		highlightVertexDef = 0b0000000000010000,
		highlightEdgeAsBridge = 0b0000000000100000,
		highlightEdgeAsDfsTree = 0b0000000001000000,
		highlightCutVertex = 0b0000000010000000,
		updateTin = 0b0000000100000000,
		updateFup = 0b0000001000000000,
		updateChildren = 0b0000010000000000,
		highlightLabels = 0b0000100000000000,
		highlightCodeLine = 0b0001000000000000,
		createNewDfsLine = 0b0010000000000000,
		removeNewDfsLine = 0b0100000000000000,
	}

	/// <summary type="" dos="">
	/// Класс события анимации. Хранит информацию об одном анимированном 
	/// шаге и ссылки на все анимируемые объекты.
	/// </summary>
	class Act {
		/// <summary type="ActTypes" dos="private">
		/// Число биты которого отвечают за то, какая анимация будет отображаться.
		/// </summary>
		ActTypes actType;
		/// <summary type="Vertex" dos="public">
		/// Вершина, которая будет анимирована на данном шаге.
		/// </summary>
		public Vertex vertex;
		/// <summary type="Edge" dos="public">
		/// Ребро, которое будет анимировано на данном шаге.
		/// </summary>
		public Edge edge;
		/// <summary type="List{ALabel}" dos="public">
		/// Список Labael-ов, которые будут анимированы на данном шаге.
		/// </summary>
		public List<ALabel> labels;
		/// <summary type="ActData" dos="public">
		/// Дополнительная информация.
		/// </summary>
		public ActData data;

		/// <summary type="" dos="public">
		/// Конструткор для инициализации полей.
		/// </summary>
		/// <param name="actType"> Тип анимационного события. </param>
		/// <param name="objs"> Список анимируемых объектов. </param>
		public Act(ActTypes actType, params object[] objs) {
			ActType = actType;
			labels = new List<ALabel>();
			foreach (var item in objs) {
				vertex = vertex ?? item as Vertex;
				edge = edge ?? item as Edge;
				data = data ?? item as ActData;
				if (item is Label)
					labels.Add(item as ALabel);
			}
		}

		/// <summary type="" dos="public">
		/// Копирующий конструктор.
		/// </summary>
		/// <param name="old"> Старый объект данного типа. </param>
		public Act(Act old) {
			ActType = old.ActType;
			vertex = old.vertex;
			edge = old.edge;
			data = new ActData(old.data);
			labels = old.labels;
		}

		/// <summary type="ActTypes" dos="public">
		/// Свойство для обращения к типу события.
		/// </summary>
		public ActTypes ActType { get => actType; set => actType = value; }

		/// <summary type="Act" dos="public">
		/// Метод для получения обратного события.
		/// </summary>
		/// <returns> Возвращает событие, которое отменяет изменение текущего. </returns>
		public Act GetReverseAct() {
			Act result = new Act(this);
			result.ActType = ActTypes.highlightCodeLine;
			result.data.lineNum = GlobalMembers.prevLine;

			if ((uint)(this.ActType & (
				ActTypes.highlightVertexCurrent |
				ActTypes.highlightVertexDef |
				ActTypes.highlightVertexUsed |
				ActTypes.highlightCutVertex)) != 0) {

				if (result.vertex.Col == DrawableObject.currentColor)
					result.ActType |= ActTypes.highlightVertexCurrent;
				if (result.vertex.Col == Vertex.notUsedColor)
					result.ActType |= ActTypes.highlightVertexDef;
				if (result.vertex.Col == Vertex.usedColor)
					result.ActType |= ActTypes.highlightVertexUsed;
				if (result.vertex.Col == Vertex.cutVertexColor)
					result.ActType |= ActTypes.highlightCutVertex;
			}

			if ((uint)(this.ActType & (
				ActTypes.highlightEdgeAsBridge |
				ActTypes.highlightEdgeAsDfsTree |
				ActTypes.highlightEdgeCurrent |
				ActTypes.highlightEdgeNotCurrent)) != 0) {

				if (result.edge.Col == DrawableObject.currentColor)
					result.ActType |= ActTypes.highlightEdgeCurrent;
				if (result.edge.Col == Edge.bridgeColor)
					result.ActType |= ActTypes.highlightEdgeAsBridge;
				if (result.edge.Col == Edge.dfsTreeColor)
					result.ActType |= ActTypes.highlightEdgeAsDfsTree;
				if (result.edge.Col == Edge.notBridgeColor)
					result.ActType |= ActTypes.highlightEdgeNotCurrent;
			}

			if ((uint)(this.ActType & ActTypes.updateTin) != 0) {
				result.ActType |= ActTypes.updateTin;
				result.data.newTin = int.Parse(((string)vertex.LabelTin.Content).Split('=')[1]);
			}
			if ((uint)(this.ActType & ActTypes.updateFup) != 0) {
				result.ActType |= ActTypes.updateFup;
				result.data.newFup = int.Parse(((string)vertex.LabelFup.Content).Split('=')[1]);
			}
			if ((uint)(this.ActType & ActTypes.updateChildren) != 0) {
				result.ActType |= ActTypes.updateChildren;
				result.data.newChildren = int.Parse(((string)GlobalMembers.childrenLabel.Content).Split('=')[1]);
			}
			if ((uint)(this.ActType & ActTypes.createNewDfsLine) != 0) {
				result.ActType |= ActTypes.removeNewDfsLine;
			}
			if ((uint)(this.ActType & ActTypes.removeNewDfsLine) != 0) {
				result.ActType |= ActTypes.createNewDfsLine;
			}

			return result;

		}

		/// <summary type="" dos="public">
		/// Вложеный класс с дополнительной информацией анимируемого события.
		/// </summary>
		public class ActData {
			/// <summary type="int?" dos="public">
			/// Обновлённое время входа в вершину.
			/// </summary>
			public int? newTin = 0;
			/// <summary type="int?" dos="public">
			/// Обновлённая самая высокая вершина в дереве поиска в глубину, 
			/// до которую можно добраться из текущей.
			/// </summary>
			public int? newFup = 0;
			/// <summary type="int?" dos="public">
			/// Строка кода, которую надо подстветить на данном шаге.
			/// </summary>
			public int? lineNum = 0;
			/// <summary type="int?" dos="public">
			/// Номер родительской вершины.
			/// </summary>
			public int? parentNum = 0;
			/// <summary type="int?" dos="public">
			/// Новое количество детей текущей вершины.
			/// </summary>
			public int? newChildren = 0;

			/// <summary type="" dos="public">
			/// Конструктор для инициализации всех параметров.
			/// </summary>
			/// <param name="newTin"> Новое значение tin. </param>
			/// <param name="newFup"> Новое значение fup. </param>
			/// <param name="lineNum"> Подсвечиваемая строка кода. </param>
			/// <param name="parentNum"> Номер вершины родителя. </param>
			/// <param name="newChildren"> Новое количество детей текущей вершины. </param>
			public ActData(int? newTin = null, int? newFup = null, int? lineNum = null, int? parentNum = null, int? newChildren = null) {
				this.newTin = newTin;
				this.newFup = newFup;
				this.lineNum = lineNum;
				this.parentNum = parentNum;
				this.newChildren = newChildren;
			}

			/// <summary type="" dos="public">
			/// Копирующий конструктор.
			/// </summary>
			/// <param name="old"> Старый экземпляр текущего класса. </param>
			public ActData(ActData old) {
				if (old is null)
					return;
				newTin = old.newTin;
				newFup = old.newFup;
				lineNum = old.lineNum;
				parentNum = old.parentNum;
			}
		}
	}
}
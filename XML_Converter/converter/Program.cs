using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace converter {
	class Elem {
		public string name;
		public string summary;

		public Elem(string name, string summary) {
			this.name = name ?? throw new ArgumentNullException(nameof(name));
			this.summary = summary ?? throw new ArgumentNullException(nameof(summary));
		}
	}
	class Field : Elem {
		public Field(string name, string summary) : base(name, summary) {
		}

		public override string ToString() =>
			$"\tПоле : {name} - " + summary + Environment.NewLine;
	}
	class Meth : Elem {
		public List<Elem> param = new List<Elem>();

		public string returns;

		public Meth(string name, string summary, string returns) : base(name, summary) {
			this.returns = returns;
		}

		public override string ToString() {
			var res = new StringBuilder();
			
			res.Append($"\tМетод : {name} - " + summary + Environment.NewLine + (param.Count == 0? "" : "\tПараметры : " + Environment.NewLine));

			foreach (var item in param) {
				res.Append($"\t\tПараметр : {item.name} - {item.summary}" + Environment.NewLine);
			}
			if (returns != "")
				res.Append(Environment.NewLine + $"\t\tВозвращаемое значение : {returns}" + Environment.NewLine);

			return res.ToString();
		}
	}
	class MyType : Elem{
		public List<Field> fields = new List<Field>();
		public List<Meth> meths = new List<Meth>();

		public MyType(string name, string summary) : base(name, summary) {
		}

		public override string ToString() {
			var res = new StringBuilder();

			res.Append($"Класс : {name} - " + summary + Environment.NewLine);

			foreach (var item in fields) {
				res.Append(item.ToString() + Environment.NewLine);
			}

			foreach (var item in meths) {
				res.Append(item.ToString() + Environment.NewLine);
			}

			return res.ToString();
		}
	}
	class Program {
		static void Main(string[] args) {
			Dictionary<string, MyType> types = new Dictionary<string, MyType>();
			var xDoc = new XmlDocument();
			xDoc.Load("..\\..\\svd.xml");
			var save = new List<XmlElement>();
			foreach (XmlElement item in xDoc.GetElementsByTagName("members")[0].ChildNodes) {
				var nameNode = item.Attributes.GetNamedItem("name");
				string name;
				if (nameNode is null)
					continue;
				else
					name = nameNode.Value;

				char type = name[0];
				switch (type) {
					case 'T':
						var temp = name.Split('.');
						name = temp[temp.Length - 1];
						var sum = item.GetElementsByTagName("summary")[0].InnerText.Trim();
						types.Add(name, new MyType(name, sum));
						break;
					case 'F':
						temp = name.Split('.');
						name = temp[temp.Length - 1];
						sum = item.GetElementsByTagName("summary")[0].InnerText.Trim();
						var className = temp[temp.Length - 2];
						types[className].fields.Add(new Field(name, sum));
						break;
					case 'M':
						var ttemp = name.Split('(');
						temp = ttemp[0].Split('.');
						name = temp[temp.Length - 1];
						if (name == "#ctor")
							name = "Конструктор";
						try {
							name += "(" + ttemp[1];
						}
						catch {
							name += "()";
						}
						sum = item.GetElementsByTagName("summary")[0].InnerText.Trim();
						string ret = "";
						if (item.GetElementsByTagName("returns").Count != 0)
							ret = item.GetElementsByTagName("returns")[0].InnerText.Trim();
						var meth = new Meth(name, sum, ret);
						foreach (XmlElement itemm in item.GetElementsByTagName("param")) {
							meth.param.Add(new Elem(itemm.Attributes.GetNamedItem("name").Value, itemm.InnerText.Trim()));
						}

						className = temp[temp.Length - 2];
						try {
							types[className].meths.Add(meth);
						}
						catch (KeyNotFoundException) {
							types.Add(className, new MyType(className, ""));
							types[className].meths.Add(meth);
						}

						break;
					default:
						break;
				}
			}

			using (StreamWriter sw = new StreamWriter(new FileStream("..\\..\\text.txt", FileMode.Create))) {
				foreach (var item in types) {
					sw.WriteLine(item.Value);
				}
			}
		}
	}
}

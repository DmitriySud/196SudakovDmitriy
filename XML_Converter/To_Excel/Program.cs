﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace converter {
	class Elem {
		public string name;
		public string summary;
		public string mod;
		public string type;

		public Elem(string name, string summary, string mod, string type) {
			this.name = name ?? throw new ArgumentNullException(nameof(name));
			//name = Regex.Replace(name, @"\w+\.", "");
			name = Regex.Replace(name, @"{", "<");
			this.name = Regex.Replace(name, @"}", ">");
			this.summary = summary ?? throw new ArgumentNullException(nameof(summary));
			this.mod = mod;
			type = Regex.Replace(type, @"{", "<");
			this.type = Regex.Replace(type, @"}", ">");
		}
	}
	class Field : Elem {
		public Field(string name, string summary, string mod, string type) : base(name, summary, mod, type) {
		}

		public override string ToString() =>
			$"{mod}\t{name}\t{summary}";
	}
	class Prop : Elem {
		public Prop(string name, string summary, string mod, string type) : base(name, summary, mod, type) {
		}

		public override string ToString() =>
			$"{mod}\t{name}\t{summary}";
	}
	class Meth : Elem {
		public List<Elem> param = new List<Elem>();

		public string returns;

		public Meth(string name, string summary, string returns, string mod, string type) : base(name, summary, mod, type) {
			this.returns = returns;
		}

		public override string ToString() {
			var res = new StringBuilder();

			string shortName = name.Split('(')[0];
			string paramList = name.Split('(')[1].Trim(')');

			res.Append($"{mod}\t{shortName}\t{paramList}\t{summary}\t{returns}");


			return res.ToString();
		}
	}
	class MyType : Elem {
		public List<Field> fields = new List<Field>();
		public List<Meth> meths = new List<Meth>();
		public List<Prop> props = new List<Prop>();

		public MyType(string name, string summary, string mod, string type) : base(name, summary, mod, type) {
		}

		public string WriteClassFields() {
			var res = new StringBuilder();

			res.Append($"Поля Класса : {name}\t" + Environment.NewLine);

			foreach (var item in fields) {
				res.Append(item.ToString() + Environment.NewLine);
			}

			//foreach (var item in meths) {
			//	res.Append(item.ToString() + Environment.NewLine);
			//}

			return res.ToString();
		}
		public string WriteClassProps() {
			var res = new StringBuilder();

			res.Append($"Свойства Класса : {name}\t" + Environment.NewLine);

			foreach (var item in props) {
				res.Append(item.ToString() + Environment.NewLine);
			}

			//foreach (var item in meths) {
			//	res.Append(item.ToString() + Environment.NewLine);
			//}

			return res.ToString();
		}
		public string WriteClassMeths() {
			var res = new StringBuilder();

			res.Append($"Методы Класса : {name}\t" + Environment.NewLine);

			foreach (var item in meths) {
				res.Append(item.ToString() + Environment.NewLine);
			}

			//foreach (var item in meths) {
			//	res.Append(item.ToString() + Environment.NewLine);
			//}

			return res.ToString();
		}
	}
	class Program {
		static void Main(string[] args) {
			Dictionary<string, MyType> types = new Dictionary<string, MyType>();
			var xDoc = new XmlDocument();
			xDoc.Load("..\\..\\Presentation.xml");
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
						var temp = name.Split(':');
						name = temp[temp.Length - 1];
						
						var sum = item.GetElementsByTagName("summary")[0].InnerText.Trim();
						types.Add(name, new MyType(name, sum, "", ""));
						break;
					case 'F':
						temp = name.Split('.');
						var Fname = temp[temp.Length - 1];
						XmlNode sumEl = item.GetElementsByTagName("summary")[0];
						sum = sumEl.InnerText.Trim();
						var className = String.Join(".", name.Split(':')[1].Split('.').Where(x => x != Fname));
						var modAttr = item.GetElementsByTagName("dos")[0];
						var typeAttr = sumEl.Attributes["type"];
						types[className].fields.Add(new Field(Fname, sum, modAttr is null ? "" : modAttr.FirstChild.Value, typeAttr is null ? "" : typeAttr.Value)); ;
						break;
					case 'P':
						temp = name.Split('.');
						var Pname = temp[temp.Length - 1];
						sumEl = item.GetElementsByTagName("summary")[0];
						sum = sumEl.InnerText.Trim();
						className = String.Join(".", name.Split(':')[1].Split('.').Where(x => x != Pname));
						modAttr = item.GetElementsByTagName("dos")[0];
						typeAttr = sumEl.Attributes["type"];
						types[className].props.Add(new Prop(Pname, sum, modAttr is null? "" : modAttr.FirstChild.Value, typeAttr is null ? "" : typeAttr.Value));
						break;
					case 'M':
						name = Regex.Replace(name, "#ctor", "Конструктор");
						if (!name.Contains('('))
							name += "()";
						var ttemp = name.Split('(');
						temp = ttemp[0].Split('.');
						var Mname = temp[temp.Length - 1];
						try {
							Mname += "(" + ttemp[1];
						}
						catch {
							Mname += "()";
						}
						sumEl = item.GetElementsByTagName("summary")[0];
						sum = sumEl?.InnerText.Trim()?? "";
						modAttr = item.GetElementsByTagName("dos")[0];
						typeAttr = sumEl?.Attributes["type"];
						string ret = "";
						if (item.GetElementsByTagName("returns").Count != 0)
							ret = item.GetElementsByTagName("returns")[0].InnerText.Trim();
						var meth = new Meth(Mname, sum, ret, modAttr is null ? "" : modAttr.FirstChild.Value, typeAttr is null ? "" : typeAttr.Value);
						foreach (XmlElement itemm in item.GetElementsByTagName("param")) {
							meth.param.Add(new Elem(itemm.Attributes.GetNamedItem("name").Value, itemm.InnerText.Trim(),"",""));
						}

						className = String.Join(".", name.Split('(')[0].Split(':')[1].Split('.').Where(x => x != Mname.Split('(')[0]));
						try {
							types[className].meths.Add(meth);
						}
						catch (KeyNotFoundException) {
							types.Add(className, new MyType(className, "", "", ""));
							types[className].meths.Add(meth);
						}

						break;
					default:
						break;
				}
			}

			using (StreamWriter sw = new StreamWriter(new FileStream("..\\..\\text.txt", FileMode.Create))) {
				foreach (var item in types) {
					sw.WriteLine(item.Value.WriteClassFields()+Environment.NewLine);
					sw.WriteLine(item.Value.WriteClassProps()+Environment.NewLine);
					sw.WriteLine(item.Value.WriteClassMeths()+Environment.NewLine);
				}
			}
		}
	}
}

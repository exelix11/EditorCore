using EditorCore;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ByamlExt.Byaml
{
	static class XmlConverter
	{
		public static string ToXml(BymlFileData data)
		{
			CustomStringWriter str = new CustomStringWriter(Encoding.GetEncoding(932));
			XmlTextWriter xr;
			xr = new XmlTextWriter(str);
			xr.Formatting = System.Xml.Formatting.Indented;
			xr.WriteStartDocument();
			xr.WriteStartElement("Root");
			xr.WriteStartElement("isBigEndian");
			xr.WriteAttributeString("Value", (data.byteOrder == Syroot.BinaryData.ByteOrder.BigEndian).ToString());
			xr.WriteEndElement();
			xr.WriteStartElement("BymlFormatVersion");
			xr.WriteAttributeString("Value", data.Version.ToString());
			xr.WriteEndElement();
			xr.WriteStartElement("SupportPaths");
			xr.WriteAttributeString("Value", data.SupportPaths.ToString());
			xr.WriteEndElement();

			xr.WriteStartElement("BymlRoot");
			WriteNode(data.RootNode, null, xr);		
			xr.WriteEndElement();

			xr.WriteEndElement();
			xr.Close();
			return str.ToString();
		}

		public static BymlFileData ToByml(string xmlString)
		{
			BymlFileData ret = new BymlFileData();
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString);
			XmlNode n = xml.SelectSingleNode("/Root/isBigEndian");
			ret.byteOrder = n.Attributes["Value"].Value.ToLower() == "true" ? ByteOrder.BigEndian : ByteOrder.LittleEndian;
			n = xml.SelectSingleNode("/Root/BymlFormatVersion");
			ret.Version = ushort.Parse(n.Attributes["Value"].Value);
			n = xml.SelectSingleNode("/Root/SupportPaths");
			ret.SupportPaths = n.Attributes["Value"].Value.ToLower() == "true";

			n = xml.SelectSingleNode("/Root/BymlRoot");
			if (n.ChildNodes.Count != 1) throw new Exception("A byml can have only one root");
			ret.RootNode = ParseNode(n.FirstChild);

			return ret;
		}

		#region XmlWriting
		static void WriteNode(dynamic node, string name, XmlTextWriter xr)
		{
			if (node is IList<dynamic>) WriteArrNode((IList<dynamic>)node, name, xr);
			else if (node is IDictionary<string, dynamic>) WriteDictNode((IDictionary<string, dynamic>)node, name, xr);
			else
			{
				xr.WriteStartElement(GetNodeName(node));
				if (name != null)
					xr.WriteAttributeString("Name", name);
				xr.WriteAttributeString("Value", node.ToString());
				xr.WriteEndElement();
			}
		}

		static void WriteArrNode(IList<dynamic> node, string name,XmlTextWriter xr)
		{
			xr.WriteStartElement(GetNodeName(node));
			if (name != null)
				xr.WriteAttributeString("Name", name);
			for (int i = 0; i < node.Count; i++)
			{
				WriteNode(node[i], null, xr);
			}
			xr.WriteEndElement();
		}

		static void WriteDictNode(IDictionary<string,dynamic> node, string name, XmlTextWriter xr)
		{
			xr.WriteStartElement(GetNodeName(node));
			if (name != null)
				xr.WriteAttributeString("Name", name);
			var keys = node.Keys.ToArray();
			for (int i = 0; i < keys.Length; i++)
			{
				WriteNode(node[keys[i]], keys[i], xr);
			}
			xr.WriteEndElement();
		}

		static string GetNodeName(dynamic node) =>
			"T" + ((byte)ByamlFile.GetNodeType(node)).ToString();
		#endregion

		#region XmlReading

		static dynamic ParseNode(XmlNode n)
		{
			ByamlNodeType nodeType = (ByamlNodeType)byte.Parse(n.Name.Substring(1));
			switch (nodeType)
			{
				case ByamlNodeType.Array:
					return ParseArrNode(n);
				case ByamlNodeType.Dictionary:
					return ParseDictNode(n);
				default:
					{
						Type T = nodeType.GetInstanceType();
						return ByamlTypeHelper.ConvertValue(T,n.Attributes["Value"].Value);
					}
			}
		}

		static IDictionary<string, dynamic> ParseDictNode(XmlNode n)
		{
			Dictionary<string, dynamic> res = new Dictionary<string, dynamic>();
			for (int i = 0; i < n.ChildNodes.Count; i++)
			{
				var c = n.ChildNodes[i];
				res.Add(c.Attributes["Name"].Value, ParseNode(c));
			}
			return res;
		}

		static IList<dynamic> ParseArrNode(XmlNode n)
		{
			List<dynamic> res = new List<dynamic>();
			for (int i = 0; i < n.ChildNodes.Count; i++)
			{
				res.Add(ParseNode(n.ChildNodes[i]));
			}
			return res;
		}

		#endregion
	}
}

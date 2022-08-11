using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RescoCLI.Helpers
{
   

	[XmlRoot(ElementName = "DisplayName")]
	public class DisplayName
	{

		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "Language")]
	public class LocalizationResult
	{

		[XmlElement(ElementName = "DisplayName")]
		public List<DisplayName> DisplayName { get; set; }

		[XmlAttribute(AttributeName = "LCID")]
		public int LCID { get; set; }

		[XmlAttribute(AttributeName = "xsi")]
		public string Xsi { get; set; }

		[XmlAttribute(AttributeName = "xsd")]
		public string Xsd { get; set; }

		[XmlText]
		public string Text { get; set; }
	}
}

using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;


namespace RescoCLI.Helpers
{
    public class PluginRegistrationRequest

    {
        public string Description { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string PluginData { get; set; }
        public string ToXML()
        {
            using var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(stringwriter, this, ns);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(stringwriter.ToString());
            foreach (XmlNode node in doc)
            {
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                {
                    doc.RemoveChild(node);
                }
            }

            return doc.OuterXml;
        }
    }
}
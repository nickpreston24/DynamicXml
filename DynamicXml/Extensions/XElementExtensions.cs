using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DynamicXml.Extensions
{
    public static partial class DynamicExtensions
    {
        public static XElement GetFirstElement(this XElement xElement, string tag) => xElement.Elements(tag).FirstOrDefault() ?? xElement;

        public static XElement GetFirstDescendant(this XElement xElement, string tag) => xElement.Descendants(tag).FirstOrDefault() ?? xElement;

        public static void AppendNode<T>(this XmlElement root, string name, T value)
        {
            XmlDocument doc = root.OwnerDocument;
            if (doc != null)
            {
                XmlElement code = doc.CreateElement(name);
                XmlText codeText = doc.CreateTextNode(value.ToString());
                root.AppendChild(code);
                code.AppendChild(codeText);
            }
        }
    }
}

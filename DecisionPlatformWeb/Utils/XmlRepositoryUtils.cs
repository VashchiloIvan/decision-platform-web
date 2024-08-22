using System.Xml;
using System.Xml.Serialization;

namespace DecisionPlatformWeb.Utils;

public static class XmlRepositoryUtils
{
    public static XmlNode ObjectToXml<T>(XmlDocument document, T obj)
    {
        using var stringwriter = new StringWriter();
        
        var xsn = new XmlSerializerNamespaces();
        xsn.Add(string.Empty, string.Empty);
            
        var serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(stringwriter, obj, xsn);
            
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(stringwriter.ToString());
            
        return document.ImportNode(doc.ChildNodes[1] ?? throw new InvalidOperationException("Failed to serialize object"), true);
    }

    public static T XmlNodeToObject<T>(XmlNode node, string nodeName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(nodeName));
        using XmlNodeReader reader = new XmlNodeReader(node);
        return (T)serializer.Deserialize(reader) ?? throw new InvalidOperationException("Failed to deserialize object");
    }
}
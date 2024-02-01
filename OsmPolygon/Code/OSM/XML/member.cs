
namespace OSM.API.v0_6.XML
{

    [System.Xml.Serialization.XmlRoot(ElementName = "member")]
    public class Member
    {
        [System.Xml.Serialization.XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "ref")]
        public string Ref { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "role")]
        public string Role { get; set; }
    }

}
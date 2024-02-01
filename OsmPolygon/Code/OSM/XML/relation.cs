
namespace OSM.API.v0_6.XML
{

    // https://www.openstreetmap.org/api/0.6/relation/2866715

    //<osm version = "0.6" generator="CGImap 0.8.10 (3350604 spike-08.openstreetmap.org)" copyright="OpenStreetMap and contributors" attribution="http://www.openstreetmap.org/copyright" license="http://opendatacommons.org/licenses/odbl/1-0/">
    //  <relation id = "2866715" visible="true" version="3" changeset="121445682" timestamp="2022-05-24T21:14:20Z" user="Wikilux" uid="1894634">
    //    <member type = "way" ref="216354955" role="outer"/>
    //    <member type = "way" ref="216354990" role="outer"/>
    //    <member type = "way" ref="216354972" role="outer"/>
    //    <tag k = "addr:city" v="St-Maurice"/>
    //    <tag k = "addr:country" v="CH"/>
    //    <tag k = "addr:housenumber" v="74"/>
    //    <tag k = "addr:postcode" v="1890"/>
    //    <tag k = "addr:street" v="Grand-Rue"/>
    //    <tag k = "building" v="yes"/>
    //    <tag k = "type" v="multipolygon"/>
    //  </relation>
    //</osm>


    [System.Xml.Serialization.XmlRoot(ElementName = "relation")]
    public class Relation
    {
        [System.Xml.Serialization.XmlElement(ElementName = "member")]
        public System.Collections.Generic.List<Member> Member { get; set; }
        [System.Xml.Serialization.XmlElement(ElementName = "tag")]
        public System.Collections.Generic.List<Tag> Tag { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "visible")]
        public string Visible { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "changeset")]
        public string Changeset { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "timestamp")]
        public string Timestamp { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "user")]
        public string User { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "uid")]
        public string Uid { get; set; }
    }

    [System.Xml.Serialization.XmlRoot(ElementName = "osm")]
    public class OsmRelationXml
    {
        [System.Xml.Serialization.XmlElement(ElementName = "relation")]
        public Relation Relation { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "generator")]
        public string Generator { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "copyright")]
        public string Copyright { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "attribution")]
        public string Attribution { get; set; }
        [System.Xml.Serialization.XmlAttribute(AttributeName = "license")]
        public string License { get; set; }

        public static OsmRelationXml FromUrl(string url)
        {
            return Tools.XML.Serialization.DeserializeXmlFromUrl<OsmRelationXml>(url);
        }

    }


}

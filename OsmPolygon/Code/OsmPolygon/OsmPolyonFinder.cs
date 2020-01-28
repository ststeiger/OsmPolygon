
using Dapper;

namespace OsmPolygon
{


    class BuildingToGeoCode
    {
        public System.Guid SO_UID;
        public System.Guid GB_UID;
        public string GB_Nr;
        public string GB_Bezeichnung;
        public string GB_Adresse;
        public decimal GB_GM_Lat;
        public decimal GB_GM_Lng;
    }


    class OsmPolyonFinder
    {


        public static string GetConnectionString()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();

            csb.DataSource = System.Environment.MachineName;
            if (System.StringComparer.OrdinalIgnoreCase.Equals("COR", System.Environment.UserDomainName))
            {
                csb.DataSource += @"\SqlExpress";
            }

            csb.DataSource = SecretManager.GetSecret<string>("DefaultDataSource");
            csb.InitialCatalog = SecretManager.GetSecret<string>("DefaultCatalog");
            
            csb.IntegratedSecurity = false;


            csb.IntegratedSecurity = System.StringComparer.OrdinalIgnoreCase.Equals(System.Environment.UserDomainName, "COR");
            if (!csb.IntegratedSecurity)
            {
                csb.UserID = SecretManager.GetSecret<string>("DefaultDbUser");
                csb.Password = SecretManager.GetSecret<string>("DefaultDbPassword");
            }

            return csb.ToString();
        }


        static bool isClockwise(GeoApis.LatLng[] poly)
        {
            decimal sum = 0;

            for (int i = 0; i < poly.Length - 1; i++)
            {
                GeoApis.LatLng cur = poly[i], next = poly[i + 1];
                sum += (next.lat - cur.lat) * (next.lng + cur.lng);
            } // Next i 

            return sum > 0;
        } // End Function isClockwise 


        // MSSQL is CLOCKWISE (MS-SQL wants the polygon points in clockwise sequence) 
        static GeoApis.LatLng[] toClockWise(GeoApis.LatLng[] poly)
        {
            if (!isClockwise(poly))
                GeoApis.ArrayExtensions.Reverse(poly);

            return poly;
        } // End Function toClockWise 


        // OSM is COUNTER-clockwise  (OSM wants the polygon points in counterclockwise sequence) 
        static GeoApis.LatLng[] toCounterClockWise(GeoApis.LatLng[] poly)
        {
            if (isClockwise(poly))
                GeoApis.ArrayExtensions.Reverse(poly);

            return poly;
        } // End Function toCounterClockWise 


        public static string CreatePolygon(GeoApis.LatLng[] latLongs)
        {
            //POLYGON ((73.232821 34.191819,73.233755 34.191942,73.233653 34.192358,73.232843 34.192246,73.23269 34.191969,73.232821 34.191819))
            string polyString = "";

            // MS-SQL polygon absolutely wants to be clockwise...
            // Don't copy array, just switch direction if necessary 
            if (isClockwise(latLongs))
            {
                for (int i = 0; i < latLongs.Length; ++i)
                {
                    if (i != 0)
                        polyString += ",";

                    polyString += latLongs[i].lng + " " + latLongs[i].lat; // + ",";
                } // Next i 
            }
            else
            {
                for (int i = latLongs.Length - 1; i > -1; --i)
                {
                    if (i != latLongs.Length - 1)
                        polyString += ",";

                    polyString += latLongs[i].lng + " " + latLongs[i].lat; // + ",";
                } // Next i 
            }

            polyString = "POLYGON((" + polyString + "))";
            return polyString;
        } // End Function CreatePolygon 


        public static string CreateSqlPolygon(GeoApis.LatLng[] latLongs)
        {
            string s = "geography::STPolyFromText('" + CreatePolygon(latLongs) + "', 4326)";
            return s;
        } // End Function CreateSqlPolygon 


        private static System.Random s_rnd = new System.Random();

        public static GeoApis.Polygon GetNearestBuildingPolygon(decimal latitide, decimal longitude)
        {
            OpenToolkit.Mathematics.DecimalVector2 geoPoint = new OpenToolkit.Mathematics.DecimalVector2(latitide, longitude);

            GeoApis.LatLngBounds bounds = GeoApis.LatLngBounds.FromPoint(
                new GeoApis.LatLng(latitide, longitude)
                , 1000
            ); // this is, radius = 500m

            decimal area = bounds.BoundsArea;
            if (area > 0.25m)
            {
                System.Console.WriteLine("The maximum bbox size is 0.25, and your request was too large.\nEither request a smaller area, or use planet.osm.");
                return null;
            } // End if (area > 0.25m) 


            string xml = null;
#if fromFile
            xml = System.IO.File.ReadAllText(@"D:\Stefan.Steiger\Desktop\map.osm.xml", System.Text.Encoding.UTF8);
#else
            const string OSM_API_VERSION = "0.6";
            // string url = "https://www.openstreetmap.org/api/0.6/map?bbox=8.626273870468141,47.69679769756054,8.636573553085329,47.700530864557194&no_cache=1562588642802";
            string url = "https://www.openstreetmap.org/api/" + OSM_API_VERSION + "/map?bbox=" + bounds.ToBBoxString();

            string[] proxyList = ProxyHelper.GetProxyArray();
            string proxy = proxyList[s_rnd.Next(0, proxyList.Length)];


            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                // wc.Proxy = new System.Net.WebProxy(proxy);

                xml = wc.DownloadString(url);
            } // End Using wc 
#endif

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);

            System.Xml.XmlNodeList nodes = doc.SelectNodes("//node");


            System.Collections.Generic.Dictionary<string, GeoApis.LatLng> nodeDictionary =
                new System.Collections.Generic.Dictionary<string, GeoApis.LatLng>(
                    System.StringComparer.InvariantCultureIgnoreCase
            );

            System.Collections.Generic.Dictionary<string, GeoApis.Polygon> buildingPolygonDictionary =
                new System.Collections.Generic.Dictionary<string, GeoApis.Polygon>(
                    System.StringComparer.InvariantCultureIgnoreCase
            );


            foreach (System.Xml.XmlElement node in nodes)
            {
                string id = node.GetAttribute("id");
                string nodeLat = node.GetAttribute("lat");
                string nodeLong = node.GetAttribute("lon");

                decimal dlat = 0;
                decimal dlong = 0;
                decimal.TryParse(nodeLat, out dlat);
                decimal.TryParse(nodeLong, out dlong);

                nodeDictionary[id] = new GeoApis.LatLng(dlat, dlong);
            } // Next node 


            // https://stackoverflow.com/questions/1457638/xpath-get-nodes-where-child-node-contains-an-attribute
            // querySelectorAll('way tag[k="building"]')
            System.Xml.XmlNodeList buildings = doc.SelectNodes("//way[tag/@k=\"building\"]");
            foreach (System.Xml.XmlElement building in buildings)
            {
                System.Collections.Generic.List<GeoApis.LatLng> lsPolygonPoints = 
                    new System.Collections.Generic.List<GeoApis.LatLng>();

                System.Xml.XmlNodeList buildingNodes = building.SelectNodes("./nd");
                foreach (System.Xml.XmlElement buildingNode in buildingNodes)
                {
                    string reff = buildingNode.GetAttribute("ref");
                    lsPolygonPoints.Add(nodeDictionary[reff]);
                } // Next buildingNode 

                GeoApis.LatLng[] polygonPoints = toCounterClockWise(lsPolygonPoints.ToArray());
                string id = building.GetAttribute("id");

                GeoApis.Polygon poly = new GeoApis.Polygon(polygonPoints);
                poly.OsmId = id;

                buildingPolygonDictionary[id] = poly;
            } // Next building 

            // System.Console.WriteLine(buildingPolygonDictionary);



            decimal? min = null;
            string uid = null;

            foreach (System.Collections.Generic.KeyValuePair<string, GeoApis.Polygon> kvp in buildingPolygonDictionary)
            {
                decimal minDist = kvp.Value.GetMinimumDistance(geoPoint);

                if (min.HasValue)
                {
                    if (minDist < min.Value)
                    {
                        min = minDist;
                        uid = kvp.Key;
                    } // End if (minDist < min.Value)
                }
                else
                {
                    uid = kvp.Key;
                    min = minDist;
                }

            } // Next kvp 

            return buildingPolygonDictionary[uid];
        } // End Sub 


        public static void GetAndInsertBuildingPolygon()
        {
            string sql = @"
INSERT INTO T_ZO_Objekt_Wgs84Polygon
(
	 ZO_OBJ_WGS84_UID
	,ZO_OBJ_WGS84_GB_UID
	,ZO_OBJ_WGS84_SO_UID
	,ZO_OBJ_WGS84_Sort
	,ZO_OBJ_WGS84_GM_Lat
	,ZO_OBJ_WGS84_GM_Lng
)
SELECT 
	 NEWID() ZO_OBJ_WGS84_UID -- uniqueidentifier
	,@gb_uid AS ZO_OBJ_WGS84_GB_UID -- uniqueidentifier
	,NULL AS ZO_OBJ_WGS84_SO_UID -- uniqueidentifier
	,@i ZO_OBJ_WGS84_Sort -- int
	,@lat ZO_OBJ_WGS84_GM_Lat -- decimal(23,20)
	,@lng ZO_OBJ_WGS84_GM_Lng -- decimal(23,20)
; 
";

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();


            ConnectionFactory fac = new ConnectionFactory(GetConnectionString());


            using (System.Data.Common.DbConnection connection = fac.Connection)
            {
                System.Collections.Generic.List<BuildingToGeoCode> ls =
                    System.Linq.Enumerable.ToList(connection.Query<BuildingToGeoCode>(
                        "GetGbOsmPolygon.sql"
                        , typeof(OsmPolyonFinder)
                    )
                );

                foreach (BuildingToGeoCode building in ls)
                {
                    System.Threading.Thread.Sleep(4000);

                    GeoApis.Polygon nearestBuilding = GetNearestBuildingPolygon(building.GB_GM_Lat, building.GB_GM_Lng);
                    if (nearestBuilding == null)
                        continue;
                    System.Console.WriteLine(nearestBuilding);
                    System.Console.WriteLine(nearestBuilding.OsmId); // 218003784

                    GeoApis.LatLng[] msPoints = nearestBuilding.ToClockWiseLatLngPoints();
                    string createPolygon = CreateSqlPolygon(msPoints);
                    System.Console.WriteLine(sql);

                    //SELECT 
                    //	 geography::STPolyFromText('POLYGON((7.7867531 46.9361500,7.7869622 46.9361188,7.7869515 46.9360856,7.7869952 46.9360793,7.7870059 46.9361123,7.7870300 46.9361087,7.7870312 46.9361124,7.7870944 46.9361028,7.7870933 46.9360991,7.7872340 46.9360778,7.7873147 46.9363299,7.7871740 46.9363510,7.7871728 46.9363473,7.7871099 46.9363568,7.7871110 46.9363605,7.7868341 46.9364021,7.7867531 46.9361500))', 4326)
                    //	,geometry::STPolyFromText('POLYGON((7.7867531 46.9361500,7.7869622 46.9361188,7.7869515 46.9360856,7.7869952 46.9360793,7.7870059 46.9361123,7.7870300 46.9361087,7.7870312 46.9361124,7.7870944 46.9361028,7.7870933 46.9360991,7.7872340 46.9360778,7.7873147 46.9363299,7.7871740 46.9363510,7.7871728 46.9363473,7.7871099 46.9363568,7.7871110 46.9363605,7.7868341 46.9364021,7.7867531 46.9361500))', 4326)

                    //	-- Geometry is BAD for area
                    //	,geography::STPolyFromText('POLYGON((7.7867531 46.9361500,7.7869622 46.9361188,7.7869515 46.9360856,7.7869952 46.9360793,7.7870059 46.9361123,7.7870300 46.9361087,7.7870312 46.9361124,7.7870944 46.9361028,7.7870933 46.9360991,7.7872340 46.9360778,7.7873147 46.9363299,7.7871740 46.9363510,7.7871728 46.9363473,7.7871099 46.9363568,7.7871110 46.9363605,7.7868341 46.9364021,7.7867531 46.9361500))', 4326).STArea() AS geogArea 
                    //	,geometry::STPolyFromText('POLYGON((7.7867531 46.9361500,7.7869622 46.9361188,7.7869515 46.9360856,7.7869952 46.9360793,7.7870059 46.9361123,7.7870300 46.9361087,7.7870312 46.9361124,7.7870944 46.9361028,7.7870933 46.9360991,7.7872340 46.9360778,7.7873147 46.9363299,7.7871740 46.9363510,7.7871728 46.9363473,7.7871099 46.9363568,7.7871110 46.9363605,7.7868341 46.9364021,7.7867531 46.9361500))', 4326).STArea() AS geomArea 
                    //";



                    GeoApis.LatLng[] osmPoints = nearestBuilding.ToCounterClockWiseLatLngPoints();

                    string sql2 = "DELETE FROM T_ZO_Objekt_Wgs84Polygon WHERE ZO_OBJ_WGS84_GB_UID = @gb_uid; ";
                    connection.Execute(sql2, new { gb_uid = building.GB_UID });


                    for (int i = 0; i < osmPoints.Length; ++i)
                    {
                        connection.Execute(sql, 
                            new
                            {
                                gb_uid = building.GB_UID,
                                i = i,
                                lat = osmPoints[i].lat,
                                lng = osmPoints[i].lng
                            }
                        );

                    } // Next i 

                } // Next building 
                
            } // End Using connection 

        } // End Sub GetAndInsertBuildingPolygon


    } // End Class OsmPolyonFinder 


} // End Namespace 

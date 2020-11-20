
// using OsmPolygon.EsriConverter;


using System.Diagnostics;
using System.Linq;
using NetTopologySuite.Geometries;

namespace OsmPolygon
{


    class Unionizer
    {


        public static void Test()
        {
            /*
            ulong[] way_ids = new ulong[] {
                // 95551570, 104279261, 104278783, 95551575, 104278691
                95551574, 104279357, 95551561, 95551579, 95551559, 95551556
            };

            System.Collections.Generic.List<System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>> ls = new System.Collections.Generic.List<System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>>();

            foreach (ulong way_id in way_ids)
            {
                ls.Add(OSM.API.v0_6.Polygon.GetPointList(way_id.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ls);
            System.Console.WriteLine(json);


            System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> lsUnionPolygonPoints = GetUnionPolygon(ls);
            string insertString = CreateInsertScript(lsUnionPolygonPoints);
            System.Console.WriteLine(insertString);
            */


            // SET @GB_UID = '1A23FD48-406E-4D20-8693-4B8786234D37'
            string a = "[[{\"Latitude\":47.0435701,\"Longitude\":8.3211035},{\"Latitude\":47.0433179,\"Longitude\":8.3214567},{\"Latitude\":47.0431221,\"Longitude\":8.3211557},{\"Latitude\":47.0433743,\"Longitude\":8.3208025},{\"Latitude\":47.0435701,\"Longitude\":8.3211035}],[{\"Latitude\":47.0432071,\"Longitude\":8.3214196},{\"Latitude\":47.0431851,\"Longitude\":8.3213865},{\"Latitude\":47.0432053,\"Longitude\":8.3213582},{\"Latitude\":47.0431253,\"Longitude\":8.3212348},{\"Latitude\":47.0429550,\"Longitude\":8.3214726},{\"Latitude\":47.0429757,\"Longitude\":8.3215046},{\"Latitude\":47.0429430,\"Longitude\":8.3215503},{\"Latitude\":47.0429219,\"Longitude\":8.3215177},{\"Latitude\":47.0428485,\"Longitude\":8.3216201},{\"Latitude\":47.0428713,\"Longitude\":8.3216552},{\"Latitude\":47.0428418,\"Longitude\":8.3216964},{\"Latitude\":47.0428117,\"Longitude\":8.3217385},{\"Latitude\":47.0427892,\"Longitude\":8.3217038},{\"Latitude\":47.0427201,\"Longitude\":8.3218004},{\"Latitude\":47.0427483,\"Longitude\":8.3218440},{\"Latitude\":47.0426922,\"Longitude\":8.3219224},{\"Latitude\":47.0426725,\"Longitude\":8.3218920},{\"Latitude\":47.0424997,\"Longitude\":8.3221332},{\"Latitude\":47.0425937,\"Longitude\":8.3222748},{\"Latitude\":47.0427034,\"Longitude\":8.3221208},{\"Latitude\":47.0432071,\"Longitude\":8.3214196}],[{\"Latitude\":47.0434380,\"Longitude\":8.3217293},{\"Latitude\":47.0434675,\"Longitude\":8.3216879},{\"Latitude\":47.0433343,\"Longitude\":8.3214835},{\"Latitude\":47.0432882,\"Longitude\":8.3215481},{\"Latitude\":47.0432632,\"Longitude\":8.3215098},{\"Latitude\":47.0427628,\"Longitude\":8.3222120},{\"Latitude\":47.0427034,\"Longitude\":8.3221208},{\"Latitude\":47.0425937,\"Longitude\":8.3222748},{\"Latitude\":47.0426827,\"Longitude\":8.3224114},{\"Latitude\":47.0426562,\"Longitude\":8.3224484},{\"Latitude\":47.0427012,\"Longitude\":8.3225175},{\"Latitude\":47.0427211,\"Longitude\":8.3225480},{\"Latitude\":47.0427451,\"Longitude\":8.3225144},{\"Latitude\":47.0427621,\"Longitude\":8.3225405},{\"Latitude\":47.0427998,\"Longitude\":8.3224876},{\"Latitude\":47.0428145,\"Longitude\":8.3225101},{\"Latitude\":47.0428878,\"Longitude\":8.3224072},{\"Latitude\":47.0429668,\"Longitude\":8.3222964},{\"Latitude\":47.0429816,\"Longitude\":8.3223191},{\"Latitude\":47.0430561,\"Longitude\":8.3222146},{\"Latitude\":47.0430413,\"Longitude\":8.3221918},{\"Latitude\":47.0431042,\"Longitude\":8.3221040},{\"Latitude\":47.0433787,\"Longitude\":8.3217183},{\"Latitude\":47.0433624,\"Longitude\":8.3216933},{\"Latitude\":47.0433897,\"Longitude\":8.3216550},{\"Latitude\":47.0434380,\"Longitude\":8.3217293}],[{\"Latitude\":47.0427621,\"Longitude\":8.3225405},{\"Latitude\":47.0427451,\"Longitude\":8.3225144},{\"Latitude\":47.0427211,\"Longitude\":8.3225480},{\"Latitude\":47.0427012,\"Longitude\":8.3225175},{\"Latitude\":47.0426384,\"Longitude\":8.3226058},{\"Latitude\":47.0426248,\"Longitude\":8.3225850},{\"Latitude\":47.0424846,\"Longitude\":8.3227821},{\"Latitude\":47.0425388,\"Longitude\":8.3228651},{\"Latitude\":47.0425523,\"Longitude\":8.3228462},{\"Latitude\":47.0425687,\"Longitude\":8.3228713},{\"Latitude\":47.0425524,\"Longitude\":8.3228942},{\"Latitude\":47.0426507,\"Longitude\":8.3230447},{\"Latitude\":47.0426899,\"Longitude\":8.3231048},{\"Latitude\":47.0427439,\"Longitude\":8.3230289},{\"Latitude\":47.0427287,\"Longitude\":8.3230056},{\"Latitude\":47.0427468,\"Longitude\":8.3229802},{\"Latitude\":47.0427631,\"Longitude\":8.3230052},{\"Latitude\":47.0429500,\"Longitude\":8.3227426},{\"Latitude\":47.0428962,\"Longitude\":8.3226601},{\"Latitude\":47.0428670,\"Longitude\":8.3227011},{\"Latitude\":47.0427621,\"Longitude\":8.3225405}],[{\"Latitude\":47.0434675,\"Longitude\":8.3216879},{\"Latitude\":47.0434380,\"Longitude\":8.3217293},{\"Latitude\":47.0434812,\"Longitude\":8.3217955},{\"Latitude\":47.0435032,\"Longitude\":8.3217646},{\"Latitude\":47.0435806,\"Longitude\":8.3218833},{\"Latitude\":47.0435465,\"Longitude\":8.3219311},{\"Latitude\":47.0435753,\"Longitude\":8.3219753},{\"Latitude\":47.0435954,\"Longitude\":8.3220062},{\"Latitude\":47.0437069,\"Longitude\":8.3218497},{\"Latitude\":47.0438841,\"Longitude\":8.3216010},{\"Latitude\":47.0438316,\"Longitude\":8.3215204},{\"Latitude\":47.0438191,\"Longitude\":8.3215380},{\"Latitude\":47.0438010,\"Longitude\":8.3215103},{\"Latitude\":47.0438141,\"Longitude\":8.3214919},{\"Latitude\":47.0437713,\"Longitude\":8.3214261},{\"Latitude\":47.0437605,\"Longitude\":8.3214412},{\"Latitude\":47.0437437,\"Longitude\":8.3214154},{\"Latitude\":47.0437557,\"Longitude\":8.3213985},{\"Latitude\":47.0436991,\"Longitude\":8.3213115},{\"Latitude\":47.0435217,\"Longitude\":8.3215604},{\"Latitude\":47.0435392,\"Longitude\":8.3215873},{\"Latitude\":47.0434675,\"Longitude\":8.3216879}]]";



            //SELECT 
            //	 ZO_OBJ_WGS84_GM_Lat AS Latitude 	
            //	,ZO_OBJ_WGS84_GM_Lng AS Longitude 
            //FROM T_ZO_Objekt_Wgs84Polygon WHERE ZO_OBJ_WGS84_GB_UID = 'C0F5CAD2-6BA8-4390-A26C-C6682D1DE4F2' 
            //ORDER BY ZO_OBJ_WGS84_Sort 

            //FOR JSON AUTO

            // Kompetenzzentrum Domicil
            a = "[ ";
            a += " [{\"Latitude\":46.94976570000000000000,\"Longitude\":7.39213610000000000000},{\"Latitude\":46.94991580000000000000,\"Longitude\":7.39203950000000000000},{\"Latitude\":46.94993780000000000000,\"Longitude\":7.39209050000000000000},{\"Latitude\":46.94992680000000000000,\"Longitude\":7.39211730000000000000},{\"Latitude\":46.94993600000000000000,\"Longitude\":7.39215490000000000000},{\"Latitude\":46.94995790000000000000,\"Longitude\":7.39216020000000000000},{\"Latitude\":46.94997750000000000000,\"Longitude\":7.39221100000000000000},{\"Latitude\":46.94999680000000000000,\"Longitude\":7.39222180000000000000},{\"Latitude\":46.95002020000000000000,\"Longitude\":7.39229430000000000000},{\"Latitude\":46.95003670000000000000,\"Longitude\":7.39230780000000000000},{\"Latitude\":46.95007370000000000000,\"Longitude\":7.39241220000000000000},{\"Latitude\":46.95005910000000000000,\"Longitude\":7.39247120000000000000},{\"Latitude\":46.95011220000000000000,\"Longitude\":7.39263490000000000000},{\"Latitude\":46.95013420000000000000,\"Longitude\":7.39264020000000000000},{\"Latitude\":46.95013920000000000000,\"Longitude\":7.39268330000000000000},{\"Latitude\":46.95013050000000000000,\"Longitude\":7.39271260000000000000},{\"Latitude\":46.94996530000000000000,\"Longitude\":7.39284150000000000000},{\"Latitude\":46.94994370000000000000,\"Longitude\":7.39282530000000000000},{\"Latitude\":46.94988510000000000000,\"Longitude\":7.39267780000000000000},{\"Latitude\":46.94989250000000000000,\"Longitude\":7.39264830000000000000},{\"Latitude\":46.94992310000000000000,\"Longitude\":7.39263230000000000000},{\"Latitude\":46.94990710000000000000,\"Longitude\":7.39258390000000000000},{\"Latitude\":46.94991810000000000000,\"Longitude\":7.39255980000000000000},{\"Latitude\":46.94994140000000000000,\"Longitude\":7.39254650000000000000},{\"Latitude\":46.94986320000000000000,\"Longitude\":7.39229690000000000000},{\"Latitude\":46.94981010000000000000,\"Longitude\":7.39233180000000000000},{\"Latitude\":46.94979680000000000000,\"Longitude\":7.39228090000000000000},{\"Latitude\":46.94980600000000000000,\"Longitude\":7.39225950000000000000},{\"Latitude\":46.94976570000000000000,\"Longitude\":7.39213610000000000000}]";
            a += ",[{\"Latitude\":46.95004810000000000000,\"Longitude\":7.39310420000000000000},{\"Latitude\":46.95002610000000000000,\"Longitude\":7.39308810000000000000},{\"Latitude\":46.94997260000000000000,\"Longitude\":7.39293000000000000000},{\"Latitude\":46.95015020000000000000,\"Longitude\":7.39279320000000000000},{\"Latitude\":46.95017630000000000000,\"Longitude\":7.39280120000000000000},{\"Latitude\":46.95021290000000000000,\"Longitude\":7.39291920000000000000},{\"Latitude\":46.95020330000000000000,\"Longitude\":7.39294080000000000000},{\"Latitude\":46.95017710000000000000,\"Longitude\":7.39296180000000000000},{\"Latitude\":46.95018500000000000000,\"Longitude\":7.39298370000000000000},{\"Latitude\":46.95017990000000000000,\"Longitude\":7.39300230000000000000},{\"Latitude\":46.95004810000000000000,\"Longitude\":7.39310420000000000000}]";
            a += "]";


            // 17+19
            a = "[ ";
            a += " [{\"Latitude\":46.88860330000000000000,\"Longitude\":7.49822180000000000000},{\"Latitude\":46.88863710000000000000,\"Longitude\":7.49820240000000000000},{\"Latitude\":46.88862600000000000000,\"Longitude\":7.49816110000000000000},{\"Latitude\":46.88879070000000000000,\"Longitude\":7.49806660000000000000},{\"Latitude\":46.88879920000000000000,\"Longitude\":7.49809820000000000000},{\"Latitude\":46.88883680000000000000,\"Longitude\":7.49807670000000000000},{\"Latitude\":46.88887590000000000000,\"Longitude\":7.49822280000000000000},{\"Latitude\":46.88863990000000000000,\"Longitude\":7.49835820000000000000},{\"Latitude\":46.88860330000000000000,\"Longitude\":7.49822180000000000000}]";
            a += ",[{\"Latitude\":46.88863990000000000000,\"Longitude\":7.49835820000000000000},{\"Latitude\":46.88828220000000000000,\"Longitude\":7.49856350000000000000},{\"Latitude\":46.88824520000000000000,\"Longitude\":7.49842520000000000000},{\"Latitude\":46.88840000000000000000,\"Longitude\":7.49833640000000000000},{\"Latitude\":46.88839070000000000000,\"Longitude\":7.49830160000000000000},{\"Latitude\":46.88855590000000000000,\"Longitude\":7.49820680000000000000},{\"Latitude\":46.88856570000000000000,\"Longitude\":7.49824340000000000000},{\"Latitude\":46.88860330000000000000,\"Longitude\":7.49822180000000000000},{\"Latitude\":46.88863990000000000000,\"Longitude\":7.49835820000000000000}]";
            a += "]";

            // 21 + 23
            a = "[ ";
            a += " [{\"Latitude\":46.88925940000000000000,\"Longitude\":7.49783360000000000000},{\"Latitude\":46.88929810000000000000,\"Longitude\":7.49781090000000000000},{\"Latitude\":46.88928660000000000000,\"Longitude\":7.49777260000000000000},{\"Latitude\":46.88942850000000000000,\"Longitude\":7.49769110000000000000},{\"Latitude\":46.88953300000000000000,\"Longitude\":7.49769160000000000000},{\"Latitude\":46.88953480000000000000,\"Longitude\":7.49783590000000000000},{\"Latitude\":46.88929780000000000000,\"Longitude\":7.49797440000000000000},{\"Latitude\":46.88925940000000000000,\"Longitude\":7.49783360000000000000}]";
            a += ",[{\"Latitude\":46.88925940000000000000,\"Longitude\":7.49783360000000000000},{\"Latitude\":46.88929780000000000000,\"Longitude\":7.49797440000000000000},{\"Latitude\":46.88899300000000000000,\"Longitude\":7.49815240000000000000},{\"Latitude\":46.88895090000000000000,\"Longitude\":7.49799820000000000000},{\"Latitude\":46.88922200000000000000,\"Longitude\":7.49783990000000000000},{\"Latitude\":46.88922570000000000000,\"Longitude\":7.49785330000000000000},{\"Latitude\":46.88925940000000000000,\"Longitude\":7.49783360000000000000}]";
            a += "]";


            System.Collections.Generic.List<System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>> ls =
                Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>>>(a);


            // ////////////////////ls = FineGrainedHullPoints(ls, 100);
            System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> lsUnionPolygonPoints = GetUnionPolygon(ls);

            // System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> lsUnionPolygonPoints =
            // Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>>(a);
            // ReadStream<System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>>(@"C:\Users\Administrator\IdeaProjects\ComputeHull\hullCoordinates.json");


            // string insertString = CreateInsertScript(lsUnionPolygonPoints, "1a23fd48-406e-4d20-8693-4b8786234d37");
            // string insertString = CreateInsertScript(lsUnionPolygonPoints, "C0F5CAD2-6BA8-4390-A26C-C6682D1DE4F2");
            // string insertString = CreateInsertScript(lsUnionPolygonPoints, "625A6B9A-88DA-454B-B472-0B8C156B3C36");
            string insertString = CreateInsertScript(lsUnionPolygonPoints, "2643530C-F626-4A9C-899A-A8B2857923CE");
            
            System.Console.WriteLine(insertString);
        }
        // http://www.rotefabrik.free.fr/concave_hull/


        public static System.Collections.Generic.List<System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>> FineGrainedHullPoints(
            System.Collections.Generic.List<System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>> ls, int numPointsOnLine
            )
        {
            System.Collections.Generic.List<System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>> ls2 = new System.Collections.Generic.List<System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>>();

            foreach (System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> thisList in ls)
            {

                System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> newList =
                    new System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>();

                for (int i = 0; i < thisList.Count; ++i)
                {
                    OSM.API.v0_6.GeoPoint thisPoint = thisList[i];
                    newList.Add(thisPoint);

                    if (i < thisList.Count - 1)
                    {
                        OSM.API.v0_6.GeoPoint nextPoint = thisList[i + 1];

                        decimal deltaX = nextPoint.Latitude - thisPoint.Latitude;
                        decimal deltaY = nextPoint.Longitude - thisPoint.Longitude;

                        deltaX = deltaX / numPointsOnLine;
                        deltaY = deltaY / numPointsOnLine;

                        for (int j = 0; j < numPointsOnLine; ++j)
                        {
                            var newPoint = new OSM.API.v0_6.GeoPoint(thisPoint.Latitude + j * deltaX, thisPoint.Longitude + j * deltaY);
                            newList.Add(newPoint);
                        }
                    }
                }

                ls2.Add(newList);

            }

            return ls2;
        }



        public static T ReadStream<T>(string fileName)
        {
            T p = default(T);

            using (System.IO.Stream s = System.IO.File.OpenRead(fileName))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                using (Newtonsoft.Json.JsonReader reader = new Newtonsoft.Json.JsonTextReader(sr))
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

                    // read the json from a stream
                    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                    p = serializer.Deserialize<T>(reader);
                }
            }

            return p;
        }


        public static System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> GetUnionPolygon(
            System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<OSM.API.v0_6.GeoPoint>> polygons)
        {
            NetTopologySuite.Geometries.GeometryFactory geomFactory = new NetTopologySuite.Geometries.GeometryFactory();


            System.Collections.Generic.List<NetTopologySuite.Geometries.Geometry> lsPolygons =
                new System.Collections.Generic.List<NetTopologySuite.Geometries.Geometry>();

            foreach (System.Collections.Generic.IEnumerable<OSM.API.v0_6.GeoPoint> coords in polygons)
            {
                NetTopologySuite.Geometries.Polygon poly = geomFactory.CreatePolygon(ToNetTopologyCoordinates(coords));
                lsPolygons.Add(poly);
            }


            NetTopologySuite.Geometries.Geometry ig = NetTopologySuite.Operation.Union.CascadedPolygonUnion.Union(lsPolygons);
            System.Console.WriteLine(ig.GetType().FullName);



            

            if (ig is NetTopologySuite.Geometries.Polygon)
            {
                System.Console.WriteLine("mulip");
                goto SIMPLIFY_POLYGON_AND_GET_UNION;
            }


            if (!(ig is NetTopologySuite.Geometries.MultiPolygon))
            {
                System.Console.WriteLine("Error: Geometry is not a multipolygon!");
                throw new System.InvalidOperationException("Geometry is not a multipolygon");
            }


            NetTopologySuite.Geometries.MultiPolygon lalala = (NetTopologySuite.Geometries.MultiPolygon)ig;

            // var convaveHull = ConcaveHull.Init.foo(lalala.Coordinates);
            // convaveHull = ToCounterClockWise(convaveHull);
            // return convaveHull;




            // var cc = new NetTopologySuite.Hull.ConcaveHull(ig, 0);
            // var cc = new NetTopologySuite.Hull.ConcaveHull(ig, 0.00049);
            var cc = new NetTopologySuite.Hull.ConcaveHull(ig, 0.00001);
            ig = cc.GetConcaveHull;

            SIMPLIFY_POLYGON_AND_GET_UNION:

            ig = NetTopologySuite.Simplify.DouglasPeuckerSimplifier.Simplify(ig, 0.00001);
            NetTopologySuite.Geometries.Polygon unionPolygon = (NetTopologySuite.Geometries.Polygon)ig;


            

            System.Console.WriteLine(unionPolygon.Shell.Coordinates);




            System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> lsUnionPolygonPoints = new System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>();

            for (int i = 0; i < unionPolygon.Shell.Coordinates.Length; ++i)
            {
                lsUnionPolygonPoints.Add(new OSM.API.v0_6.GeoPoint(unionPolygon.Shell.Coordinates[i].X, unionPolygon.Shell.Coordinates[i].Y));
            }

            lsUnionPolygonPoints = ToCounterClockWise(lsUnionPolygonPoints);

            return lsUnionPolygonPoints;
        }


        public static bool IsClockwise(System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> poly)
        {
            decimal sum = 0;

            for (int i = 0; i < poly.Count - 1; i++)
            {
                OSM.API.v0_6.GeoPoint cur = poly[i], next = poly[i + 1];
                sum += (next.Latitude - cur.Latitude) * (next.Longitude + cur.Longitude);
            } // Next i 

            return sum > 0;
        } // End Function isClockwise 


        // MSSQL is CLOCKWISE (MS-SQL wants the polygon points in clockwise sequence) 
        public static System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> ToClockWise(System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> poly)
        {
            if (!IsClockwise(poly))
                poly.Reverse();

            return poly;
        } // End Function toClockWise 


        // OSM is COUNTER-clockwise  (OSM wants the polygon points in counterclockwise sequence) 
        public static System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> ToCounterClockWise(System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> poly)
        {
            if (IsClockwise(poly))
                poly.Reverse();

            return poly;
        } // End Function toCounterClockWise 


        public static NetTopologySuite.Geometries.Coordinate[] ToNetTopologyCoordinates(System.Collections.Generic.IEnumerable<OSM.API.v0_6.GeoPoint> coords) //, int z)
        {
            NetTopologySuite.Geometries.Coordinate[] coordinates = new NetTopologySuite.Geometries.Coordinate[coords.Count()];

            int i = 0;
            foreach (OSM.API.v0_6.GeoPoint thisCoordinate in coords)
            {
                coordinates[i] = new NetTopologySuite.Geometries.Coordinate((double)thisCoordinate.Latitude, (double)thisCoordinate.Longitude);
                ++i;
            }

            return coordinates;
        } // End Function ToNetTopologyCoordinates 


        public static string CreateInsertScript(System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> coords)
        {
            return CreateInsertScript(coords, System.Guid.NewGuid().ToString().Replace("0", "A"));
        }

        public static string CreateInsertScript(System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> coords, string gb_uid)
        {
            // Close polygon if unclosed
            if (coords[0].Latitude != coords[coords.Count - 1].Latitude || coords[0].Longitude != coords[coords.Count - 1].Longitude)
                coords.Add(coords[0]);


            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"
DECLARE @GB_UID AS uniqueidentifier;
DECLARE @SO_UID AS uniqueidentifier;

SET @GB_UID = NULLIF('" + gb_uid + @"', '');
SET @SO_UID = NULLIF('', '');


DELETE FROM T_ZO_Objekt_Wgs84Polygon WHERE ZO_OBJ_WGS84_GB_UID = @GB_UID; 


/* */
INSERT INTO T_ZO_Objekt_Wgs84Polygon
(
     ZO_OBJ_WGS84_UID
    ,ZO_OBJ_WGS84_GB_UID
    ,ZO_OBJ_WGS84_SO_UID
    ,ZO_OBJ_WGS84_Sort
    ,ZO_OBJ_WGS84_GM_Lat
    ,ZO_OBJ_WGS84_GM_Lng
)
");


            for (int i = 0; i < coords.Count; ++i)
            {
                if (i != 0)
                    sb.Append(" \r\n\r\n\r\nUNION ALL \r\n\r\n");


                sb.Append($@"
SELECT
     NEWID() AS ZO_OBJ_WGS84_UID
    ,CAST(@GB_UID AS uniqueidentifier) AS ZO_OBJ_WGS84_GB_UID
    ,CAST(@SO_UID AS uniqueidentifier) AS ZO_OBJ_WGS84_SO_UID
    ,CAST({i.ToString(System.Globalization.CultureInfo.InvariantCulture)} AS integer) + 1 AS ZO_OBJ_WGS84_Sort
    ,{coords[i].Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)} AS ZO_OBJ_WGS84_GM_Lat -- decimal(23, 20)
    ,{coords[i].Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)} AS ZO_OBJ_WGS84_GM_Lng -- decimal(23, 20) ");
            }


            sb.Append(" \r\n; \r\n\r\n");
            string insertString = sb.ToString();
            return insertString;
        }


    }


}

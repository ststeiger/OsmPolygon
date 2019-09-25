
namespace OsmPolygon.EsriConverter
{


    public class ESRI
    {


        public static string ProjectEsriCoordinatesToWGS84()
        {
            System.Collections.Generic.List<XYCoordinates> ls = new System.Collections.Generic.List<XYCoordinates>();

            ls.Add(new XYCoordinates(2649442.892000001M, 1223838.7969999984M));
            ls.Add(new XYCoordinates(2649442.9860000014M, 1223891.6520000026M));
            ls.Add(new XYCoordinates(2649521.438000001M, 1223891.5129999965M));
            ls.Add(new XYCoordinates(2649521.3440000005M, 1223838.6569999978M));
            ls.Add(new XYCoordinates(2649442.892000001M, 1223838.7969999984M));

            ls.Clear();


            ls.Add(new XYCoordinates(2649501.129999999M, 1223826.9710000008M));
            ls.Add(new XYCoordinates(2649541.4679999985M, 1223827.25M));
            ls.Add(new XYCoordinates(2649541.8850000016M, 1223776.6180000007M));
            ls.Add(new XYCoordinates(2649501.4800000004M, 1223776.3390000015M));
            ls.Add(new XYCoordinates(2649501.129999999M, 1223826.9710000008M));


            ls.Clear();

            ls.Add(new XYCoordinates(2648891M, 1223263));


            ls.Clear();

            // Reservoir
            ls.Add(new XYCoordinates(2648888.9059999995M, 1223264.7739999965M));
            ls.Add(new XYCoordinates(2648891.649M, 1223266.199000001M));
            ls.Add(new XYCoordinates(2648893.0749999993M, 1223263.3510000035M));
            ls.Add(new XYCoordinates(2648890.2919999994M, 1223261.9739999995M));
            ls.Add(new XYCoordinates(2648888.9059999995M, 1223264.7739999965M));


            ls.Clear();


            //ls.Add(new XYCoordinates(2649492M, 1223528M));
            // ls.Add(new XYCoordinates(2649538M, 1223668M));


            ls.Add(new XYCoordinates(2649182.0439999998M, 1223866.9340000004M));
            ls.Add(new XYCoordinates(2649182.886M, 1223883.7870000005M));
            ls.Add(new XYCoordinates(2649317.0320000015M, 1223877.8879999965M));
            ls.Add(new XYCoordinates(2649324.810009818M, 1223877.4092443336M));
            ls.Add(new XYCoordinates(2649332.566485611M, 1223876.6584268059M));
            ls.Add(new XYCoordinates(2649340.291914045M, 1223875.6364682931M));
            ls.Add(new XYCoordinates(2649347.976819863M, 1223874.3446222297M));
            ls.Add(new XYCoordinates(2649355.6117775105M, 1223872.784473068M));
            ls.Add(new XYCoordinates(2649363.187422694M, 1223870.9579343346M));
            ls.Add(new XYCoordinates(2649370.6944638663M, 1223868.8672462837M));
            ls.Add(new XYCoordinates(2649378.1236936236M, 1223866.514973149M));
            ls.Add(new XYCoordinates(2649385.465999998M, 1223863.9039999992M));
            ls.Add(new XYCoordinates(2649379.666000001M, 1223848.0219999999M));
            ls.Add(new XYCoordinates(2649372.3908646205M, 1223850.478075915M));
            ls.Add(new XYCoordinates(2649365.038716049M, 1223852.692983383M));
            ls.Add(new XYCoordinates(2649357.6175465505M, 1223854.6643146556M));
            ls.Add(new XYCoordinates(2649350.1354234195M, 1223856.389926767M));
            ls.Add(new XYCoordinates(2649342.6004802105M, 1223857.867943865M));
            ls.Add(new XYCoordinates(2649335.0209078966M, 1223859.0967592488M));
            ls.Add(new XYCoordinates(2649327.4049459663M, 1223860.075037116M));
            ls.Add(new XYCoordinates(2649319.760873466M, 1223860.801714015M));
            ls.Add(new XYCoordinates(2649312.096999999M, 1223861.2760000005M));
            ls.Add(new XYCoordinates(2649182.0439999998M, 1223866.9340000004M));


            string sql = ProjectEsriCoordinatesToWGS84(ls);
            return sql;
        }


        public static string ProjectEsriCoordinatesToWGS84(System.Collections.Generic.List<XYCoordinates> ls)
        {
            DotSpatial.Projections.ProjectionInfo projFrom = DotSpatial.Projections.ProjectionInfo.FromEpsgCode(2056);
            DotSpatial.Projections.ProjectionInfo projTo = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
            // DotSpatial.Projections.ProjectionInfo projTo = DotSpatial.Projections.ProjectionInfo.FromEpsgCode(3857); // Web Mercator projection
            // DotSpatial.Projections.ProjectionInfo projTo = DotSpatial.Projections.ProjectionInfo.FromEpsgCode(4326); // WGS84

            XYCoordinates[] mycoordinates = ls.ToArray();



            double[] latLonPoints = new double[mycoordinates.Length * 2];
            double[] z = new double[mycoordinates.Length];

            // dotspatial takes the x,y in a single array, and z in a separate array.  I'm sure there's a 
            // reason for this, but I don't know what it is.
            for (int i = 0; i < mycoordinates.Length; i++)
            {
                latLonPoints[i * 2] = (double)mycoordinates[i].X;
                latLonPoints[i * 2 + 1] = (double)mycoordinates[i].Y;
                z[i] = 0;
            } // Next i 

            // prepare for ReprojectPoints (it's mutate array)
            DotSpatial.Projections.Reproject.ReprojectPoints(
                latLonPoints, z, projFrom, projTo
                , 0, latLonPoints.Length / 2
            );

            // assemblying new points array to create polygon
            Wgs84Coordinates[] polyPoints = new Wgs84Coordinates[latLonPoints.Length / 2];

            for (int i = 0; i < latLonPoints.Length / 2; ++i)
            {
                polyPoints[i] = new Wgs84Coordinates(latLonPoints[i * 2 + 1], latLonPoints[i * 2]);
            } // Next i 

            string sql = @"
DECLARE @gb_uid uniqueidentifier;
-- SET @gb_uid = '';

/*
DELETE FROM T_ZO_Objekt_Wgs84Polygon WHERE ZO_OBJ_WGS84_GB_UID = @gb_uid; 


INSERT INTO T_ZO_Objekt_Wgs84Polygon 
( 
     ZO_OBJ_WGS84_UID, ZO_OBJ_WGS84_GB_UID, ZO_OBJ_WGS84_SO_UID 
    ,ZO_OBJ_WGS84_Sort ,ZO_OBJ_WGS84_GM_Lat, ZO_OBJ_WGS84_GM_Lng 
) 
*/

SELECT 
     ZO_OBJ_WGS84_UID, ZO_OBJ_WGS84_GB_UID, ZO_OBJ_WGS84_SO_UID 
    ,ZO_OBJ_WGS84_Sort ,ZO_OBJ_WGS84_GM_Lat, ZO_OBJ_WGS84_GM_Lng 
FROM 
(
";

            for (int i = 0; i < polyPoints.Length; ++i)
            {
                if (i != 0)
                {
                    sql += @"

UNION ALL 

";
                }

                sql += @"
SELECT 
	 NEWID() AS ZO_OBJ_WGS84_UID
	,@gb_uid AS ZO_OBJ_WGS84_GB_UID
	,NULL AS ZO_OBJ_WGS84_SO_UID
	," + i.ToString(System.Globalization.CultureInfo.InvariantCulture) + @" AS ZO_OBJ_WGS84_Sort
	,CAST(" + polyPoints[i].Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + @" AS decimal(23,20) ) AS ZO_OBJ_WGS84_GM_Lat 
	,CAST(" + polyPoints[i].Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + @" AS decimal(23,20) ) AS ZO_OBJ_WGS84_GM_Lng

";
            }

            sql += @"
) AS t 

ORDER BY ZO_OBJ_WGS84_Sort
";

            return sql;
        }



    }
}

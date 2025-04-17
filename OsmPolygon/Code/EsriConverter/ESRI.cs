
namespace OsmPolygon.EsriConverter
{


    public class ESRI
    {

        // https://map.geo.tg.ch/
        // Grundlagen und Planung -> Grundstückkataster -> Grundbuch, Grundeigentümer -> Grundbuch
        // Grundlagen und Planung -> Grundstückkataster -> Amtliche Vermessung -> ProjBodenbedeckung
        // Grundlagen und Planung -> Bodenbedeckung ->  Amtliche Vermessung -> 
        // https://map.geo.tg.ch/apps/mf-geoadmin3/?lang=de&topic=ech&catalogNodes=10000,12000,12001,12006&layers=av_wms_lcsfproj&layers_opacity=0.9&E=2734377.19&N=1267817.41&zoom=8


        // "EPSG:2056" (LV95)  to EPSG:4326 (WGS84)
        // https://epsg.io/transform#s_srs=2056&t_srs=4326&ops=1676&x=NaN&y=NaN
        // https://epsg.io/transform#s_srs=2056&t_srs=4326&ops=1676&x=2734367.098&y=1267789.988
        // https://epsg.io/map#srs=2056&x=2734380.943&y=1267793.486&z=19&layer=streets
        public static string Test3()
        {
            string json = @"
[
    [
        2734431.581,
        1267805.457
    ],
    [
        2734433.708,
        1267805.979
    ],
    [
        2734433.534,
        1267806.687
    ],
    [
        2734446.066,
        1267809.776
    ],
    [
        2734453.024,
        1267781.542
    ],
    [
        2734452.266,
        1267781.357
    ],
    [
        2734441.249,
        1267778.662
    ],
    [
        2734440.502,
        1267778.479
    ],
    [
        2734438.726,
        1267785.669
    ],
    [
        2734438.056,
        1267785.504
    ],
    [
        2734437.84,
        1267786.377
    ],
    [
        2734438.5,
        1267786.54
    ],
    [
        2734436.46,
        1267794.81
    ],
    [
        2734435.643,
        1267798.125
    ],
    [
        2734433.516,
        1267797.602
    ],
    [
        2734433.475,
        1267797.767
    ],
    [
        2734431.581,
        1267805.457
    ]
]

";
            Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.Converters.Add(new XYCoordinatesNewtonsoftConverter());

            System.Collections.Generic.List<XYCoordinates> coordinates = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<XYCoordinates>>(json, settings);

#if false
            foreach (XYCoordinates coord in coordinates)
            {
                System.Console.WriteLine(coord);
            }
#endif 

            string sql = ProjectEsriCoordinatesToWGS84(coordinates);
            return sql;
        }

        public static string Test2()
        {
            string json = @"
 [
    [
        2734367.098,
        1267789.988
    ],
    [
        2734380.943,
        1267793.486
    ],
    [
        2734388.978,
        1267761.686
    ],
    [
        2734375.133,
        1267758.188
    ],
    [
        2734367.098,
        1267789.988
    ]
]
";

            System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(new XYCoordinatesConverter());

            System.Collections.Generic.List<XYCoordinates> coordinates = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<XYCoordinates>>(json, options);

#if false
            foreach (XYCoordinates coord in coordinates)
            {
                System.Console.WriteLine(coord);
            }
#endif 


            string sql = ProjectEsriCoordinatesToWGS84(coordinates);
            return sql;
        }

        public static string Test()
        {
            string coordString = "2649154.965999998,1223895.6469999999;2649157.3489999995,1223895.538999997;2649395.416000001,1223884.7140000015;2649386.8440000005,1223874.1290000007;2649369.3170000017,1223852.4870000035;2649350.7030000016,1223857.1920000017;2649329.9389999993,1223859.7810000032;2649176.011,1223866.4399999976;2649168.857999999,1223871.7760000005;2649154.965999998,1223895.6469999999";
            coordString = "2649182.085000001,1223867.0059999973;2649182.8729999997,1223883.8950000033;2649183.561999999,1223883.8800000027;2649183.7619999982,1223888.0879999995;2649231.535,1223885.9689999968;2649231.3440000005,1223881.7419999987;2649319.140999999,1223877.7229999974;2649327.6027735183,1223877.099832654;2649336.0351367067,1223876.1589821363;2649344.4261669307,1223874.9017787254;2649352.7639999986,1223873.3299999982;2649359.4124017823,1223871.8419524927;2649366.012805041,1223870.1536964455;2649372.559182193,1223868.2667735957;2649379.0455549946,1223866.1829071082;2649385.465999998,1223863.9039999992;2649385.9970000014,1223863.726999998;2649387.4299999997,1223867.6450000033;2649394.6113535245,1223864.8711497632;2649401.696160275,1223861.8592405831;2649408.6765312525,1223858.612626252;2649415.5446937494,1223855.1349219054;2649422.2930000015,1223851.4299999997;2649422.2939999998,1223851.1499999985;2649425.7309999987,1223849.1279999986;2649425.2349999994,1223848.2859999985;2649425.4439999983,1223848.1459999979;2649423.965,1223845.6080000028;2649430.411304071,1223841.5885832564;2649436.73181778,1223837.3741255673;2649442.9206418707,1223832.9685605033;2649448.971999999,1223828.376000002;2649451.708999999,1223826.163999997;2649456.278000001,1223822.3189999983;2649461.3312038975,1223817.8284375388;2649466.2545339344,1223813.1958527127;2649471.0439999998,1223808.424999997;2649475.1689999998,1223804.0890000015;2649480.8368671318,1223797.7551539226;2649486.2749999985,1223791.2229999974;2649489.965,1223786.5050000027;2649494.20935077,1223780.7569424736;2649498.2789113256,1223774.8838380391;2649502.170000002,1223768.8910000026;2649504.642000001,1223770.5370000005;2649505.666000001,1223771.2189999968;2649512.962000001,1223760.1730000004;2649518.085447566,1223752.0879827759;2649523.0798673593,1223743.922629789;2649527.9439999983,1223735.6789999977;2649519.5650000013,1223730.825000003;2649516.087000001,1223728.8069999963;2649509.6499999985,1223725.0710000023;2649507.7419999987,1223728.3449999988;2649502.0667869775,1223737.8053977352;2649496.2131423927,1223747.1564411656;2649490.183172671,1223756.3947653277;2649483.9790476914,1223765.517045821;2649477.603,1223774.5200000033;2649476.697999999,1223775.7449999973;2649471.9377391343,1223781.8590448555;2649466.960940949,1223787.798161612;2649461.774028377,1223793.5546853885;2649456.3836955195,1223799.121186957;2649450.7968990086,1223804.4904823266;2649445.0208490263,1223809.655642017;2649439.063000001,1223814.6099999994;2649434.704999998,1223817.9689999968;2649428.877418447,1223822.228290052;2649422.9083995814,1223826.28699333;2649416.8048446607,1223830.1404172417;2649410.57381049,1223833.7841065363;2649404.222501264,1223837.2138484546;2649397.7582602366,1223840.4256775994;2649391.188561232,1223843.415880519;2649384.5210000016,1223846.1810000017;2649383.335000001,1223846.6340000033;2649379.853,1223847.9509999976;2649379.666000001,1223848.0219999999;2649372.169812606,1223850.610128859;2649364.587361403,1223852.9334175182;2649356.9279999994,1223854.989;2649350.151813826,1223856.5366447174;2649343.329527922,1223857.866426004;2649336.468145009,1223858.9769789088;2649329.574707941,1223859.8671635066;2649322.656292473,1223860.536066069;2649315.719999999,1223860.9830000028;2649187.284000002,1223866.7669999972;2649186.283,1223866.7920000032;2649182.085000001,1223867.0059999973";
            coordString = "2649388.7060000002,1223568.9120000005;2649442.5689999983,1223606.789999999;2649456.5560000017,1223617.7819999978;2649469.587000001,1223629.6410000026;2649480.079999998,1223640.5600000024;2649491.0119999982,1223653.3500000015;2649501.0119999982,1223666.7220000029;2649512.563000001,1223684.9909999967;2649528.2540000007,1223713.9540000036;2649536.7349999994,1223728.553000003;2649545.8099999987,1223741.012000002;2649549.2239999995,1223745.0219999999;2649561.4580000006,1223645.4650000036;2649561.6779999994,1223643.3259999976;2649563.8720000014,1223635.7419999987;2649563.3880000003,1223625.9230000004;2649564.4329999983,1223610.6429999992;2649566.5890000015,1223605.8570000008;2649566.162999999,1223600.7760000005;2649566.3649999984,1223591.9430000037;2649564.9450000003,1223590.2719999999;2649565.0659999996,1223584.9209999964;2649562.949000001,1223577.4060000032;2649564.1739999987,1223575.023000002;2649563.329999998,1223564.8559999987;2649563.8079999983,1223550.5340000018;2649565.892000001,1223537.1590000018;2649565.256000001,1223527.5979999974;2649568.976,1223522.6550000012;2649570.232999999,1223514.439000003;2649575.5989999995,1223486.2159999982;2649577.590999998,1223476.571999997;2649577.2060000002,1223467.914999999;2649581.170000002,1223459.9780000001;2649583.0130000003,1223460.7819999978;2649604.4250000007,1223470.1159999967;2649625.2459999993,1223480.4619999975;2649629.4420000017,1223470.1660000011;2649636.052000001,1223441.523000002;2649647.6609999985,1223396.7079999968;2649657.8630000018,1223377.3830000013;2649667.096000001,1223360.3100000024;2649671.465,1223343.2339999974;2649682.6160000004,1223329.4739999995;2649732.4789999984,1223289.023000002;2649733.4849999994,1223288.2060000002;2649729.9880000018,1223285.612999998;2649715.837000001,1223275.123999998;2649706.379999999,1223270.0939999968;2649703.034000002,1223270.226999998;2649687.193,1223250.0960000008;2649666.4180000015,1223226.5670000017;2649635.0670000017,1223262.1000000015;2649581.4219999984,1223309.7419999987;2649543.9499999993,1223364.2060000002;2649496.1959999986,1223388.700000003;2649476.16,1223392.4240000024;2649432.4569999985,1223480.726999998;2649388.7060000002,1223568.9120000005";
            coordString = "2649528,1223702;2649548,1223702;2649548,1223634;2649528,1223634;";

            string[] coords = coordString.Split(new char[] { ';' },System.StringSplitOptions.RemoveEmptyEntries );

            System.Collections.Generic.List<XYCoordinates> ls = new System.Collections.Generic.List<XYCoordinates>();

            for (int i = 0; i < coords.Length; ++i)
            {
                string[] strxy = coords[i].Split(',');

                string strx = strxy[0];
                string stry = strxy[1];

                decimal x = decimal.Parse(strx);
                decimal y = decimal.Parse(stry);

                ls.Add(new XYCoordinates(x, y));
            }

            string sql = ProjectEsriCoordinatesToWGS84(ls);
            return sql;
        }


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

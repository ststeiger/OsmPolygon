
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OsmPolygon
{


    class Program
    {



        public static void GetInsertPoints()
        {
            // DELETE FROM T_ZO_Objekt_Wgs84Polygon WHERE ZO_OBJ_WGS84_GB_UID IN ('54DCED29-5B90-48EA-9B0B-9B165AB27869', 'A952E260-C278-4DE3-81C1-8D258985A3F5', '33BD0502-CAF4-4213-89E6-A982F27F2D86', '27894D75-95AA-4DE6-AD18-B113453A192C', '485A23D7-7DA8-4011-92B8-DBFE44F77D1A')

            string[] inserts = new string[13];
            /*
            // Via Crusch
            inserts[0] = OSM.API.v0_6.Polygon.GetPointsInsert("263865951", "485A23D7-7DA8-4011-92B8-DBFE44F77D1A");


            // W + P Zollikon
            inserts[1] = OSM.API.v0_6.Polygon.GetPointsInsert("444785642", "27894D75-95AA-4DE6-AD18-B113453A192C");

            // Erlen Lista
            inserts[2] = OSM.API.v0_6.Polygon.GetPointsInsert("231594843", "33BD0502-CAF4-4213-89E6-A982F27F2D86");

            // Soodring 33
            inserts[3] = OSM.API.v0_6.Polygon.GetPointsInsert("37247719", "A952E260-C278-4DE3-81C1-8D258985A3F5");

            // Hôtel de ville, Bellegarde-sur-Valserine,
            inserts[4] = OSM.API.v0_6.Polygon.GetPointsInsert("83381692", "54DCED29-5B90-48EA-9B0B-9B165AB27869");
            
            // Post Erlen
            inserts[5] = OSM.API.v0_6.Polygon.GetPointsInsert("418577088", "7F839043-D997-4F50-B5D2-F120F1EA9EA6");

            // Silberwürfel Bahnbaugebäude
            inserts[6] = OSM.API.v0_6.Polygon.GetPointsInsert("53560029", "9C79019E-C901-4C15-959C-A25412362C30");

            // Aufstockung
            inserts[7] = OSM.API.v0_6.Polygon.GetPointsInsert("24593132", "9F131BDC-353D-4DBF-AF6D-A38CAD0B3DB4");

            // Blaues Haus
            inserts[8] = OSM.API.v0_6.Polygon.GetPointsInsert("106567017", "31D3B5D6-7089-4855-98B5-E12F88080CFB");

            // Parkhaus
            inserts[9] = OSM.API.v0_6.Polygon.GetPointsInsert("520515573", "81F77307-6C77-4282-9BD9-D0D3913DD074");

            // Gemeindeverwaltung Hünenberg	Chamerstrasse	11
            inserts[10] = OSM.API.v0_6.Polygon.GetPointsInsert("397894744", "8F45E16B-BF88-487C-AEF3-CC0BCCBAADC0");

            // -- Pilatusstrasse 12
            inserts[11] = OSM.API.v0_6.Polygon.GetPointsInsert("86335450", "C408352D-0265-4166-BDEA-363A795F51EF");
            */




            // Mägenwil 47.415412, 8.235762
            inserts[12] = OSM.API.v0_6.Polygon.GetPointsInsert("28858277", "A3E35F21-97B5-448F-BD4A-D62EF5590495");

            string insert = string.Join(System.Environment.NewLine + System.Environment.NewLine, inserts);
            System.Console.WriteLine(insert);
        }


        public static string GetInsertPoints(string way)
        {
            return OSM.API.v0_6.Polygon.GetPointsInsert(way, "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        }


        static void sub()
        {
            Microsoft.Extensions.DependencyInjection.ServiceCollection services =
                new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            string dir = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
            // dir = System.IO.Directory.GetCurrentDirectory();

            //Create configuration builder  
            var configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(dir)
                .AddIniFile("OsmPolygon.ini")
                .AddJsonFile("appsettings.json", true);

            Microsoft.Extensions.Configuration.IConfigurationRoot ic = configurationBuilder.Build();

            services.ConfigureOptions(new ApplicationConfiguration(ic));

            // Inject configuration  
            services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(
                delegate (System.IServiceProvider provider)
                {
                    return ic;
                }
            );

            services.AddTransient<IConnectionFactory, ConnectionFactory>();
            services.AddTransient<Des3EncryptionService>();


            // Inject Serilog  
            services.AddLogging(
                delegate (Microsoft.Extensions.Logging.ILoggingBuilder options)
            {
                string logDir = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                logDir = System.IO.Path.Combine(logDir, "Log");

                if (!System.IO.Directory.Exists(logDir))
                    System.IO.Directory.CreateDirectory(logDir);

                /*
                options.AddFileLogger(
                    delegate (LdapService.Helpers.Log2.FileLoggerOptions fo)
                    {
                        fo.Folder = logDir;
                        fo.LogLevel = LogLevel.Trace;
                    }
                );
                */

                options.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
                options.AddFilter(x => x >= LogLevel.Trace);

                options.AddConsole();
                options.AddDebug();
            });

            // Inject common service  
            // services.AddSingleton(typeof(ICommonService), typeof(CommonSampleService));

            // Inject concrete implementaion of the service  
            // services.AddSingleton(typeof(ServiceBase), typeof(Service1));

            // Build DI provider  
            ServiceProvider serviceProvider = services.BuildServiceProvider();


            try
            {
                IConfiguration conf = serviceProvider.GetService<Microsoft.Extensions.Configuration.IConfiguration>();
                System.Console.WriteLine(conf);

                Newtonsoft.Json.Linq.JToken jtoken = ApplicationConfiguration.SerializeInstance(conf);
                string json = jtoken.ToString();
                System.Console.WriteLine(json);

                var conf2 = serviceProvider.GetService<Microsoft.Extensions.Options.IOptions<ApplicationConfiguration>>();
                System.Console.WriteLine(conf2);

                IConnectionFactory fac = serviceProvider.GetService<IConnectionFactory>();

                // System.Console.WriteLine(typeof(System.Data.SqlClient.SqlClientFactory).AssemblyQualifiedName);
                // System.Console.WriteLine(typeof(Npgsql.NpgsqlFactory).AssemblyQualifiedName);
                // System.Console.WriteLine(typeof(MySql.Data.MySqlClient.MySqlClientFactory).AssemblyQualifiedName);
                System.Console.WriteLine(fac);
            }
            catch (System.Exception ex)
            {
                System.Console.Write(ex.Message);
                System.Console.Write(ex.StackTrace);
            }
        }


        public class ApplicationConfiguration
            : Microsoft.Extensions.Options.IConfigureOptions<Microsoft.Extensions.Configuration.IConfiguration>
        {

            protected Microsoft.Extensions.Configuration.IConfiguration m_config;


            public ApplicationConfiguration(Microsoft.Extensions.Configuration.IConfiguration options)
            {
                this.m_config = options;
            }


            public ApplicationConfiguration()
                :this(null)
            { }


            void IConfigureOptions<Microsoft.Extensions.Configuration.IConfiguration>.Configure(Microsoft.Extensions.Configuration.IConfiguration options)
            {
                this.m_config = options;
            }


            public Newtonsoft.Json.Linq.JToken Serialize()
            {
                return SerializeInstance(this.m_config);
            }


            public static Newtonsoft.Json.Linq.JToken SerializeInstance(Microsoft.Extensions.Configuration.IConfiguration config)
            {
                Newtonsoft.Json.Linq.JObject obj = new Newtonsoft.Json.Linq.JObject();
                foreach (Microsoft.Extensions.Configuration.IConfigurationSection child in config.GetChildren())
                {
                    obj.Add(child.Key, SerializeInstance(child));
                }

                if (!obj.HasValues && config is Microsoft.Extensions.Configuration.IConfigurationSection section)
                    return new Newtonsoft.Json.Linq.JValue(section.Value);

                return obj;
            }

        } // End Class ApplicationConfiguration 


        static void TestArea()
        {
            // <path d="M783.4105582714081 625.6885507297516L881.1681015396113 625.6885507297515L881.1681015396113 812.3167255687712L1210.5360040140147 812.3167255687712L1210.5360040140147 636.6879940795897L1282.0604986762996 636.6996831798552L1282.0604986762996 812.0946326637267L1299.8802755689617 812.0946326637267L1697.8397127056116 812.0946326637267L1697.8397127056116 871.6389094161985L1757.4220355749126 871.6622876167296L1757.4220355749126 874.3858479785918L1946.3817812347406 874.3975370788573L1946.3817812347406 897.308173599243L1970.5465672111507 897.308173599243L1970.5465672111507 1332.0375015735626L1970.5465672111507 1334.1766069221496L1970.3128652191158 1335.929971961975L1969.4832231473918 1338.6652214241026L1968.5367300796504 1340.5471665668485L1967.041037330627 1342.6161373138425L1965.25321709156 1344.346124153137L1962.8460865736004 1345.9475308895107L1960.1234583663938 1347.0346172142026L1957.412515258789 1347.5956940269466L1956.1388394021988 1347.5956940269466L1949.3965369319915 1347.5956940269466L647.7465518951415 1347.5956940269466L642.2194997835159 1347.5255594253538L639.2397993850707 1346.835902509689L636.4470605802535 1345.5267232799529L634.3904830503463 1344.0188293457031L632.2754800224304 1341.716076593399L630.4175491857528 1338.6535323238375L626.8652789068223 1332.4933764839172L572.9268591451646 1238.9455070590975L388.96833611488347 1345.3513867759705L387.7881410551072 1346.2046910953522L385.17067874431615 1347.151508216858L383.12578631401067 1347.5372485256196L340.5452833652497 1347.642450428009L222.61925818443308 1347.642450428009L222.61925818443308 1247.607130355835L222.46735188961037 1158.2088915252684L105.10233989838632 1158.2088915252684L105.10233989838632 1071.0470677037392L387.7881410551072 1071.0470677037392L388.87485531806954 871.6739767169952L318.54224081516276 871.6739767169952L318.7642577075959 544.2505891799926L115.61880113124852 544.2505891799926L119.34634790420537 459.46954495429986L295.5576498985291 459.46954495429986L295.5576498985291 456.7459845924377L385.78998902320865 456.7459845924377L385.78998902320865 486.2142063617706L377.598734202385 486.2142063617706L377.598734202385 812.3167255687714L714.515211019516 812.3167255687714L714.515211019516 689.2421888732911L783.4105582714081 689.2421888732911L783.4105582714081 625.6885507297516" />
            string coords = "783.4105582714081,625.6885507297516;881.1681015396113,625.6885507297515;881.1681015396113,812.3167255687712;1210.5360040140147,812.3167255687712;1210.5360040140147,636.6879940795897;1282.0604986762996,636.6996831798552;1282.0604986762996,812.0946326637267;1299.8802755689617,812.0946326637267;1697.8397127056116,812.0946326637267;1697.8397127056116,871.6389094161985;1757.4220355749126,871.6622876167296;1757.4220355749126,874.3858479785918;1946.3817812347406,874.3975370788573;1946.3817812347406,897.308173599243;1970.5465672111507,897.308173599243;1970.5465672111507,1332.0375015735626;1970.5465672111507,1334.1766069221496;1970.3128652191158,1335.929971961975;1969.4832231473918,1338.6652214241026;1968.5367300796504,1340.5471665668485;1967.041037330627,1342.6161373138425;1965.25321709156,1344.346124153137;1962.8460865736004,1345.9475308895107;1960.1234583663938,1347.0346172142026;1957.412515258789,1347.5956940269466;1956.1388394021988,1347.5956940269466;1949.3965369319915,1347.5956940269466;647.7465518951415,1347.5956940269466;642.2194997835159,1347.5255594253538;639.2397993850707,1346.835902509689;636.4470605802535,1345.5267232799529;634.3904830503463,1344.0188293457031;632.2754800224304,1341.716076593399;630.4175491857528,1338.6535323238375;626.8652789068223,1332.4933764839172;572.9268591451646,1238.9455070590975;388.96833611488347,1345.3513867759705;387.7881410551072,1346.2046910953522;385.17067874431615,1347.151508216858;383.12578631401067,1347.5372485256196;340.5452833652497,1347.642450428009;222.61925818443308,1347.642450428009;222.61925818443308,1247.607130355835;222.46735188961037,1158.2088915252684;105.10233989838632,1158.2088915252684;105.10233989838632,1071.0470677037392;387.7881410551072,1071.0470677037392;388.87485531806954,871.6739767169952;318.54224081516276,871.6739767169952;318.7642577075959,544.2505891799926;115.61880113124852,544.2505891799926;119.34634790420537,459.46954495429986;295.5576498985291,459.46954495429986;295.5576498985291,456.7459845924377;385.78998902320865,456.7459845924377;385.78998902320865,486.2142063617706;377.598734202385,486.2142063617706;377.598734202385,812.3167255687714;714.515211019516,812.3167255687714;714.515211019516,689.2421888732911;783.4105582714081,689.2421888732911;783.4105582714081,625.6885507297516";

            decimal svtToMeter = 28.082194824755913M;
            
            GeoApis.Polygon poly = new GeoApis.Polygon(coords);
            System.Console.WriteLine(poly.isClockwise);
            

            decimal area = poly.Area / (svtToMeter * svtToMeter);
            System.Console.WriteLine(area);

            /*             
            SELECT * FROM T_VWS_SVGElement  
            WHERE SVE_UID = '3D2A190E-6244-4D54-8381-0F2DE3261A51'

            SELECT SO_Nr, SO_Bezeichnung, GB_Nr, GB_Bezeichnung, GS_Nr, GST_Kurz_DE, GS_UID FROM T_AP_Zone 
            LEFT JOIN T_AP_Geschoss ON GS_UID = ZN_GS_UID 
            LEFT JOIN T_AP_Ref_Geschosstyp ON GST_UID = GS_GST_UID 
            LEFT JOIN T_AP_Gebaeude ON GB_UID = GS_GB_UID 
            LEFT JOIN T_AP_Standort ON SO_UID = GB_SO_UID 
            WHERE ZN_UID = '3AC6B81D-8E94-4D4C-9FD0-7A6AE5DBCC95'


            SELECT * FROM T_VWS_SVGElement  
            WHERE SVE_OBJ_UID = 'ABD086E5-45EA-46FB-9B7C-9ED57A44240B'
            AND SVE_OBJT_Code = 'GS' AND SVE_dateDeleted IS NULL

            SELECT SVG_toUNI, UNI_toMeter, T_VWS_Ref_Einheit.* FROM T_VWS_SVG 
            LEFT JOIN T_VWS_Ref_Einheit 
	            ON UNI_UID = SVG_UNI_UID 
            WHERE SVG_UID = '72E739AE-370B-4035-9153-F3E48683978D' 
             */

        }


        static void Main(string[] args)
        {
            // EsriConverter.ESRI.ProjectEsriCoordinatesToWGS84();

            // TestArea();
            // sub();

            // GetInsertPoints();
            // OSM.API.v0_6.Tests.TestBoundingBox();
            // OSM.API.v0_6.Tests.TestPolygonPoints();
            // args = new string[] { "464651233", "95691336", "148117240", "104041936", "43012904", "49589463", "224285187", "58080194", "479999588", "218557958"  };
            // args = new string[] { "224267897", "224269589" };
            // args = new string[] { "58080208", "464651232" };
            // args = new string[] { "58080208", "464651232" };
            args = new string[] { "690355074", "690355077", "690355073" };
            args = new string[] { "690355074" };

            for (int i = 0; i < args.Length; ++i)
            {
                string way = args[i];
                System.Console.WriteLine($"i[{i}]: {way}");

                string script = GetInsertPoints(way);
                System.IO.File.WriteAllText(way + ".sql", script, System.Text.Encoding.UTF8);
                System.Console.WriteLine(script);
            } // Next i 

            System.Console.WriteLine(" --- Press any key to continue --- ");
            while (!System.Console.KeyAvailable)
            {
                System.Threading.Thread.Sleep(100);
            }

        } // End Sub Main 


    } // End Class Program 


} // End Namespace OsmPolygon 

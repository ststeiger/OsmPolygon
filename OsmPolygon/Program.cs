
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


        static void Main(string[] args)
        {
            sub();

            // GetInsertPoints();
            // OSM.API.v0_6.Tests.TestBoundingBox();
            // OSM.API.v0_6.Tests.TestPolygonPoints();
            // args = new string[] { "464651233", "95691336", "148117240", "104041936", "43012904", "49589463", "224285187", "58080194", "479999588", "218557958"  };
            // args = new string[] { "224267897", "224269589" };
            // args = new string[] { "58080208", "464651232" };
            // args = new string[] { "58080208", "464651232" };
            args = new string[] { "690355074", "690355077", "690355073" };


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

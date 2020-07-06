
using Dapper;

    
namespace OsmPolygon
{
    
    
    class Program
    {
        
        
        public static void CreateImportScriptForPolygonByWayId(string[] args)
        {
            // Do it manually: 
            // args = new string[] { "464651233", "95691336", "148117240", "104041936", "43012904", "49589463", "224285187", "58080194", "479999588", "218557958"  };
            // args = new string[] { "224267897", "224269589" }; 
            // args = new string[] { "58080208", "464651232" };  
            // args = new string[] { "58080208", "464651232" }; 
            // args = new string[] { "690355074", "690355077", "690355073" }; 
            // args = new string[] { "690355074" }; 
            // args = new string[] { "326116406", "176675521" }; 
            // args = new string[] { "100787726", "100787718", "337954728"}; 

            // args = new string[] { "37037133" };
            // args = new string[] { "377701803" };
            // args = new string[] { "101768609", "442482822", "442482820" };
            args = new string[] {"231594843"};

            for (int i = 0; i < args.Length; ++i)
            {
                string way = args[i];
                System.Console.WriteLine($"i[{i}]: {way}");

                string script = OSM.API.v0_6.Polygon.GetPointsInsert(way, "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
                System.IO.File.WriteAllText(way + ".sql", script, System.Text.Encoding.UTF8);
                System.Console.WriteLine(script);
            } // Next i 
            
        } // End Sub CreateImportScriptForPolygonByWayId 
        
        
        public static void TestNpgSql()
        {
            var a = new {Test = 5, Result = "Success"};
            var b = new {Test = 3, Result = "foo"};
            var c = new {Test1 = 3, Result = "foo"};

            System.Type t = a.GetType();
            System.Console.WriteLine(t);

            if (object.ReferenceEquals(a.GetType(), b.GetType()))
                System.Console.WriteLine("Two anony = equal");


            Npgsql.NpgsqlConnectionStringBuilder csb = new Npgsql.NpgsqlConnectionStringBuilder();

            csb.Database = "osm_test"; // must be set

            csb.Host = "localhost";
            // csb.Host = "127.0.0.1"; // doesn't work
            // csb.Host = System.Environment.MachineName; // doesn't work 
            csb.Port = 5432;

            csb.IntegratedSecurity = true;
            csb.Username = System.Environment.UserName; // Works when user exists
            // csb.Username = "postgres"; // works as root 

            object obj = null;
            string sql = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public'; ";

            using (System.Data.Common.DbConnection conn = Npgsql.NpgsqlFactory.Instance.CreateConnection())
            {
                conn.ConnectionString = csb.ConnectionString;

                bool ret = conn.ExecuteScalar<bool>(sql);
                System.Console.WriteLine(ret);

                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                using (System.Data.Common.DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    obj = cmd.ExecuteScalar();
                }

                if (conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }

            System.Console.WriteLine(obj);
        } // End Sub TestNpgSql 


        static void Main(string[] args)
        {
            Concave.Hull2.ComputeHull();

            // TestNpgSql();

            Unionizer.Test();

            // Do it all automatically: 
            // OsmPolyonFinder.GetAndInsertBuildingPolygon();
            // EsriConverter.ESRI.Test();
            // MoveMe.MoveMe.TestArea();

            // CreateImportScriptForPolygonByWayId(args);

            // OsmPolygonHelper.Test();

            WaitForExit();
        } // End Sub Main 


        public static void WaitForExit()
        {
            System.Console.Write(System.Environment.NewLine);
            System.Console.Write(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            while (!System.Console.KeyAvailable)
            {
                System.Threading.Thread.Sleep(100);
            } // Whend 
        } // End Sub WaitForExit 
        
        
    } // End Class Program 
    
    
} // End Namespace OsmPolygon 
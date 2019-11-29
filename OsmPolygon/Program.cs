
namespace OsmPolygon
{


    class Program
    {


        static void Main(string[] args)
        {
            // args = new string[] { "464651233", "95691336", "148117240", "104041936", "43012904", "49589463", "224285187", "58080194", "479999588", "218557958"  };
            // args = new string[] { "224267897", "224269589" };
            // args = new string[] { "58080208", "464651232" };
            // args = new string[] { "58080208", "464651232" };
            // args = new string[] { "690355074", "690355077", "690355073" };
            // args = new string[] { "690355074" };
            //args = new string[] { "326116406", "176675521" };

            // Do it all automatically: 
            // OsmPolyonFinder.GetAndInsertBuildingPolygon();

            for (int i = 0; i < args.Length; ++i)
            {
                string way = args[i];
                System.Console.WriteLine($"i[{i}]: {way}");

                string script = OSM.API.v0_6.Polygon.GetPointsInsert(way, "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"); 
                System.IO.File.WriteAllText(way + ".sql", script, System.Text.Encoding.UTF8);
                System.Console.WriteLine(script);
            } // Next i 

            System.Console.WriteLine(" --- Press any key to continue --- ");
            while (!System.Console.KeyAvailable)
            {
                System.Threading.Thread.Sleep(100);
            } // Whend 

        } // End Sub Main 


    } // End Class Program 


} // End Namespace OsmPolygon 

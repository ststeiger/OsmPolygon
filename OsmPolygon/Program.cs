
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


        static void Main(string[] args)
        {
            GetInsertPoints();

            // OSM.API.v0_6.Tests.TestBoundingBox();
            // OSM.API.v0_6.Tests.TestPolygonPoints();


            System.Console.WriteLine(" --- Press any key to continue --- ");
            while (!System.Console.KeyAvailable)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}

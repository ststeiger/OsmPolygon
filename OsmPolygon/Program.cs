
using OsmPolygon.RationalMath;


namespace OsmPolygon
{


    class Program
    {


        public static void CreateImportScriptForPolygonByWayId(string[] args)
        {
            // Do it manually: 
            // args = new string[] { "464651233", "95691336", "148117240", "104041936", "43012904", "49589463", "224285187", "58080194", "479999588", "218557958"  };
            // args = new string[] { "224267897", "224269589" }; 
            // args = new string[] { "37037133" };
            
            args = new string[] { "41087904" };
            string[] gb_uids = new string[] {
                 "83D84846-3841-4AB1-9F1D-5C11A7FCA1FD"
            };



            // gb_uids = null;


            if (gb_uids!= null && gb_uids.Length != args.Length)
            {
                throw new System.Exception("wayid[].length != gb_uids[].length");
            }


            for (int i = 0; i < args.Length; ++i)
            {
                string way = args[i];
                System.Console.WriteLine($"i[{i}]: {way}");

                string script = "";
                if (gb_uids == null)
                    script = OSM.API.v0_6.Polygon.GetPointsInsert(way, null);
                else
                    script = OSM.API.v0_6.Polygon.GetPointsInsert(way, gb_uids[i]);

                System.IO.File.WriteAllText(way + ".sql", script, System.Text.Encoding.UTF8);
                System.Console.WriteLine(script);
            } // Next i 

        } // End Sub CreateImportScriptForPolygonByWayId 



        //get closest Longitude that is on a 15 deg multiple.
        public static double calculate_LongitudeTimeZone(double longitude)
        {
            return (System.Math.Round(longitude / 15d)) * 15d;
        }




        // http://www.luschny.de/math/factorial/FastFactorialFunctions.htm
        // http://www.luschny.de/math/factorial/csharp/FactorialSplit.cs.html
        public static class FactorialSplit
        {
            

            public static System.Numerics.BigInteger Factorial(int n)
            {
                if (n < 0)
                {
                    throw new System.ArgumentOutOfRangeException("n", nameof(FactorialSplit) + "." + nameof(Factorial) + ": n >= 0 required, but was " + n);
                }

                if (n < 2) 
                    return System.Numerics.BigInteger.One;

                ulong[] knownFactorials = new ulong[] { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800, 479001600, 6227020800, 87178291200, 1307674368000, 20922789888000, 355687428096000, 6402373705728000, 121645100408832000 };

                if (n < knownFactorials.Length)
                    return knownFactorials[n];

                System.Numerics.BigInteger p = System.Numerics.BigInteger.One;
                System.Numerics.BigInteger r = System.Numerics.BigInteger.One;
                System.Numerics.BigInteger currentN = System.Numerics.BigInteger.One;

                int h = 0, shift = 0, high = 1;
                int log2n = (int) System.Math.Floor(System.Numerics.BigInteger.Log(n) / System.Numerics.BigInteger.Log(2));
                while (h != n)
                {
                    shift += h;
                    h = n >> log2n--;
                    int len = high;
                    high = (h - 1) | 1;
                    len = (high - len) / 2;

                    if (len > 0)
                    {
                        p *= Product(len, ref currentN);
                        r *= p;
                    }
                }

                return r << shift;
            }


            private static System.Numerics.BigInteger Product(int n, ref System.Numerics.BigInteger currentN)
            {
                int m = n / 2;
                if (m == 0) return currentN += 2;
                if (n == 2) return (currentN += 2) * (currentN += 2);
                return Product(n - m, ref currentN) * Product(m, ref currentN);
            }


        }


        class Continued
        {
            static double Calc(System.Func<int, int[]> f, int n)
            {
                double temp = 0.0;
                for (int ni = n; ni >= 1; ni--)
                {
                    int[] p = f(ni);
                    temp = p[1] / (p[0] + temp);
                }
                return f(0)[0] + temp;
            }


            public struct FractionParameters
            {
                public int Ai;
                public double Bi;
            }

            public static FractionParameters foo(int n, double z)
            {
                int a0 = 0;

                // https://functions.wolfram.com/ElementaryFunctions/ArcTan/10/
                return new FractionParameters()
                {
                    Ai = n > 0 ? (2 * n - 1) : a0,
                    Bi = (n == 0 ? z : n*n* z*z )
                };
            }


            // https://functions.wolfram.com/ElementaryFunctions/ArcTan/10/
            // https://rosettacode.org/wiki/Continued_fraction#C.2B.2B
            // https://rosettacode.org/wiki/Continued_fraction#C.23
            // https://en.wikipedia.org/wiki/Continued_fraction
            // https://en.wikipedia.org/wiki/Inverse_trigonometric_functions#Continued_fractions_for_arctangent
            static double ArcTan(double z, int n)
            {
                double temp = 0.0;
                for (int ni = n; ni >= 1; ni--)
                {
                    // 3 ==> 5, 2.25
                    FractionParameters p = foo(ni, z);
                    temp = p.Bi / (p.Ai + temp);
                } // Next ni 

                double omg = foo(0, z).Ai;
                return  omg + temp;
            } // End Function ArcTan 




            // https://functions.wolfram.com/ElementaryFunctions/ArcTan/10/
            // https://rosettacode.org/wiki/Continued_fraction#C.2B.2B
            // https://rosettacode.org/wiki/Continued_fraction#C.23
            // https://en.wikipedia.org/wiki/Continued_fraction
            // https://en.wikipedia.org/wiki/Inverse_trigonometric_functions#Continued_fractions_for_arctangent
            static double NewArcTan(double z, int maxLevel)
            {
                double temp = 0.0;
                for (int n = maxLevel; n >= 1; n--)
                {
                    // 3 ==> 5, 2.25
                    // 2 ==> 3, 1
                    // 1 ==> 1, 0.25
                    int Ai = (2 * n - 1);
                    double Bi = n*n * z*z;
                    
                    temp = Bi / (Ai + temp);
                } // Next n 
                
                // 0, 0.5
                return z / temp;
            } // End Function NewArcTan 


            public static void Test()
            {
                MyRational.LogTest();

                double at = NewArcTan(0.5, 3); // Excel: arctan(0.5) = 0.463647609 rad = 26.565051177098°
                System.Console.WriteLine(at);


                System.Collections.Generic.List<System.Func<int, int[]>> fList = new System.Collections.Generic.List<System.Func<int, int[]>>();

                // f(n) => int[]{ai, bi};
                fList.Add(n => new int[] { n > 0 ? 2 : 1, 1 });
                fList.Add(n => new int[] { n > 0 ? n : 2, n > 1 ? (n - 1) : 1 });
                fList.Add(n => new int[] { n > 0 ? 6 : 3, (int)System.Math.Pow(2 * n - 1, 2) });


                fList.Add(n => new int[] { n > 0 ? 6 : 3, (int)System.Math.Pow(2 * n - 1, 2) });


                foreach (var f in fList)
                {
                    System.Console.WriteLine(Calc(f, 200));
                }
            }
        }




        static void Main(string[] args)
        {
            // Continued.Test();

            // RationalMath.MyRational.Test();
            // FixedMath.FixedDecimalTests.TestMe(123.456M);

            // OsmPolygon.Concave.COORDS.TestCoordinateConversion.Test(); // Bad 
            // Concave.Hull2.ComputeHull(); // Bad




            // EsriConverter.ESRI.Test();
            // MoveMe.MoveMe.TestArea();

            // OsmPolygonHelper.Test();


            // Do it all automatically: 
            /////////////// OsmPolyonFinder.GetAndInsertBuildingPolygon();

            // Do it manually 
            CreateImportScriptForPolygonByWayId(args);

            // Union of polygons 
            /////// Unionizer.Test(); // Merge N polygons to 1 Polygon with Concave Hull


            // Unionizer.UnionizePolygonsByWayId();


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

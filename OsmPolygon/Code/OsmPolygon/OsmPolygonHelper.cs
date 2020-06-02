
namespace OsmPolygon
{
    using OpenToolkit.Mathematics;


    // A C# program to check if a given DecimalVector2  
    // lies inside a given polygon 
    // Refer https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/ 
    // and https://www.geeksforgeeks.org/how-to-check-if-a-given-DecimalVector2-lies-inside-a-polygon/
    // for explanation of functions onSegment(), 
    // orientation() and doIntersect()  
    public class OsmPolygonHelper
    {

        // Define Infinite (Using INT_MAX  
        // caused overflow problems) 
        static int INF = 10000;

        // Given three colinear points p, q, r,  
        // the function checks if point q lies 
        // on line segment 'pr' 
        private static bool onSegment(DecimalVector2 p, DecimalVector2 q, DecimalVector2 r)
        {
            if (q.X <= System.Math.Max(p.X, r.X) &&
                q.X >= System.Math.Min(p.X, r.X) &&
                q.Y <= System.Math.Max(p.Y, r.Y) &&
                q.Y >= System.Math.Min(p.Y, r.Y))
            {
                return true;
            }
            return false;
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        private static int orientation(DecimalVector2 p, DecimalVector2 q, DecimalVector2 r)
        {
            decimal val = (q.Y - p.Y) * (r.X - q.X) -
                      (q.X - p.X) * (r.Y - q.Y);

            if (val == 0)
            {
                return 0; // colinear 
            }
            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        // The function that returns true if  
        // line segment 'p1q1' and 'p2q2' intersect. 
        private static bool doIntersect(DecimalVector2 p1, DecimalVector2 q1,
                                DecimalVector2 p2, DecimalVector2 q2)
        {
            // Find the four orientations needed for  
            // general and special cases 
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4)
            {
                return true;
            }

            // Special Cases 
            // p1, q1 and p2 are colinear and 
            // p2 lies on segment p1q1 
            if (o1 == 0 && onSegment(p1, p2, q1))
            {
                return true;
            }

            // p1, q1 and p2 are colinear and 
            // q2 lies on segment p1q1 
            if (o2 == 0 && onSegment(p1, q2, q1))
            {
                return true;
            }

            // p2, q2 and p1 are colinear and 
            // p1 lies on segment p2q2 
            if (o3 == 0 && onSegment(p2, p1, q2))
            {
                return true;
            }

            // p2, q2 and q1 are colinear and 
            // q1 lies on segment p2q2 
            if (o4 == 0 && onSegment(p2, q1, q2))
            {
                return true;
            }

            // Doesn't fall in any of the above cases 
            return false;
        }


        // Returns true if the point p lies  
        // inside the polygon[] with n vertices 
        public static bool IsInside(System.Collections.Generic.IList<DecimalVector2> polygon,  DecimalVector2 p)
        {
            int n = polygon.Count;


            // There must be at least 3 vertices in polygon[] 
            if (n < 3)
            {
                return false;
            }

            // Create a point for line segment from p to infinite 
            DecimalVector2 extreme = new DecimalVector2(INF, p.Y);

            // Count intersections of the above line  
            // with sides of polygon 
            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % n;

                // Check if the line segment from 'p' to  
                // 'extreme' intersects with the line  
                // segment from 'polygon[i]' to 'polygon[next]' 
                if (doIntersect(polygon[i],
                                polygon[next], p, extreme))
                {
                    // If the point 'p' is colinear with line  
                    // segment 'i-next', then check if it lies  
                    // on segment. If it lies, return true, otherwise false 
                    if (orientation(polygon[i], p, polygon[next]) == 0)
                    {
                        return onSegment(polygon[i], p,
                                         polygon[next]);
                    }
                    count++;
                }
                i = next;
            } while (i != 0);

            // Return true if count is odd, false otherwise 
            return (count % 2 == 1); // Same as (count%2 == 1) 
        }


        public static void Test()
        {
            DecimalVector2[] polygon1 = new DecimalVector2[] 
            {
                 new DecimalVector2(47.03687500000000000000M, 8.29712870000000000000M)
                ,new DecimalVector2(47.03681430000000000000M, 8.29710690000000000000M)
                ,new DecimalVector2(47.03659660000000000000M, 8.29702870000000000000M)
                ,new DecimalVector2(47.03666160000000000000M, 8.29665210000000000000M)
                ,new DecimalVector2(47.03651580000000000000M, 8.29659780000000000000M)
                ,new DecimalVector2(47.03657590000000000000M, 8.29625910000000000000M)
                ,new DecimalVector2(47.03700750000000000000M, 8.29641350000000000000M)
                ,new DecimalVector2(47.03696060000000000000M, 8.29669370000000000000M)
                ,new DecimalVector2(47.03691760000000000000M, 8.29667890000000000000M)
                ,new DecimalVector2(47.03688220000000000000M, 8.29688350000000000000M)
                ,new DecimalVector2(47.03691530000000000000M, 8.29689480000000000000M)
                ,new DecimalVector2(47.03687500000000000000M, 8.29712870000000000000M)
            };


            DecimalVector2 p = new DecimalVector2(47.03677485655230000000M, 8.29674839973449900000M);
            // DecimalVector2 p = new DecimalVector2(46.03687500000000000000M, 8.29712870000000000000M);

            bool inside = IsInside(polygon1, p);
            System.Console.WriteLine(inside);
        }


    } // End Class 


} // End Namespace 

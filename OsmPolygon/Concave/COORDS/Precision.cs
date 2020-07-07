
namespace OsmPolygon.Concave.COORDS
{


    public class Precision
    {
        public static readonly double EPSILON = FastMath.longBitsToDouble(4368491638549381120L);
        public static readonly double SAFE_MIN = FastMath.longBitsToDouble(4503599627370496L);
        private static readonly long EXPONENT_OFFSET = 1023L;
        private static readonly long SGN_MASK = -9223372036854775808L;
        private static readonly int SGN_MASK_FLOAT = -2147483648;

        private Precision()
        { }


        public static int compareTo(double x, double y, double eps)
        {
            if (equals(x, y, eps))
            {
                return 0;
            }
            else
            {
                return x < y ? -1 : 1;
            }
        }

        public static int compareTo(double x, double y, int maxUlps)
        {
            if (equals(x, y, maxUlps))
            {
                return 0;
            }
            else
            {
                return x < y ? -1 : 1;
            }
        }

        public static bool equals(float x, float y)
        {
            return equals(x, y, 1);
        }

        public static bool equalsIncludingNaN(float x, float y)
        {
            return System.Single.IsNaN(x) && System.Single.IsNaN(y) || equals(x, y, 1);
        }

        public static bool equals(float x, float y, float eps)
        {
            return equals(x, y, 1) || FastMath.abs(y - x) <= eps;
        }

        public static bool equalsIncludingNaN(float x, float y, float eps)
        {
            return equalsIncludingNaN(x, y) || FastMath.abs(y - x) <= eps;
        }

        public static bool equals(float x, float y, int maxUlps)
        {
            int xInt = FastMath.floatToIntBits(x);
            int yInt = FastMath.floatToIntBits(y);
            if (xInt < 0)
            {
                xInt = -2147483648 - xInt;
            }

            if (yInt < 0)
            {
                yInt = -2147483648 - yInt;
            }

            bool isEqual = FastMath.abs(xInt - yInt) <= maxUlps;
            return isEqual && !System.Single.IsNaN(x) && !System.Single.IsNaN(y);
        }

        public static bool equalsIncludingNaN(float x, float y, int maxUlps)
        {
            return System.Single.IsNaN(x) && System.Single.IsNaN(y) || equals(x, y, maxUlps);
        }

        public static bool equals(double x, double y)
        {
            return equals(x, y, 1);
        }

        public static bool equalsIncludingNaN(double x, double y)
        {
            return System.Double.IsNaN(x) && System.Double.IsNaN(y) || equals(x, y, 1);
        }

        public static bool equals(double x, double y, double eps)
        {
            return equals(x, y, 1) || FastMath.abs(y - x) <= eps;
        }

        public static bool equalsWithRelativeTolerance(double x, double y, double eps)
        {
            if (equals(x, y, 1))
            {
                return true;
            }
            else
            {
                double absoluteMax = FastMath.max(FastMath.abs(x), FastMath.abs(y));
                double relativeDifference = FastMath.abs((x - y) / absoluteMax);
                return relativeDifference <= eps;
            }
        }

        public static bool equalsIncludingNaN(double x, double y, double eps)
        {
            return equalsIncludingNaN(x, y) || FastMath.abs(y - x) <= eps;
        }

        public static bool equals(double x, double y, int maxUlps)
        {
            long xInt = FastMath.doubleToLongBits(x);
            long yInt = FastMath.doubleToLongBits(y);
            if (xInt < 0L)
            {
                xInt = -9223372036854775808L - xInt;
            }

            if (yInt < 0L)
            {
                yInt = -9223372036854775808L - yInt;
            }

            bool isEqual = FastMath.abs(xInt - yInt) <= (long)maxUlps;
            return isEqual && !System.Double.IsNaN(x) && !System.Double.IsNaN(y);
        }

        public static bool equalsIncludingNaN(double x, double y, int maxUlps)
        {
            return System.Double.IsNaN(x) && System.Double.IsNaN(y) || equals(x, y, maxUlps);
        }



    public static double representableDelta(double x, double originalDelta)
    {
        return x + originalDelta - x;
    }
}

}

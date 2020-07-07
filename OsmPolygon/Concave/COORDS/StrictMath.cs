
namespace OsmPolygon.Concave.COORDS
{
    class StrictMath
    {

        // https://stackoverflow.com/questions/1971645/is-System.Math-ieeeremainderx-y-equivalent-to-xy
        public static double IEEEremainder1(double f1, double f2)
        {
            return System.Math.IEEERemainder(f1, f2);
        }

        public static double IEEERemainder(double x, double y)
        {
            double regularMod = x % y;
            if (double.IsNaN(regularMod))
            {
                return double.NaN;
            }
            if (regularMod == 0)
            {
                if (double.IsNegative(x))
                {
                    return (-0); // Double.NegativeZero;
                }
            }
            double alternativeResult;
            alternativeResult = regularMod - (System.Math.Abs(y) * System.Math.Sign(x));
            if (System.Math.Abs(alternativeResult) == System.Math.Abs(regularMod))
            {
                double divisionResult = x / y;
                double roundedResult = System.Math.Round(divisionResult);
                if (System.Math.Abs(roundedResult) > System.Math.Abs(divisionResult))
                {
                    return alternativeResult;
                }
                else
                {
                    return regularMod;
                }
            }
            if (System.Math.Abs(alternativeResult) < System.Math.Abs(regularMod))
            {
                return alternativeResult;
            }
            else
            {
                return regularMod;
            }
        }

    }
}

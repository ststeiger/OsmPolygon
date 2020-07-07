using System;
using System.Collections.Generic;
using System.Text;

namespace OsmPolygon.Concave.COORDS
{
    //
    // Source code recreated from a .class file by IntelliJ IDEA
    // (powered by Fernflower decompiler)
    //

    public enum OrderDirection
    {
        INCREASING,
        DECREASING
    }

    public class DimensionMismatchException // extends MathIllegalNumberException
        : System.Exception
    {
        /** Serializable version Id. */
        private static long serialVersionUID = -8415396756375798143L;
        /** Correct dimension. */
        private int dimension;

        /**
         * Construct an exception from the mismatched dimensions.
         *
         * @param specific Specific context information pattern.
         * @param wrong Wrong dimension.
         * @param expected Expected dimension.
         */
        public DimensionMismatchException(int wrong,
                                          int expected)
                : base("Received " + wrong.ToString() + ", expected " + expected.ToString())
        {
        }

    }



    public class MathArrays
    {
        private static int SPLIT_FACTOR = 134217729;

        private MathArrays()
        {
        }



        public static double linearCombination(double[] a, double[] b) // throws DimensionMismatchException
        {
            int len = a.Length;
            if (len != b.Length)
            {
                throw new DimensionMismatchException(len, b.Length);
            }
            else
            {
                double[] prodHigh = new double[len];
                double prodLowSum = 0.0D;

                double result;
                for (int i = 0; i < len; ++i)
                {
                    double ai = a[i];
                    double ca = 1.34217729E8D * ai;
                    double aHigh = ca - (ca - ai);
                    double aLow = ai - aHigh;
                    double bi = b[i];
                    result = 1.34217729E8D * bi;
                    double bHigh = result - (result - bi);
                    double bLow = bi - bHigh;
                    prodHigh[i] = ai * bi;
                    double prodLow = aLow * bLow - (prodHigh[i] - aHigh * bHigh - aLow * bHigh - aHigh * bLow);
                    prodLowSum += prodLow;
                }

                double prodHighCur = prodHigh[0];
                double prodHighNext = prodHigh[1];
                double sHighPrev = prodHighCur + prodHighNext;
                double sPrime = sHighPrev - prodHighNext;
                double sLowSum = prodHighNext - (sHighPrev - sPrime) + (prodHighCur - sPrime);
                int lenMinusOne = len - 1;

                for (int i = 1; i < lenMinusOne; ++i)
                {
                    prodHighNext = prodHigh[i + 1];
                    double sHighCur = sHighPrev + prodHighNext;
                    sPrime = sHighCur - prodHighNext;
                    sLowSum += prodHighNext - (sHighCur - sPrime) + (sHighPrev - sPrime);
                    sHighPrev = sHighCur;
                }

                result = sHighPrev + prodLowSum + sLowSum;
                if (Double.IsNaN(result))
                {
                    result = 0.0D;

                    for (int i = 0; i < len; ++i)
                    {
                        result += a[i] * b[i];
                    }
                }

                return result;
            }
        }

        public static double linearCombination(double a1, double b1, double a2, double b2)
        {
            double ca1 = 1.34217729E8D * a1;
            double a1High = ca1 - (ca1 - a1);
            double a1Low = a1 - a1High;
            double cb1 = 1.34217729E8D * b1;
            double b1High = cb1 - (cb1 - b1);
            double b1Low = b1 - b1High;
            double prod1High = a1 * b1;
            double prod1Low = a1Low * b1Low - (prod1High - a1High * b1High - a1Low * b1High - a1High * b1Low);
            double ca2 = 1.34217729E8D * a2;
            double a2High = ca2 - (ca2 - a2);
            double a2Low = a2 - a2High;
            double cb2 = 1.34217729E8D * b2;
            double b2High = cb2 - (cb2 - b2);
            double b2Low = b2 - b2High;
            double prod2High = a2 * b2;
            double prod2Low = a2Low * b2Low - (prod2High - a2High * b2High - a2Low * b2High - a2High * b2Low);
            double s12High = prod1High + prod2High;
            double s12Prime = s12High - prod2High;
            double s12Low = prod2High - (s12High - s12Prime) + (prod1High - s12Prime);
            double result = s12High + prod1Low + prod2Low + s12Low;
            if (Double.IsNaN(result))
            {
                result = a1 * b1 + a2 * b2;
            }

            return result;
        }

        public static double linearCombination(double a1, double b1, double a2, double b2, double a3, double b3)
        {
            double ca1 = 1.34217729E8D * a1;
            double a1High = ca1 - (ca1 - a1);
            double a1Low = a1 - a1High;
            double cb1 = 1.34217729E8D * b1;
            double b1High = cb1 - (cb1 - b1);
            double b1Low = b1 - b1High;
            double prod1High = a1 * b1;
            double prod1Low = a1Low * b1Low - (prod1High - a1High * b1High - a1Low * b1High - a1High * b1Low);
            double ca2 = 1.34217729E8D * a2;
            double a2High = ca2 - (ca2 - a2);
            double a2Low = a2 - a2High;
            double cb2 = 1.34217729E8D * b2;
            double b2High = cb2 - (cb2 - b2);
            double b2Low = b2 - b2High;
            double prod2High = a2 * b2;
            double prod2Low = a2Low * b2Low - (prod2High - a2High * b2High - a2Low * b2High - a2High * b2Low);
            double ca3 = 1.34217729E8D * a3;
            double a3High = ca3 - (ca3 - a3);
            double a3Low = a3 - a3High;
            double cb3 = 1.34217729E8D * b3;
            double b3High = cb3 - (cb3 - b3);
            double b3Low = b3 - b3High;
            double prod3High = a3 * b3;
            double prod3Low = a3Low * b3Low - (prod3High - a3High * b3High - a3Low * b3High - a3High * b3Low);
            double s12High = prod1High + prod2High;
            double s12Prime = s12High - prod2High;
            double s12Low = prod2High - (s12High - s12Prime) + (prod1High - s12Prime);
            double s123High = s12High + prod3High;
            double s123Prime = s123High - prod3High;
            double s123Low = prod3High - (s123High - s123Prime) + (s12High - s123Prime);
            double result = s123High + prod1Low + prod2Low + prod3Low + s12Low + s123Low;
            if (Double.IsNaN(result))
            {
                result = a1 * b1 + a2 * b2 + a3 * b3;
            }

            return result;
        }

        public static double linearCombination(double a1, double b1, double a2, double b2, double a3, double b3, double a4, double b4)
        {
            double ca1 = 1.34217729E8D * a1;
            double a1High = ca1 - (ca1 - a1);
            double a1Low = a1 - a1High;
            double cb1 = 1.34217729E8D * b1;
            double b1High = cb1 - (cb1 - b1);
            double b1Low = b1 - b1High;
            double prod1High = a1 * b1;
            double prod1Low = a1Low * b1Low - (prod1High - a1High * b1High - a1Low * b1High - a1High * b1Low);
            double ca2 = 1.34217729E8D * a2;
            double a2High = ca2 - (ca2 - a2);
            double a2Low = a2 - a2High;
            double cb2 = 1.34217729E8D * b2;
            double b2High = cb2 - (cb2 - b2);
            double b2Low = b2 - b2High;
            double prod2High = a2 * b2;
            double prod2Low = a2Low * b2Low - (prod2High - a2High * b2High - a2Low * b2High - a2High * b2Low);
            double ca3 = 1.34217729E8D * a3;
            double a3High = ca3 - (ca3 - a3);
            double a3Low = a3 - a3High;
            double cb3 = 1.34217729E8D * b3;
            double b3High = cb3 - (cb3 - b3);
            double b3Low = b3 - b3High;
            double prod3High = a3 * b3;
            double prod3Low = a3Low * b3Low - (prod3High - a3High * b3High - a3Low * b3High - a3High * b3Low);
            double ca4 = 1.34217729E8D * a4;
            double a4High = ca4 - (ca4 - a4);
            double a4Low = a4 - a4High;
            double cb4 = 1.34217729E8D * b4;
            double b4High = cb4 - (cb4 - b4);
            double b4Low = b4 - b4High;
            double prod4High = a4 * b4;
            double prod4Low = a4Low * b4Low - (prod4High - a4High * b4High - a4Low * b4High - a4High * b4Low);
            double s12High = prod1High + prod2High;
            double s12Prime = s12High - prod2High;
            double s12Low = prod2High - (s12High - s12Prime) + (prod1High - s12Prime);
            double s123High = s12High + prod3High;
            double s123Prime = s123High - prod3High;
            double s123Low = prod3High - (s123High - s123Prime) + (s12High - s123Prime);
            double s1234High = s123High + prod4High;
            double s1234Prime = s1234High - prod4High;
            double s1234Low = prod4High - (s1234High - s1234Prime) + (s123High - s1234Prime);
            double result = s1234High + prod1Low + prod2Low + prod3Low + prod4Low + s12Low + s123Low + s1234Low;

            if (Double.IsNaN(result))
            {
                result = a1 * b1 + a2 * b2 + a3 * b3 + a4 * b4;
            }

            return result;
        }

        public static bool equals(float[] x, float[] y)
        {
            if (x != null && y != null)
            {
                if (x.Length != y.Length)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < x.Length; ++i)
                    {
                        if (!Precision.equals(x[i], y[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            else
            {
                return !(x == null ^ y == null);
            }
        }

        public static bool equalsIncludingNaN(float[] x, float[] y)
        {
            if (x != null && y != null)
            {
                if (x.Length != y.Length)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < x.Length; ++i)
                    {
                        if (!Precision.equalsIncludingNaN(x[i], y[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            else
            {
                return !(x == null ^ y == null);
            }
        }

        public static bool equals(double[] x, double[] y)
        {
            if (x != null && y != null)
            {
                if (x.Length != y.Length)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < x.Length; ++i)
                    {
                        if (!Precision.equals(x[i], y[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            else
            {
                return !(x == null ^ y == null);
            }
        }

        public static bool equalsIncludingNaN(double[] x, double[] y)
        {
            if (x != null && y != null)
            {
                if (x.Length != y.Length)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < x.Length; ++i)
                    {
                        if (!Precision.equalsIncludingNaN(x[i], y[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            else
            {
                return !(x == null ^ y == null);
            }
        }



    }

}

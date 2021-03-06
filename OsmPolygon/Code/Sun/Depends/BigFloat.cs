﻿
// https://github.com/Osinko/BigFloat/blob/master/src/BigFloat.cs
namespace OsmPolygon.sunrisesunset
{
    using OsmPolygon.RationalMath;
    using System.Numerics;


    // https://github.com/openjdk-mirror/jdk7u-jdk/blob/master/src/share/classes/java/math/BigDecimal.java
    // https://github.com/Osinko/BigFloat/blob/master/src/BigFloat.cs
    // Implementation of BigDecimal 
    [System.Serializable]
    public class BigFloat 
        : System.IComparable, System.IComparable<BigFloat>, System.IEquatable<BigFloat>
    {
        private BigInteger numerator;
        private BigInteger denominator;

        public static readonly BigFloat One = new BigFloat(1);
        public static readonly BigFloat Zero = new BigFloat(0);
        public static readonly BigFloat MinusOne = new BigFloat(-1);
        public static readonly BigFloat OneHalf = new BigFloat(1, 2);

        public static BigFloat valueOf(double x)
        {
            return new BigFloat(x);
        }

        public static BigFloat valueOf(int x)
        {
            return new BigFloat(x);
        }


        public double doubleValue()
        {
            return (double)this;
        }

        public int intValue()
        {
            return (int)(double)this;
        }


        public System.Numerics.BigInteger NumeratorValue
        {
            get
            {
                return this.numerator;
            }
        }

        public System.Numerics.BigInteger DenominatorValue
        {
            get
            {
                return this.denominator;
            }
        }


        public string toPlainString()
        {
            return this.ToString();
        }
        /*
             // https://docs.oracle.com/javase/7/docs/api/java/math/RoundingMode.html
    public enum RoundingMode
    {
       X CEILING // Rounding mode to round towards positive infinity.
       X ,DOWN // Rounding mode to round towards zero.
       X ,FLOOR // Rounding mode to round towards negative infinity.
        ,HALF_DOWN // Rounding mode to round towards "nearest neighbor" unless both neighbors are equidistant, in which case round down.
        ,HALF_EVEN // Rounding mode to round towards the "nearest neighbor" unless both neighbors are equidistant, in which case, round towards the even neighbor.
        ,HALF_UP // Rounding mode to round towards "nearest neighbor" unless both neighbors are equidistant, in which case round up.
       X ,UNNECESSARY //Rounding mode to assert that the requested operation has an exact result, hence no rounding is necessary.
       X ,UP // Rounding mode to round away from zero.
    }
         */

        public BigFloat Round2(RoundingMode roundingMode)
        {
            //get remainder. Over divisor see if it is > new BigFloat(0.5)
            BigFloat value = BigFloat.Decimals(this);

            int order = value.CompareTo(OneHalf);




            if (order >= 0)
                this.Ceil();
            else
                this.Floor();


            if (roundingMode == RoundingMode.UP)
            {
                // Rounding mode to round away from zero.
                if (value.Sign <= 0)
                    this.Floor();
                else
                    this.Ceil();
            }
            else if (roundingMode == RoundingMode.CEILING)
            {
                // Towards +infinity
                this.Ceil();
            }
            else if (roundingMode == RoundingMode.DOWN)
            {
                // Towards zero
                if (value.Sign <= 0)
                    this.Ceil();
                else
                    this.Floor();
            }
            else if (roundingMode == RoundingMode.FLOOR)
            {
                // Towards -infinity
                this.Floor();
            }
            else if (roundingMode == RoundingMode.HALF_UP)
            {
                // HALF_UP: Mercantile rounding
                // ,HALF_UP // Rounding mode to round towards "nearest neighbor" unless both neighbors are equidistant, 
                //               in which case round up.
                if (order >= 0)
                    this.Ceil();
                else
                    this.Floor();
            }
            else if (roundingMode == RoundingMode.HALF_DOWN)
            {
                // Towards -infinity
                // ,HALF_DOWN // Rounding mode to round towards "nearest neighbor" unless both neighbors are equidistant, 
                //               in which case round down.
                this.Round();
            }
            else if (roundingMode == RoundingMode.HALF_EVEN)
            {
                // HALF_EVEN: Banker's rounding
            }









            else if (roundingMode == RoundingMode.UNNECESSARY)
            {  // Rounding prohibited
                throw new System.ArithmeticException("Rounding necessary");
            }
            else
            {
            }





            if (value.CompareTo(OneHalf) >= 0)
                this.Ceil();
            else
                this.Floor();

            return this;
        }


        // https://www.tutorialspoint.com/java/math/bigdecimal_setscale_rm_roundingmode.htm
        public BigFloat setScale(int scale, RoundingMode mode)
        {
            // bg1 = new BigDecimal("123.12678");
            // bg2 = bg1.setScale(2, RoundingMode.FLOOR);
            // 123.12678 after changing the scale to 2 and rounding is 123.12

            BigInteger bigPrecision = BigInteger.Pow(10, scale);
            BigFloat newValue = new BigFloat(this);

            newValue = newValue * bigPrecision;

            // ,UP // Rounding mode to round away from zero.
            //  CEILING // Rounding mode to round towards positive infinity.
            // ,HALF_UP // Rounding mode to round towards "nearest neighbor" unless both neighbors are equidistant, in which case round up.

            // ,DOWN // Rounding mode to round towards zero.
            // ,FLOOR // Rounding mode to round towards negative infinity.
            // ,HALF_DOWN // Rounding mode to round towards "nearest neighbor" unless both neighbors are equidistant, in which case round down.

            // ,HALF_EVEN // Rounding mode to round towards the "nearest neighbor" unless both neighbors are equidistant, in which case, round towards the even neighbor.
            // ,UNNECESSARY //Rounding mode to assert that the requested operation has an exact result, hence no rounding is necessary.

            // HALF_EVEN: Banker's rounding
            // HALF_UP: Mercantile rounding
            if (mode == RoundingMode.CEILING || mode == RoundingMode.UP)
            {
                newValue = newValue.Ceil();
            }
            else if (mode == RoundingMode.FLOOR || mode == RoundingMode.DOWN || mode == RoundingMode.HALF_DOWN)
            {
                newValue = newValue.Floor();
            }
            else if (mode != RoundingMode.UNNECESSARY) // mode == RoundingMode.HALF_UP
                newValue = newValue.Round();

            newValue = newValue / bigPrecision;

            return newValue;
        }


        public BigFloat Divide(BigFloat divisor, int scale, RoundingMode mode)
        {
            BigFloat foo = this.Divide(divisor);
            BigFloat ret = foo.setScale(scale, mode);
            return ret;
        }

        public BigFloat Divide(BigFloat divisor, MathContext mc)
        {
            BigFloat ret = this.Divide(divisor);
            // TODO: mc? 
            return ret;
        }


        private BigFloat Normalize()
        {
            if (this.denominator == 0)
            {
                throw new System.DivideByZeroException("In function " + nameof(Normalize));
            }

            System.Numerics.BigInteger n = System.Numerics.BigInteger.GreatestCommonDivisor(this.numerator, this.denominator);
            this.numerator /= n; // lowest
            this.denominator /= n; //    terms

            if (this.denominator < 0)
            {
                // make denom positive
                this.denominator = -this.denominator;
                this.numerator = -this.numerator;
            }

            return this;
        }


        public int Sign
        {
            get
            {
                this.Normalize();

                if (this.numerator == 0)
                    return 0;

                if (this.denominator == 0)
                    throw new System.DivideByZeroException("In function " + nameof(Sign));

                if (this.numerator > 0)
                    return 1;

                return -1;
            }
        }


        //constructors
        public BigFloat()
        {
            numerator = BigInteger.Zero;
            denominator = BigInteger.One;
        }
        public BigFloat(string value)
        {
            BigFloat bf = Parse(value);
            this.numerator = bf.numerator;
            this.denominator = bf.denominator;
        }
        public BigFloat(BigInteger numerator, BigInteger denominator)
        {
            this.numerator = numerator;
            if (denominator == 0)
                throw new System.ArgumentException("denominator equals 0");
            this.denominator = BigInteger.Abs(denominator);
        }
        public BigFloat(BigInteger value)
        {
            this.numerator = value;
            this.denominator = BigInteger.One;
        }
        public BigFloat(BigFloat value)
        {
            if (BigFloat.Equals(value, null))
            {
                this.numerator = BigInteger.Zero;
                this.denominator = BigInteger.One;
            }
            else
            {

                this.numerator = value.numerator;
                this.denominator = value.denominator;
            }
        }
        public BigFloat(ulong value)
        {
            numerator = new BigInteger(value);
            denominator = BigInteger.One;
        }
        public BigFloat(long value)
        {
            numerator = new BigInteger(value);
            denominator = BigInteger.One;
        }
        public BigFloat(uint value)
        {
            numerator = new BigInteger(value);
            denominator = BigInteger.One;
        }
        public BigFloat(int value)
        {
            numerator = new BigInteger(value);
            denominator = BigInteger.One;
        }
        public BigFloat(float value) : this(value.ToString("N99"))
        {
        }
        public BigFloat(double value) : this(value.ToString("N99"))
        {
        }
        public BigFloat(decimal value) : this(value.ToString("N99"))
        {
        }

        //non-static methods
        public BigFloat Add(BigFloat other)
        {
            if (BigFloat.Equals(other, null))
                throw new System.ArgumentNullException("other");

            this.numerator = this.numerator * other.denominator + other.numerator * this.denominator;
            this.denominator *= other.denominator;
            return this;
        }
        public BigFloat Subtract(BigFloat other)
        {
            if (BigFloat.Equals(other, null))
                throw new System.ArgumentNullException("other");

            this.numerator = this.numerator * other.denominator - other.numerator * this.denominator;
            this.denominator *= other.denominator;
            return this;
        }
        public BigFloat Multiply(BigFloat other)
        {
            if (BigFloat.Equals(other, null))
                throw new System.ArgumentNullException("other");

            this.numerator *= other.numerator;
            this.denominator *= other.denominator;
            return this;
        }
        public BigFloat Divide(BigFloat other)
        {
            if (BigInteger.Equals(other, null))
                throw new System.ArgumentNullException("other");
            if (other.numerator == 0)
                throw new System.DivideByZeroException("other");

            this.numerator *= other.denominator;
            this.denominator *= other.numerator;
            return this;
        }
        public BigFloat Remainder(BigFloat other)
        {
            if (BigInteger.Equals(other, null))
                throw new System.ArgumentNullException("other");

            //b = a mod n
            //remainder = a - floor(a/n) * n

            BigFloat result = this - Floor(this / other) * other;

            this.numerator = result.numerator;
            this.denominator = result.denominator;


            return this;
        }
        public BigFloat DivideRemainder(BigFloat other, out BigFloat remainder)
        {
            this.Divide(other);

            remainder = BigFloat.Remainder(this, other);

            return this;
        }
        public BigFloat Pow(int exponent)
        {
            if (numerator.IsZero)
            {
                // Nothing to do
            }
            else if (exponent < 0)
            {
                BigInteger savedNumerator = numerator;
                numerator = BigInteger.Pow(denominator, -exponent);
                denominator = BigInteger.Pow(savedNumerator, -exponent);
            }
            else
            {
                numerator = BigInteger.Pow(numerator, exponent);
                denominator = BigInteger.Pow(denominator, exponent);
            }

            return this;
        }
        public BigFloat Abs()
        {
            numerator = BigInteger.Abs(numerator);
            return this;
        }
        public BigFloat Negate()
        {
            numerator = BigInteger.Negate(numerator);
            return this;
        }
        public BigFloat Inverse()
        {
            BigInteger temp = numerator;
            numerator = denominator;
            denominator = temp;
            return this;
        }
        public BigFloat Increment()
        {
            numerator += denominator;
            return this;
        }
        public BigFloat Decrement()
        {
            numerator -= denominator;
            return this;
        }
        public BigFloat Ceil()
        {
            if (numerator < 0)
                numerator -= BigInteger.Remainder(numerator, denominator);
            else
                numerator += denominator - BigInteger.Remainder(numerator, denominator);

            Factor();
            return this;
        }
        public BigFloat Floor()
        {
            this.numerator = 5;
            this.denominator = 3;

            var bi = BigInteger.Remainder(numerator, denominator);



            System.Console.WriteLine(bi);


            /*
            if (numerator < 0)
                numerator += denominator - BigInteger.Remainder(numerator, denominator);
            else
                numerator -= BigInteger.Remainder(numerator, denominator);
            */

            Factor();
            return this;
        }
        public BigFloat Round()
        {
            //get remainder. Over divisor see if it is > new BigFloat(0.5)
            BigFloat value = BigFloat.Decimals(this);

            if (value.CompareTo(OneHalf) >= 0)
                this.Ceil();
            else
                this.Floor();

            return this;
        }
        public BigFloat Truncate()
        {
            numerator -= BigInteger.Remainder(numerator, denominator);
            Factor();
            return this;
        }
        public BigFloat Decimals()
        {
            BigInteger result = BigInteger.Remainder(numerator, denominator);

            return new BigFloat(result, denominator);
        }
        public BigFloat ShiftDecimalLeft(int shift)
        {
            if (shift < 0)
                return ShiftDecimalRight(-shift);

            numerator *= BigInteger.Pow(10, shift);
            return this;
        }
        public BigFloat ShiftDecimalRight(int shift)
        {
            if (shift < 0)
                return ShiftDecimalLeft(-shift);
            denominator *= BigInteger.Pow(10, shift);
            return this;
        }
        public double Sqrt()
        {
            return System.Math.Pow(10, BigInteger.Log10(numerator) / 2) / System.Math.Pow(10, BigInteger.Log10(denominator) / 2);
        }
        public double Log10()
        {
            return BigInteger.Log10(numerator) - BigInteger.Log10(denominator);
        }
        public double Log(double baseValue)
        {
            return BigInteger.Log(numerator, baseValue) - BigInteger.Log(numerator, baseValue);
        }
        public override string ToString()
        {
            //default precision = 100
            return ToString(100);
        }
        public string ToString(int precision, bool trailingZeros = false)
        {
            Factor();

            BigInteger remainder;
            BigInteger result = BigInteger.DivRem(numerator, denominator, out remainder);

            if (remainder == 0 && trailingZeros)
                return result + ".0";
            else if (remainder == 0)
                return result.ToString();


            BigInteger decimals = BigInteger.Abs((numerator * BigInteger.Pow(10, precision)) / denominator);

            if (decimals == 0 && trailingZeros)
                return result + ".0";
            else if (decimals == 0)
                return result.ToString();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            while (precision-- > 0 && decimals > 0)
            {
                sb.Append(decimals % 10);
                decimals /= 10;
            }


            if (trailingZeros)
            {
                char[] ca = sb.ToString().ToCharArray();
                System.Array.Reverse(ca);

                // return result + "." + new string(sb.ToString().Reverse().ToArray());
                return (Sign < 0 ? "-" : "") + result + "." + new string(ca);
            }
            // else

            char[] caa = sb.ToString().ToCharArray();
            System.Array.Reverse(caa);

            // return result + "." + new string(sb.ToString().Reverse().ToArray()).TrimEnd(new char[] { '0' });
            return (Sign < 0 ? "-" : "") + result + "." + new string(caa).TrimEnd(new char[] { '0' });
        }


        public string ToMixString()
        {
            Factor();

            BigInteger remainder;
            BigInteger result = BigInteger.DivRem(numerator, denominator, out remainder);

            if (remainder == 0)
                return result.ToString();
            else
                return result + ", " + remainder + "/" + denominator;
        }

        public string ToRationalString()
        {
            Factor();

            return numerator + " / " + denominator;
        }
        public int CompareTo(BigFloat other)
        {
            if (BigFloat.Equals(other, null))
                throw new System.ArgumentNullException("other");

            //Make copies
            BigInteger one = this.numerator;
            BigInteger two = other.numerator;

            //cross multiply
            one *= other.denominator;
            two *= this.denominator;

            //test
            return BigInteger.Compare(one, two);
        }
        public int CompareTo(object other)
        {
            if (other == null)
                throw new System.ArgumentNullException("other");

            if (!(other is BigFloat))
                throw new System.ArgumentException("other is not a BigFloat");

            return CompareTo((BigFloat)other);
        }
        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }

            return this.numerator == ((BigFloat)other).numerator && this.denominator == ((BigFloat)other).denominator;
        }
        public bool Equals(BigFloat other)
        {
            return (other.numerator == this.numerator && other.denominator == this.denominator);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        
        //static methods
        public static bool Equals(object left, object right)
        {
            if (left == null && right == null) return true;
            else if (left == null || right == null) return false;
            else if (left.GetType() != right.GetType()) return false;
            else
                return (((BigInteger)left).Equals((BigInteger)right));
        }
        public static string ToString(BigFloat value)
        {
            return value.ToString();
        }

        public static BigFloat Inverse(BigFloat value)
        {
            return (new BigFloat(value)).Inverse();
        }
        public static BigFloat Decrement(BigFloat value)
        {
            return (new BigFloat(value)).Decrement();
        }
        public static BigFloat Negate(BigFloat value)
        {
            return (new BigFloat(value)).Negate();
        }
        public static BigFloat Increment(BigFloat value)
        {
            return (new BigFloat(value)).Increment();
        }
        public static BigFloat Abs(BigFloat value)
        {
            return (new BigFloat(value)).Abs();
        }
        public static BigFloat Add(BigFloat left, BigFloat right)
        {
            return (new BigFloat(left)).Add(right);
        }
        public static BigFloat Subtract(BigFloat left, BigFloat right)
        {
            return (new BigFloat(left)).Subtract(right);
        }
        public static BigFloat Multiply(BigFloat left, BigFloat right)
        {
            return (new BigFloat(left)).Multiply(right);
        }
        public static BigFloat Divide(BigFloat left, BigFloat right)
        {
            return (new BigFloat(left)).Divide(right);
        }
        public static BigFloat Pow(BigFloat value, int exponent)
        {
            return (new BigFloat(value)).Pow(exponent);
        }
        public static BigFloat Remainder(BigFloat left, BigFloat right)
        {
            return (new BigFloat(left)).Remainder(right);
        }
        public static BigFloat DivideRemainder(BigFloat left, BigFloat right, out BigFloat remainder)
        {
            return (new BigFloat(left)).DivideRemainder(right, out remainder);
        }
        public static BigFloat Decimals(BigFloat value)
        {
            return value.Decimals();
        }
        public static BigFloat Truncate(BigFloat value)
        {
            return (new BigFloat(value)).Truncate();
        }
        public static BigFloat Ceil(BigFloat value)
        {
            return (new BigFloat(value)).Ceil();
        }
        public static BigFloat Floor(BigFloat value)
        {
            return (new BigFloat(value)).Floor();
        }
        public static BigFloat Round(BigFloat value)
        {
            return (new BigFloat(value)).Round();
        }


        // =ROUND(num*(100/multipleof);0)/(100/multipleof) // multipleof = 1, 3, 5 etc.
        // =ROUND(num*(100/multipleof);0)/(100/multipleof) // multipleof = 1, 3, 5 etc.
        public static BigFloat Round(BigFloat value, int scale)
        {
            BigInteger bigPrecision = BigInteger.Pow(10, scale);
            BigFloat newValue = new BigFloat(value);

            newValue = newValue * bigPrecision;
            newValue = newValue.Round();
            newValue = newValue / bigPrecision;

            return newValue;
        }




        public static BigFloat Parse(string value)
        {
            if (value == null)
                throw new System.ArgumentNullException("value");

            value.Trim();
            value = value.Replace(",", "");
            int pos = value.IndexOf('.');
            value = value.Replace(".", "");

            if (pos < 0)
            {
                //no decimal point
                BigInteger numerator = BigInteger.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                return (new BigFloat(numerator)).Factor();
            }
            else
            {
                //decimal point (length - pos - 1)
                BigInteger numerator = BigInteger.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                BigInteger denominator = BigInteger.Pow(10, value.Length - pos);

                return (new BigFloat(numerator, denominator)).Factor();
            }
        }
        public static BigFloat ShiftDecimalLeft(BigFloat value, int shift)
        {
            return (new BigFloat(value)).ShiftDecimalLeft(shift);
        }
        public static BigFloat ShiftDecimalRight(BigFloat value, int shift)
        {
            return (new BigFloat(value)).ShiftDecimalRight(shift);
        }
        public static bool TryParse(string value, out BigFloat result)
        {
            try
            {
                result = BigFloat.Parse(value);
                return true;
            }
            catch (System.ArgumentNullException)
            {
                result = null;
                return false;
            }
            catch (System.FormatException)
            {
                result = null;
                return false;
            }
        }
        public static int Compare(BigFloat left, BigFloat right)
        {
            if (BigFloat.Equals(left, null))
                throw new System.ArgumentNullException("left");
            if (BigFloat.Equals(right, null))
                throw new System.ArgumentNullException("right");

            return (new BigFloat(left)).CompareTo(right);
        }
        public static double Log10(BigFloat value)
        {
            return (new BigFloat(value)).Log10();
        }
        public static double Log(BigFloat value, double baseValue)
        {
            return (new BigFloat(value)).Log(baseValue);
        }
        public static double Sqrt(BigFloat value)
        {
            return (new BigFloat(value)).Sqrt();
        }

        public static BigFloat operator -(BigFloat value)
        {
            return (new BigFloat(value)).Negate();
        }
        public static BigFloat operator -(BigFloat left, BigFloat right)
        {
            return (new BigFloat(left)).Subtract(right);
        }
        public static BigFloat operator --(BigFloat value)
        {
            return value.Decrement();
        }
        public static BigFloat operator +(BigFloat left, BigFloat right)
        {
            return (new BigFloat(left)).Add(right);
        }
        public static BigFloat operator +(BigFloat value)
        {
            return (new BigFloat(value)).Abs();
        }
        public static BigFloat operator ++(BigFloat value)
        {
            return value.Increment();
        }
        public static BigFloat operator %(BigFloat left, BigFloat right)
        {
            return (new BigFloat(left)).Remainder(right);
        }
        public static BigFloat operator *(BigFloat left, BigFloat right)
        {
            return (new BigFloat(left)).Multiply(right);
        }
        public static BigFloat operator /(BigFloat left, BigFloat right)
        {
            return (new BigFloat(left)).Divide(right);
        }
        public static BigFloat operator >>(BigFloat value, int shift)
        {
            return (new BigFloat(value)).ShiftDecimalRight(shift);
        }
        public static BigFloat operator <<(BigFloat value, int shift)
        {
            return (new BigFloat(value)).ShiftDecimalLeft(shift);
        }
        public static BigFloat operator ^(BigFloat left, int right)
        {
            return (new BigFloat(left)).Pow(right);
        }
        public static BigFloat operator ~(BigFloat value)
        {
            return (new BigFloat(value)).Inverse();
        }

        public static bool operator !=(BigFloat left, BigFloat right)
        {
            return Compare(left, right) != 0;
        }
        public static bool operator ==(BigFloat left, BigFloat right)
        {
            return Compare(left, right) == 0;
        }
        public static bool operator <(BigFloat left, BigFloat right)
        {
            return Compare(left, right) < 0;
        }
        public static bool operator <=(BigFloat left, BigFloat right)
        {
            return Compare(left, right) <= 0;
        }
        public static bool operator >(BigFloat left, BigFloat right)
        {
            return Compare(left, right) > 0;
        }
        public static bool operator >=(BigFloat left, BigFloat right)
        {
            return Compare(left, right) >= 0;
        }

        public static bool operator true(BigFloat value)
        {
            return value != 0;
        }
        public static bool operator false(BigFloat value)
        {
            return value == 0;
        }

        public static explicit operator decimal(BigFloat value)
        {
            if (decimal.MinValue > value) throw new System.OverflowException("value is less than System.decimal.MinValue.");
            if (decimal.MaxValue < value) throw new System.OverflowException("value is greater than System.decimal.MaxValue.");

            return (decimal)value.numerator / (decimal)value.denominator;
        }
        public static explicit operator double(BigFloat value)
        {
            if (double.MinValue > value) throw new System.OverflowException("value is less than System.double.MinValue.");
            if (double.MaxValue < value) throw new System.OverflowException("value is greater than System.double.MaxValue.");

            return (double)value.numerator / (double)value.denominator;
        }
        public static explicit operator float(BigFloat value)
        {
            if (float.MinValue > value) throw new System.OverflowException("value is less than System.float.MinValue.");
            if (float.MaxValue < value) throw new System.OverflowException("value is greater than System.float.MaxValue.");

            return (float)value.numerator / (float)value.denominator;
        }

        //byte, sbyte, 
        public static implicit operator BigFloat(byte value)
        {
            return new BigFloat((uint)value);
        }
        public static implicit operator BigFloat(sbyte value)
        {
            return new BigFloat((int)value);
        }
        public static implicit operator BigFloat(short value)
        {
            return new BigFloat((int)value);
        }
        public static implicit operator BigFloat(ushort value)
        {
            return new BigFloat((uint)value);
        }
        public static implicit operator BigFloat(int value)
        {
            return new BigFloat(value);
        }
        public static implicit operator BigFloat(long value)
        {
            return new BigFloat(value);
        }
        public static implicit operator BigFloat(uint value)
        {
            return new BigFloat(value);
        }
        public static implicit operator BigFloat(ulong value)
        {
            return new BigFloat(value);
        }
        public static implicit operator BigFloat(decimal value)
        {
            return new BigFloat(value);
        }
        public static implicit operator BigFloat(double value)
        {
            return new BigFloat(value);
        }
        public static implicit operator BigFloat(float value)
        {
            return new BigFloat(value);
        }
        public static implicit operator BigFloat(BigInteger value)
        {
            return new BigFloat(value);
        }
        public static explicit operator BigFloat(string value)
        {
            return new BigFloat(value);
        }

        private BigFloat Factor()
        {
            //factoring can be very slow. So use only when neccessary (ToString, and comparisons)

            if (denominator == 1)
                return this;

            //factor numerator and denominator
            BigInteger factor = BigInteger.GreatestCommonDivisor(numerator, denominator);

            numerator /= factor;
            denominator /= factor;

            return this;
        }

    }


}

﻿
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System;
using System.Security.Cryptography.X509Certificates;

namespace OsmPolygon.RationalMath
{



    // https://docs.oracle.com/javase/7/docs/api/java/math/RoundingMode.html
    public enum RoundingMode
    {
        CEILING // Rounding mode to round towards positive infinity.
        , DOWN // Rounding mode to round towards zero.
        , FLOOR // Rounding mode to round towards negative infinity.
        , HALF_DOWN // Rounding mode to round towards "nearest neighbor" unless both neighbors are equidistant, in which case round down.
        , HALF_EVEN // Rounding mode to round towards the "nearest neighbor" unless both neighbors are equidistant, in which case, round towards the even neighbor.
        , HALF_UP // Rounding mode to round towards "nearest neighbor" unless both neighbors are equidistant, in which case round up.
        , UNNECESSARY //Rounding mode to assert that the requested operation has an exact result, hence no rounding is necessary.
        , UP // Rounding mode to round away from zero.
    }

    // https://github.com/openjdk/jdk/blob/master/src/java.base/share/classes/java/math/MathContext.java
    public class MathContext
    {
        //  π = 22⁄7 = ​355⁄113.
        //  The latter fraction is the best possible rational approximation of π 
        // using fewer than five decimal digits in the numerator and denominator
        // , being accurate to 6 decimal places
        // 86953/27678, accurate to 8 decimal places
        // https://en.wikipedia.org/wiki/Mil%C3%BC#:~:text=355113%20is%20the%20best,than%2013748629.
        // http://qin.laya.com/tech_projects_approxpi.html
        // Num.                Den.                = Result                                   (Accuracy                                 )
        // 2646693125139304345/ 842468587426513207 = 3.14159265358979323846264338327950288418 ( 0.00000000000000000000000000000000000001) [37]

        // https://en.wikipedia.org/wiki/Approximations_of_%CF%80

        public MyRational Epsilon;
        public RoundingMode Rounding;

        // The default-values for the constructor - DRY ! 
        private const int DEFAULT_SCALE = 10;
        private const RoundingMode DEFAULT_ROUNDING = RoundingMode.HALF_UP;

        private static int IntPow(int basis, uint exponent)
        {
            if (exponent < 0)
                throw new System.ArgumentException("Exponent cannot be < 0 !");

            int ret = 1;
            while (exponent != 0)
            {
                if ((exponent & 1) == 1)
                    ret *= basis;

                basis *= basis;
                exponent >>= 1;
            }
            return ret;
        }

        public MathContext(int scale, RoundingMode mode)
        {
            // this.Epsilon = new MyRational(System.Numerics.BigInteger.One, System.Numerics.BigInteger.Pow(10, scale));
            this.Epsilon = new MyRational(System.Numerics.BigInteger.One, 10000);
            this.Rounding = mode;
        }

        public MathContext(int scale)
            : this(scale, DEFAULT_ROUNDING)
        { }

        public MathContext(RoundingMode mode)
            : this(DEFAULT_SCALE, mode)
        { }


        public MathContext()
            : this(DEFAULT_SCALE, DEFAULT_ROUNDING)
        { }

    } // End Class MathContext 



    // «BigRational»
    public class MyRational
        : System.IComparable, System.IComparable<MyRational>, System.IEquatable<MyRational>
    {

        public readonly System.Numerics.BigInteger Numerator;
        public readonly System.Numerics.BigInteger Denominator;


        public static void Test()
        {
            double gold = (1.0d + System.Math.Sqrt(5.0d)) / 2.0d;
            MyRational golden = new MyRational(gold);



            MyRational rat = new MyRational("0.5");
            MyRational ata = rat.Arctan();
            System.Console.WriteLine(ata);

            // System.Console.WriteLine(ata); // {0.4621615816987241548078304229099378605228827730020354712665967136200503624217050367267952488982260125}
            // arctan(0.5) = 0,463647609


            MyRational reciprocalValue = new MyRational(5, 3).Pow(-1);
            System.Console.WriteLine(reciprocalValue);



            MyRational a = new MyRational(3);
            MyRational b = new MyRational(5);



            if (a > b)
                System.Console.WriteLine("true");
            else
                System.Console.WriteLine("false");

            MyRational num = new MyRational(4, 2);
            MyRational sqrt = MyRational.Sqrt(num, new MyRational(1, 10000));
            MyRational rt = Root(num, 4, new MyRational(1, 10000));

            string mix = rt.ToMixString();
            System.Console.WriteLine(mix);

            MyRational negativeNegative = new MyRational(-5, -3);
            MyRational negativePositive = new MyRational(-5, 3);
            MyRational positiveNegative = new MyRational(5, -3);

            negativeNegative = negativeNegative.Pow(-3);
            negativePositive = negativePositive.Pow(-3);
            positiveNegative = positiveNegative.Pow(-3);

            System.Console.WriteLine(negativeNegative);
            System.Console.WriteLine(negativePositive);
            System.Console.WriteLine(positiveNegative);


            MyRational a53 = new MyRational(5, 3);
            MyRational a27 = new MyRational(2, 7);

            MyRational div = a53.Divide(a27);
            MyRational rem = a53.Mod(a27);
            MyRational dec = div.Decimals();

            System.Console.WriteLine(golden);
            System.Console.WriteLine(div);
            System.Console.WriteLine(rem);
            System.Console.WriteLine(dec);
        }


        // Use Euclid's algorithm to calculate the
        // greatest common divisor (GCD) of two numbers.
        public static System.Numerics.BigInteger GCD(System.Numerics.BigInteger a, System.Numerics.BigInteger b)
        {
            System.Numerics.BigInteger divisor = System.Numerics.BigInteger.Abs(a);
            System.Numerics.BigInteger remainder = System.Numerics.BigInteger.Abs(b);

            while (remainder != System.Numerics.BigInteger.Zero)
            {
                System.Numerics.BigInteger oldRemainder = remainder;
                remainder = divisor % remainder;
                divisor = oldRemainder;
            } // Whend

            return divisor;
        }


        // Return the least common multiple (LCM) of two numbers.
        public static System.Numerics.BigInteger LCM(System.Numerics.BigInteger a, System.Numerics.BigInteger b)
        {
            return a * b / GCD(a, b);
        }


        // Use Euclid's algorithm to calculate the
        // greatest common divisor (GCD) of two numbers.
        public static long GCD(long a, long b)
        {
            long divisor = System.Math.Abs(a);
            long remainder = System.Math.Abs(b);

            while (remainder != System.Numerics.BigInteger.Zero)
            {
                long oldRemainder = remainder;
                remainder = divisor % remainder;
                divisor = oldRemainder;
            } // Whend

            return divisor;
        }

        // Return the least common multiple (LCM) of two numbers.
        public static long LCM(long a, long b)
        {
            return a * b / GCD(a, b);
        }


        // Use Euclid's algorithm to calculate the
        // greatest common divisor (GCD) of two numbers.
        public static int GCD(int a, int b)
        {
            int divisor = System.Math.Abs(a);
            int remainder = System.Math.Abs(b);

            while (remainder != System.Numerics.BigInteger.Zero)
            {
                int oldRemainder = remainder;
                remainder = divisor % remainder;
                divisor = oldRemainder;
            } // Whend

            return divisor;
        }


        // Return the least common multiple (LCM) of two numbers.
        public static int LCM(int a, int b)
        {
            return a * b / GCD(a, b);
        }


        private static void Normalize(out System.Numerics.BigInteger numerator, out System.Numerics.BigInteger denominator)
        {
            // System.Numerics.BigInteger.GreatestCommonDivisor
            System.Numerics.BigInteger n = GCD(numerator, denominator);
            numerator /= n;         // lowest
            denominator /= n;       //    terms
            if (denominator < 0)
            {
                // make denom positive
                denominator = -denominator;
                numerator = -numerator;
            }

        }



        // https://www.quora.com/What-is-the-best-rational-approximation-of-pi-Let-best-be-the-difference-between-the-number-of-digits-used-to-represent-the-rational-and-the-number-of-accurate-digits-in-the-decimal-expansion?share=1
        // http://qin.laya.com/tech_projects_approxpi.html
        // Num./Den. = Result (Accuracy )
        // 355/ 113 = 3.14159292035398230088495575221238938053 (-0.00000026676418906242231236893288649634)
        // public static readonly MyRational PI = new MyRational(355, 113);

        // 52163/16604 = 3.14159238737653577451216574319441098530 ( 0.00000026621325746395047764008509189889)
        // public static readonly MyRational PI = new MyRational(52163, 16604);

        // 2646693125139304345/ 842468587426513207 = 3.14159265358979323846264338327950288418 ( 0.00000000000000000000000000000000000001)
        public static readonly MyRational PI = new MyRational(2646693125139304345, 842468587426513207);
        public static readonly MyRational PiSquared = new MyRational(2646693125139304345, 842468587426513207).Pow(2);
        public static readonly MyRational FivePiSquared = new MyRational(5).Multiply(PI.Pow(2));

        public static readonly MyRational PiHalf = PI.Divide(2);
        public static readonly MyRational PiFourth = PI.Divide(4);




        // https://www.tandfonline.com/doi/full/10.1080/0020739X.2017.1352043
        // which evaluates to a decimal giving correctly the first 20 digits of e:
        public static readonly MyRational e = new MyRational(System.Numerics.BigInteger.Parse("611070150698522592097"), System.Numerics.BigInteger.Parse("224800145555521536000"));
        // http://www.acsu.buffalo.edu/~adamcunn/spring2017/Week3Notebook.html
        public static readonly MyRational e_Exact = new MyRational(System.Numerics.BigInteger.Parse("337310723185584470837549"), System.Numerics.BigInteger.Parse("124089680346647887872000"));


        // https://www.theproblemsite.com/ask/2017/09/approximation-for-the-golden-ratio
        // Golden ratio 1.6180
        // (1+sqrt(5))/2 ==> accurate to 0.00000000001 ? 
        // public static readonly MyRational GoldenRatio_COMPUTED = new MyRational(161803398874989, 100000000000000);
        public static readonly MyRational GoldenRatio = new MyRational(317811, 196418);

        // 299'792'458 m/s
        public static readonly MyRational SpeedOfLightInVacuum = new MyRational(299792458, 1);
        // 1 Meter = 1/299'792'458  m
        public static readonly MyRational LightSecondsPerMeter = new MyRational(1, 299792458);

        public static readonly MyRational Zero = new MyRational(0);
        public static readonly MyRational One = new MyRational(1);
        public static readonly MyRational MinusOne = new MyRational(-1);
        public static readonly MyRational Ten = new MyRational(10);
        public static readonly MyRational Thousand = new MyRational(1000);

        public static readonly MyRational OneHalf = new MyRational(1, 2);




        // Plack constant
        // Avogardo
        // light speed
        // gravitational constant
        // bolthmann constant
        // square root two


        public static MyRational Parse(string value)
        {
            if (value == null)
                throw new System.ArgumentNullException("value");

            value.Trim();
            value = value.Replace(",", "");
            int pos = value.IndexOf('.');
            value = value.Replace(".", "");

            if (pos < 0)
            {
                // no decimal point
                System.Numerics.BigInteger numerator = System.Numerics.BigInteger.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                return new MyRational(numerator);
            }
            else
            {
                // decimal point (length - pos - 1)
                System.Numerics.BigInteger numerator = System.Numerics.BigInteger.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                System.Numerics.BigInteger denominator = System.Numerics.BigInteger.Pow(10, value.Length - pos);

                return new MyRational(numerator, denominator);
            }
        }



        public MyRational(string number)
        {
            MyRational r = Parse(number);
            this.Numerator = r.Numerator;
            this.Denominator = r.Denominator;
        }

        public MyRational(float value) : this(value.ToString("N99", System.Globalization.CultureInfo.InvariantCulture))
        { }

        public MyRational(double value) : this(value.ToString("N99", System.Globalization.CultureInfo.InvariantCulture))
        { }

        public MyRational(decimal value) : this(value.ToString("N99", System.Globalization.CultureInfo.InvariantCulture))
        { }


        // Create a fraction given numerator and denominator.
        public MyRational(System.Numerics.BigInteger numerator, System.Numerics.BigInteger denominator)
        {
            if (denominator == 0)
            {
                throw new System.ArgumentException("Denominator cannot be ZERO.");
            }

            Normalize(out numerator, out denominator);

            this.Numerator = numerator;
            this.Denominator = denominator;
        }


        public MyRational(System.Numerics.BigInteger numerator)
            : this(numerator, System.Numerics.BigInteger.One)
        { }

        public MyRational(int numerator)
            : this(new System.Numerics.BigInteger(numerator), System.Numerics.BigInteger.One)
        { }

        public MyRational(uint numerator)
            : this(new System.Numerics.BigInteger(numerator), System.Numerics.BigInteger.One)
        { }

        public MyRational(long numerator)
            : this(new System.Numerics.BigInteger(numerator), System.Numerics.BigInteger.One)
        { }

        public MyRational(ulong numerator)
            : this(new System.Numerics.BigInteger(numerator), System.Numerics.BigInteger.One)
        { }



        private static int MyCompare(MyRational a, MyRational b)
        {
            if (a is null && b is null)
                return 0;

            if (a is null)
                return -1;

            if (b is null)
                return 1;

            MyRational delta = a.Subtract(b);
            return delta.Sign;
        }



        #region System.IComparable

        int System.IComparable.CompareTo(object obj)
        {
            if (!(obj is MyRational))
                throw new System.ArgumentException("System.IComparable.CompareTo: obj is not a MyRational");

            return MyCompare(this, (MyRational)obj);
        }


        public int CompareTo(object other)
        {
            return ((System.IComparable)this).CompareTo(other);
        }

        #endregion // System.IComparable



        #region System.IComparable<MyRational>
        int IComparable<MyRational>.CompareTo(MyRational other)
        {
            return MyCompare(this, other);
        }



        // Return a number that is positive, zero or negative, respectively, if
        // the value of this Rational is bigger than f ==> +1,
        // the values of this Rational and f are equal or ==> 0
        // the value of this Rational is smaller than f ==> -1
        public int CompareTo(MyRational other)
        {
            return MyCompare(this, other);
        }



        #endregion // System.IComparable<MyRational>



        #region System.IEquatable<MyRational>

        bool IEquatable<MyRational>.Equals(MyRational other)
        {
            if (other == null)
                throw new System.ArgumentNullException("other");

            return this.Numerator == other.Numerator && this.Denominator == other.Denominator;
        }

        public bool Equals(MyRational other)
        {
            return ((IEquatable<MyRational>)this).Equals(other);
        }

        #endregion // System.IEquatable<MyRational>





        public override bool Equals(object other)
        {
            if (!(other is MyRational))
                throw new System.ArgumentException("other is not a MyRational");

            MyRational rat = (MyRational)other;
            return this.Equals(rat);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        // Return a decimal approximation to the fraction.
        public decimal ToDecimal()
        {
            return (decimal)this.Numerator / (decimal)this.Denominator;
        }


        public int Sign
        {
            get
            {
                // the fraction is normalized at this point ! 
                if (this.Numerator == 0)
                    return 0;

                if (this.Denominator == 0)
                    throw new System.DivideByZeroException("In function " + nameof(Sign));

                if (this.Numerator > 0)
                    return 1;

                return -1;
            }
        }



        public MyRational Negate()
        {
            return new MyRational(-this.Numerator, this.Denominator);
        }


        // Reciprocal
        public MyRational Inverse()
        {
            return new MyRational(this.Denominator, this.Numerator);
        }


        public MyRational Increment()
        {
            return new MyRational(this.Numerator + this.Denominator, this.Denominator);
        }


        public MyRational Decrement()
        {
            return new MyRational(this.Numerator - this.Denominator, this.Denominator);
        }


        public MyRational Abs()
        {
            System.Numerics.BigInteger a = System.Numerics.BigInteger.Abs(this.Numerator);
            System.Numerics.BigInteger b = System.Numerics.BigInteger.Abs(this.Denominator);

            return new MyRational(a, b);
        }


        // thisᵉˣᵖᵒⁿᵉⁿᵗ = Numeratorᵉˣᵖᵒⁿᵉⁿᵗ / Denominatorᵉˣᵖᵒⁿᵉⁿᵗ
        public MyRational Pow(int exponent)
        {
            if (exponent < 0)
            {
                exponent = -exponent;

                MyRational dividend = new MyRational(1, System.Numerics.BigInteger.Pow(this.Numerator, exponent));
                MyRational divisor = new MyRational(1, System.Numerics.BigInteger.Pow(this.Denominator, exponent));
                MyRational quotient = dividend.Divide(divisor);
                return quotient;
            }

            return new MyRational(
                  System.Numerics.BigInteger.Pow(this.Numerator, exponent)
                , System.Numerics.BigInteger.Pow(this.Denominator, exponent)
            );
        }

        public MyRational Pow(System.Numerics.BigInteger exponent)
        {
            if (exponent < 0)
            {
                exponent = -exponent;

                MyRational dividend = new MyRational(1, MyRational.Pow(this.Numerator, exponent));
                MyRational divisor = new MyRational(1, MyRational.Pow(this.Denominator, exponent));
                MyRational quotient = dividend.Divide(divisor);
                return quotient;
            }

            return new MyRational(
                  MyRational.Pow(this.Numerator, exponent)
                , MyRational.Pow(this.Denominator, exponent)
            );
        }


        // private static System.Numerics.BigInteger Pow(System.Numerics.BigInteger value, System.Numerics.BigInteger exponent)




        // https://www.geeksforgeeks.org/find-root-of-a-number-using-newtons-method/
        public static MyRational Sqrt(MyRational radicand, MyRational epsilon)
        {
            // Assuming the sqrt of radicand as radicand only 
            MyRational x = radicand;

            // The closed guess will be stored in the root 
            MyRational root;

            // To count the number of iterations 
            int count = 0;

            while (true)
            {
                count++;

                // Calculate more closed x 
                root = MyRational.OneHalf * (x + (radicand / x));

                // Check for closeness 
                if ((root - x).Abs() < epsilon)
                    break;

                // Update root 
                x = root;
            } // Whend 

            return root;
        } // End Function SquareRoot 


        // nth-root(radicand, degree) = root 
        // https://en.wikipedia.org/wiki/Nth_root#Using_Newton.27s_method
        public static MyRational Root(MyRational radicand, System.Numerics.BigInteger degree, MyRational epsilon)
        {
            if (degree == 2)
                return Sqrt(radicand, epsilon); // Should be lightly faster

            // Assuming the sqrt of n as n only 
            MyRational x = radicand;

            // The closed guess will be stored in the root 
            MyRational root;

            System.Numerics.BigInteger degreeMinusOne = degree - 1;

            MyRational one_over_degree = new MyRational(1, degree);
            MyRational degree_minus_one = new MyRational(degreeMinusOne);

            // To count the number of iterations 
            int count = 0;
            while (true)
            {
                count++;

                // Calculate more closed x 
                root = one_over_degree * (degree_minus_one * x + (radicand / x.Pow(degreeMinusOne)));
                // x_{k+1}={\frac {1}{n}}\left({(n-1)x_{k}+{\frac {A}{x_{k}^{n-1}}}}\right)

                // Check for closeness 
                if ((root - x).Abs() < epsilon)
                    break;

                // Update root 
                x = root;
            } // Whend 

            return root;
        } // End Function RootN 


        // this + other 
        public MyRational Add(MyRational other)
        {
            return new MyRational(this.Numerator * other.Denominator + this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }


        // https://www.mathsisfun.com/numbers/addition.html
        // augend + addend = sum
        public MyRational Add(int addend)
        {
            MyRational other = new MyRational(addend);

            return new MyRational(this.Numerator * other.Denominator + this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }

        public MyRational Add(uint addend)
        {
            MyRational other = new MyRational(addend);

            return new MyRational(this.Numerator * other.Denominator + this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }


        public MyRational Add(long addend)
        {
            MyRational other = new MyRational(addend);

            return new MyRational(this.Numerator * other.Denominator + this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }


        public MyRational Add(ulong addend)
        {
            MyRational other = new MyRational(addend);

            return new MyRational(this.Numerator * other.Denominator + this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }


        // this - other
        public MyRational Subtract(MyRational other)
        {
            return new MyRational(this.Numerator * other.Denominator - this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }


        // https://www.mathsisfun.com/numbers/subtraction.html
        // minuend - subtrahend = difference
        public MyRational Subtract(int subtrahend)
        {
            MyRational other = new MyRational(subtrahend);

            return new MyRational(this.Numerator * other.Denominator - this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }

        public MyRational Subtract(uint subtrahend)
        {
            MyRational other = new MyRational(subtrahend);

            return new MyRational(this.Numerator * other.Denominator - this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }

        public MyRational Subtract(long subtrahend)
        {
            MyRational other = new MyRational(subtrahend);

            return new MyRational(this.Numerator * other.Denominator - this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }

        public MyRational Subtract(ulong subtrahend)
        {
            MyRational other = new MyRational(subtrahend);

            return new MyRational(this.Numerator * other.Denominator - this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }


        // this * other
        public MyRational Multiply(MyRational other)
        {
            return new MyRational(this.Numerator * other.Numerator, this.Denominator * other.Denominator);
        }

        // https://en.wikipedia.org/wiki/Logarithm
        // multiplier * multiplicant = product 
        public MyRational Multiply(int factor)
        {
            MyRational other = new MyRational(factor);

            return new MyRational(this.Numerator * other.Numerator, this.Denominator * other.Denominator);
        }


        public MyRational Multiply(uint factor)
        {
            MyRational other = new MyRational(factor);

            return new MyRational(this.Numerator * other.Numerator, this.Denominator * other.Denominator);
        }

        public MyRational Multiply(long factor)
        {
            MyRational other = new MyRational(factor);

            return new MyRational(this.Numerator * other.Numerator, this.Denominator * other.Denominator);
        }

        public MyRational Multiply(ulong factor)
        {
            MyRational other = new MyRational(factor);

            return new MyRational(this.Numerator * other.Numerator, this.Denominator * other.Denominator);
        }


        // this/other
        public MyRational Divide(MyRational other)
        {
            return new MyRational(this.Numerator * other.Denominator, this.Denominator * other.Numerator);
        }

        // 55 ÷ 9 = 6 and 1
        // Dividend ÷ Divisor = Quotient and Remainder
        public MyRational Divide(int divisor)
        {
            MyRational other = new MyRational(divisor);

            return new MyRational(this.Numerator * other.Denominator, this.Denominator * other.Numerator);
        }

        public MyRational Divide(uint divisor)
        {
            MyRational other = new MyRational(divisor);

            return new MyRational(this.Numerator * other.Denominator, this.Denominator * other.Numerator);
        }

        public MyRational Divide(long divisor)
        {
            MyRational other = new MyRational(divisor);

            return new MyRational(this.Numerator * other.Denominator, this.Denominator * other.Numerator);
        }

        public MyRational Divide(ulong divisor)
        {
            MyRational other = new MyRational(divisor);

            return new MyRational(this.Numerator * other.Denominator, this.Denominator * other.Numerator);
        }




        // this \ other = (5/3)\(2/7) = 5 rem 5/21 ==> 5
        public MyRational Div(MyRational other)
        {
            System.Numerics.BigInteger num = this.Numerator * other.Denominator;
            System.Numerics.BigInteger den = this.Denominator * other.Numerator;

            return new MyRational(num / den);
        }


        // this % other = (5/3)%(2/7) = 5 rem 5/21 ==> 5/21 = 0.238095238
        // this % other ==> this - (this / other) * other
        // WARNING: This is NOT the decimal part when the denominator is not 1 ! 
        public MyRational Mod(MyRational other)
        {
            // The 'modulus' operation is defined as: a % n ==> a - (a / n) * n
            MyRational div = this.Div(other);
            MyRational divXother = div.Multiply(other);
            MyRational rem = this.Subtract(divXother);
            return rem;
        }


        // (5/3) / (2/7) = 35/6 = 5 5/6 = 5.833333333 ==> 0.833333333 aka 5/6
        // this - (this div 1) = 35/6-((35/6)\1) = 5/6
        public MyRational Decimals()
        {
            MyRational div = this.Div(MyRational.One);
            MyRational res = this.Subtract(div);

            return res;
        }



        public static MyRational operator ~(MyRational value)
        {
            // Correct ?
            return value.Inverse();
        }

        public static MyRational operator -(MyRational value)
        {
            return value.Negate();
        }
        public static MyRational operator -(MyRational left, MyRational right)
        {
            return left.Subtract(right);
        }


        public static MyRational operator --(MyRational value)
        {
            return value.Decrement();
        }

        public static MyRational operator +(MyRational left, MyRational right)
        {
            return left.Add(right);
        }
        public static MyRational operator +(MyRational value)
        {
            // return (new MyRational(value)).Abs();
            return value;
        }
        public static MyRational operator ++(MyRational value)
        {
            return value.Increment();
        }

        public static MyRational operator %(MyRational left, MyRational right)
        {
            return left.Mod(right);
        }
        public static MyRational operator *(MyRational left, MyRational right)
        {
            return left.Multiply(right);
        }
        public static MyRational operator /(MyRational left, MyRational right)
        {
            return left.Divide(right);
        }


        public static bool operator !=(MyRational left, MyRational right)
        {
            return MyCompare(left, right) != 0;
        }
        public static bool operator ==(MyRational left, MyRational right)
        {
            return MyCompare(left, right) == 0;
        }


        public static bool operator <(MyRational left, MyRational right)
        {
            return MyCompare(left, right) < 0;
        }
        public static bool operator <=(MyRational left, MyRational right)
        {
            return MyCompare(left, right) <= 0;
        }
        public static bool operator >(MyRational left, MyRational right)
        {
            return MyCompare(left, right) > 0;
        }
        public static bool operator >=(MyRational left, MyRational right)
        {
            return MyCompare(left, right) >= 0;
        }


        public static bool operator true(MyRational value)
        {
            return value.Numerator != System.Numerics.BigInteger.Zero;
        }
        public static bool operator false(MyRational value)
        {
            return value.Numerator == System.Numerics.BigInteger.Zero;
        }


        public string ToString(int precision, bool trailingZeros = false)
        {
            System.Numerics.BigInteger remainder;
            System.Numerics.BigInteger result = System.Numerics.BigInteger.DivRem(this.Numerator, this.Denominator, out remainder);

            if (remainder == 0 && trailingZeros)
                return result + ".0";
            else if (remainder == 0)
                return result.ToString();

            System.Numerics.BigInteger decimals = System.Numerics.BigInteger.Abs((this.Numerator * System.Numerics.BigInteger.Pow(10, precision)) / this.Denominator);

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

            return (Sign < 0 ? "-" : "") + result + "." + new string(caa).TrimEnd(new char[] { '0' });
        }


        public override string ToString()
        {
            // default precision = 100
            return ToString(100);
        }

        public string ToRationalString()
        {
            return this.Numerator.ToString(System.Globalization.CultureInfo.InvariantCulture)
                   + " / " +
                   this.Denominator.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public MyRational IntegerPart
        {
            get { return this.Div(One); }
        }

        public MyRational FractionalPart
        {
            get { return this.Mod(One); }
        }


        public string ToMixString()
        {
            string s = this.IntegerPart.ToString();
            string f = this.FractionalPart.ToRationalString();
            return string.Concat(s, " ", f);
        }


        // http://datagenetics.com/blog/july12019/index.html
        // https://en.wikipedia.org/wiki/Bhaskara_I%27s_sine_approximation_formula#:~:text=In%20mathematics%2C%20Bhaskara%20I's%20sine,in%20his%20treatise%20titled%20Mahabhaskariya.
        //  maximum absolute error in using the formula is around 0.0016. 
        public MyRational Sin()
        {
            MyRational sixteen = new MyRational(16);
            MyRational four = new MyRational(4);

            MyRational nom = sixteen * this * (PI - this);
            MyRational denom = FivePiSquared - four * this * (PI - this);

            MyRational sine = nom.Divide(denom);
            return sine;
        }


        public MyRational Cos()
        {
            MyRational sixteen = new MyRational(16);
            MyRational four = new MyRational(4);

            MyRational nom = PiSquared - four * this.Pow(2);
            MyRational denom = PiSquared + this.Pow(2);

            MyRational cosine = nom.Divide(denom);
            return cosine;
        }


        public MyRational Tan()
        {
            return Sin() / Cos();
        }


        public MyRational Sec()
        {
            // https://www.britannica.com/science/cosecant
            return Cos().Inverse();
        }


        public MyRational Csc()
        {
            // https://www.britannica.com/science/cosecant
            return Sin().Inverse();
        }


        public MyRational Cot()
        {
            // https://www.britannica.com/science/cosecant
            return Cos() / Sin();
        }


        // reported maximum error 0.0015 radians (0.085944 degrees), lowest error in the paper.
        // https://dsp.stackexchange.com/questions/20444/books-resources-for-implementing-various-mathematical-functions-in-fixed-point-a/20482#20482
        public MyRational FastButBadArctan()
        {
            // http://nghiaho.com/?p=997
            // return  π/4 * x - x * (fabs(x) - 1) * (0.2447 + 0.0663 * fabs(x));
            return PiFourth * this - this * (this.Abs() - One) * (new MyRational(2447, 10000) + new MyRational(663, 10000) * this.Abs());
        }



        public MyRational Arctan(MathContext mc)
        {
            // https://en.wikipedia.org/wiki/Inverse_trigonometric_functions#Infinite_series
            MyRational sum = new MyRational(0);

            for (int n = 0; true; ++n)
            {
                MyRational dividend = MyRational.MinusOne.Pow(n) * this.Pow(2 * n + 1);
                MyRational divisor = new MyRational(2 * n + 1);

                MyRational quotient = dividend.Divide(divisor);

                MyRational newSum = sum + quotient;

                if (newSum.Subtract(sum).Abs() < mc.Epsilon)
                {
                    return newSum;
                }

                sum = newSum;
            } // Next i 

        }


        public MyRational Arctan()
        {
            return Arctan(new MathContext());
        }


        public MyRational Arcsin(MathContext mc)
        {
            // https://en.wikipedia.org/wiki/Inverse_trigonometric_functions#Extension_to_complex_plane
            // https://dsp.stackexchange.com/questions/25770/looking-for-an-arcsin-algorithm
            MyRational divisor = Sqrt(One.Subtract(this.Pow(2)), new MyRational(1, 10000));
            MyRational x = this.Divide(divisor);

            return x.Arctan(mc);
        }


        public MyRational Arcsin()
        {
            return Arcsin(new MathContext());
        }


        public MyRational Arcsec(MathContext mc)
        {
            // https://brownmath.com/twt/inverse.htm
            // Arcsec x = Arccos(1/x)
            return One.Divide(this).Arccos(mc);
        }

        public MyRational Arcsec()
        {
            return Arcsec(new MathContext());
        }


        public MyRational Arccos(MathContext mc)
        {
            // https://en.wikipedia.org/wiki/Inverse_trigonometric_functions#Relationships_among_the_inverse_trigonometric_functions
            return PiHalf - this.Arcsin(mc);
        }


        public MyRational Arccos()
        {
            return Arccos(new MathContext());
        }


        public MyRational Arccot(MathContext mc)
        {
            // https://en.wikipedia.org/wiki/Inverse_trigonometric_functions#Relationships_among_the_inverse_trigonometric_functions
            return PiHalf - this.Arctan(mc);
        }


        public MyRational Arccot()
        {
            return Arccot(new MathContext());
        }


        public MyRational Arccsc(MathContext mc)
        {
            // https://en.wikipedia.org/wiki/Inverse_trigonometric_functions#Relationships_among_the_inverse_trigonometric_functions
            // return PiHalf - this.Arcsec();

            // https://brownmath.com/twt/inverse.htm
            // Arccsc x = Arcsin(1/x)
            return One.Divide(this).Arcsin(mc);
        }


        public MyRational Arccsc()
        {
            return Arccsc(new MathContext());
        }


        // https://en.wikipedia.org/wiki/Inverse_trigonometric_functions
        public MyRational Arctan2(MyRational y, MyRational x, MathContext mc)
        {
            if (x.Numerator == System.Numerics.BigInteger.Zero)
            {
                if (y.Numerator == System.Numerics.BigInteger.Zero)
                    throw new System.ArithmeticException("arctan2(0,0) is undefined.");

                if (y.Sign == 1)
                    return PiHalf;
                else
                    return -PiHalf;
            }

            if (x.Sign == 1)
                return (y / x).Arctan(mc);

            if (y.Sign == -1)
                return (y / x).Arctan(mc).Subtract(PI);

            return (y / x).Arctan(mc).Add(PI);
        }

        public MyRational Arctan2(MyRational y, MyRational x)
        {
            return Arctan2(y, x, new MathContext());
        }


        public System.Numerics.BigInteger SlowPow(System.Numerics.BigInteger value, System.Numerics.BigInteger exponent)
        {
            if (exponent < 0)
                throw new System.ArgumentException("Exponent cannot be < 0 !");

            if (exponent == 0)
            {
                if (value != 0)
                    return 1;
                else
                    throw new System.ArithmeticException("0^0 is not defined");
            }

            System.Numerics.BigInteger originalValue = value;
            while (exponent-- > 1)
                value = System.Numerics.BigInteger.Multiply(value, originalValue);

            return value;
        }


        private static System.Numerics.BigInteger Pow(System.Numerics.BigInteger value, System.Numerics.BigInteger exponent)
        {
            if (exponent < 0)
                throw new System.ArgumentException("Exponent cannot be < 0 !");

            if (exponent == 0)
            {
                if (value != 0)
                    return 1;
                else
                    throw new System.ArithmeticException("0^0 is not defined");
            }

            System.Numerics.BigInteger total = 1;
            while (exponent > int.MaxValue)
            {
                exponent -= int.MaxValue;
                total = total * System.Numerics.BigInteger.Pow(value, int.MaxValue);
            }

            total = total * System.Numerics.BigInteger.Pow(value, (int)exponent);
            return total;
        }


        // x^(m/n) = root(x^m,n) = root(x,n)^m
        //   ==> (x/y)^(m/n) =  (x^(m/n))/(y^(m/n))
        //     ==> root(x^m,n) / root(y^m,n)
        public MyRational Pow(MyRational exponent, MathContext mc)
        {
            System.Numerics.BigInteger numerPow = Pow(this.Numerator, exponent.Numerator);
            MyRational num = MyRational.Root(new MyRational(numerPow), this.Denominator, mc.Epsilon);

            System.Numerics.BigInteger denomPow = Pow(this.Denominator, exponent.Numerator);
            MyRational denom = MyRational.Root(new MyRational(denomPow), this.Denominator, mc.Epsilon);

            return num.Divide(denom);
        }

        public MyRational Pow(MyRational exponent)
        {
            return Pow(exponent, new MathContext());
        }


        // https://pwg.gsfc.nasa.gov/stargaze/Slog4.htm
        // https://www.purplemath.com/modules/logrules.htm
        // Basic Log Rules & Expanding Log Expressions
        // 1) logb(mn) = logb(m) + logb(n)
        // 2) logb(m/n) = logb(m) – logb(n)
        // 3) logb(mn) = n · logb(m)

        // logn(x) = ln(x)/ln(n)

        public MyRational ln(MathContext mc)
        {
            // ln(x) can be expressed as (x^ h - 1)/ h
            // logn(x) = ln(x) / ln(n).
            MyRational log = (this.Pow(mc.Epsilon, mc) - One) / mc.Epsilon;

            return log;
        }

        public MyRational ln()
        {
            return ln(new MathContext(10));
        }







        public static double ln(double x, double h)
        {
            return (Math.Pow(x, h) - 1) / h;
        }


        public static double ln(double x)
        {
            double h = 0.000000001d; // where h approaches 0
            return ln(x, h);
        }


        // logn(x) = ln(x)/ln(n).
        public static double log(double x, double n, double epsilon)
        {
            return ln(x, epsilon) / ln(n, epsilon);
        }


        public static double log(double x, double n)
        {

            double h = 0.000000001d; // where h approaches 0
            return log(x, n, h);
        }


        // Using Newton's method, the iteration simplifies to (implementation) 
        // which has cubic convergence to ln(x).
        public static double lne(double x, double epsilon)
        {
            // https://en.wikipedia.org/wiki/Natural_logarithm#High_precision
            double yn = x;
            double yn1 = x;

            do
            {
                yn = yn1;
                yn1 = yn + 2 * (x - System.Math.Exp(yn)) / (x + System.Math.Exp(yn));
            } while (System.Math.Abs(yn - yn1) > epsilon);

            return yn1;
        }



        public static void LogTest()
        {
            double log2 = System.Math.Log(2);

            double h = 1d;

            for (int i = 0; i < 15; ++i)
            {
                h /= 10.0d;

                // double loga = ln(2, h);
                double loga = lne(2, h);
                double delta = System.Math.Abs(log2 - loga);
                System.Console.WriteLine("{0}: {1}", i, delta);
            }

        }


        // Lacks Log10, Log, Round, Ceiling, Floor, Truncate, SetScale, ToMixString
        // ShiftDecimalLeft ShiftDecimalRight
    }


}
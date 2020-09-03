
using System.Threading;

namespace OsmPolygon.RationalMath
{


    public class MyRational
    {

        public readonly System.Numerics.BigInteger Numerator;
        public readonly System.Numerics.BigInteger Denominator;


        public static void Test()
        {
            double gold = (1.0d + System.Math.Sqrt(5.0d)) / 2.0d;
            MyRational golden = new MyRational(gold);

            MyRational a = new MyRational(3);
            MyRational b = new MyRational(5);



            if ( a > b)
                System.Console.WriteLine("true");
            else
                System.Console.WriteLine("false");

            MyRational num = new MyRational(4, 2);
            MyRational sqrt = MyRational.Sqrt(num, new MyRational(1,10000));
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

        public static readonly MyRational PiHalf = PI.Divide(new MyRational(2));
        
        
        
        
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


        /*
        // Return a number that is positive, zero or negative, respectively, if
        // the value of this Rational is bigger than f ==> +1,
        // the values of this Rational and f are equal or ==> 0
        // the value of this Rational is smaller than f ==> -1
        public int CompareTo(MyRational other)
        {


            if (other == null)
                throw new System.ArgumentNullException("other");

            // Why ? Correct ? 

            //// Make copies
            //System.Numerics.BigInteger one = this.Numerator;
            //System.Numerics.BigInteger two = other.Numerator;

            //// cross multiply
            //one *= other.Denominator;
            //two *= this.Denominator;

            ////test
            //return System.Numerics.BigInteger.Compare(one, two);

            MyRational delta = this.Subtract(other);
            return delta.Sign;
        }




        public System.Numerics.BigInteger CompareTo(object other)
        {
            if (!(other is MyRational))
                throw new System.ArgumentException("other is not a MyRational");

            return CompareTo((MyRational)other);
        }
        */

        public bool Equals(MyRational other)
        {
            if (other == null)
                throw new System.ArgumentNullException("other");

            return this.Numerator == other.Numerator && this.Denominator == other.Denominator;
        }

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


        public static MyRational Sqrt(MyRational n, MyRational epsilon)
        {
            // Assuming the sqrt of n as n only 
            MyRational x = n;

            // The closed guess will be stored in the root 
            MyRational root;

            // To count the number of iterations 
            int count = 0;

            while (true)
            {
                count++;

                // Calculate more closed x 
                root = MyRational.OneHalf * (x + (n / x));

                // Check for closeness 
                if ((root - x).Abs() < epsilon)
                    break;

                // Update root 
                x = root;
            } // Whend 

            return root;
        } // End Function SquareRoot 


        // root(A, n)
        // https://en.wikipedia.org/wiki/Nth_root#Using_Newton.27s_method
        // https://www.geeksforgeeks.org/find-root-of-a-number-using-newtons-method/
        public static MyRational Root(MyRational n, int exponent, MyRational epsilon)
        {
            if (exponent == 2)
                return Sqrt(n, epsilon); // Should be lightly faster
            
            // Assuming the sqrt of n as n only 
            MyRational x = n;
            
            // The closed guess will be stored in the root 
            MyRational root;
            
            int exp_minus_one = exponent - 1;
            
            MyRational one_over_exponent = new MyRational(1, exponent);
            MyRational exponent_minus_one = new MyRational(exp_minus_one);

            // To count the number of iterations 
            int count = 0;
            while (true)
            {
                count++;
                
                // Calculate more closed x 
                root = one_over_exponent * (exponent_minus_one * x + (n / x.Pow(exp_minus_one)));
                // x_{k+1}={\frac {1}{n}}\left({(n-1)x_{k}+{\frac {A}{x_{k}^{n-1}}}}\right)
                
                // Check for closeness 
                if ((root - x).Abs() < epsilon)
                    break;
                
                // Update root 
                x = root;
            } // Whend 

            return root;
        } // End Function RootN 


        // r^n = x ==> r = root(x, n) = root(this, exponent)
        public MyRational Root(int exponent)
        {
            System.Numerics.BigInteger.Pow(this.Numerator, -exponent);
            System.Numerics.BigInteger.Pow(this.Denominator, -exponent);

            MyRational xk = null;

            MyRational oneOverN = new MyRational(1, exponent);




            return new MyRational(0, 0);
        }



        // this + other 
        public MyRational Add(MyRational other)
        {
            return new MyRational(this.Numerator * other.Denominator + this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }

        // this - other
        public MyRational Subtract(MyRational other)
        {
            return new MyRational(this.Numerator * other.Denominator - this.Denominator * other.Numerator, this.Denominator * other.Denominator);
        }

        // this * other
        public MyRational Multiply(MyRational other)
        {
            return new MyRational(this.Numerator * other.Numerator, this.Denominator * other.Denominator);
        }

        // this/other
        public MyRational Divide(MyRational other)
        {
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

            else
            {
                char[] ca = sb.ToString().ToCharArray();
                System.Array.Reverse(ca);

                return (Sign < 0 ? "-" : "") + result + "." + new string(ca).TrimEnd(new char[] { '0' });
            }
                
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

        
        // https://www.britannica.com/science/cosecant
        public MyRational Sec()
        {
            return Cos().Inverse();
        }
        
        
        // https://www.britannica.com/science/cosecant
        public MyRational Csc()
        {
            return Sin().Inverse();
        }
        
        
        // https://www.britannica.com/science/cosecant
        public MyRational Cot()
        {
            return Cos() / Sin();
        }
        
        public MyRational Arctan()
        {
            throw new System.NotImplementedException("arctan");
            return null;
        }
        
        
        // https://en.wikipedia.org/wiki/Inverse_trigonometric_functions
        public MyRational Arctan2(MyRational y, MyRational x)
        {
            if (x.Numerator == System.Numerics.BigInteger.Zero)
            {
                if(y.Numerator == System.Numerics.BigInteger.Zero)
                    throw new System.ArithmeticException("arctan2(0,0) is undefined.");

                if (y.Sign == 1)
                    return PiHalf;
                else
                    return -PiHalf;
            }
            
            if (x.Sign == 1)
                return (y / x).Arctan();

            if (y.Sign == -1)
                return (y / x).Arctan().Subtract(PI);
            
            return (y / x).Arctan().Add(PI);
        }
        
        
        // Lacks Log10, Log, Round, Ceiling, Floor, Truncate, SetScale, ToMixString
        // ShiftDecimalLeft ShiftDecimalRight
    }
    
    
}

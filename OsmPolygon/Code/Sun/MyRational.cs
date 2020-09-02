using System;
using System.Collections.Generic;
using System.Text;

namespace OsmPolygon.Code.Sun
{
    class MyRational
    {

        public System.Numerics.BigInteger Numerator;
        public System.Numerics.BigInteger Denominator; // denom > 0, fraction is reduced to lowest terms

        // Create a fraction given numerator and denominator.
        public MyRational(System.Numerics.BigInteger numerator, System.Numerics.BigInteger denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
            Normalize();
        }


        public MyRational(System.Numerics.BigInteger numerator)
            :this(numerator,1)
        { }


        // Use Euclid's algorithm to calculate the
        // greatest common divisor (GCD) of two numbers.
        public static System.Numerics.BigInteger GCD(System.Numerics.BigInteger a, System.Numerics.BigInteger b)
        {
            a = System.Numerics.BigInteger.Abs(a);
            b = System.Numerics.BigInteger.Abs(b);

            System.Numerics.BigInteger remainder = 0;
            while ((remainder = a % b) == 0)
            {
                a = b;
                b = remainder;
            }

            return b;
        }


        // Return the least common multiple
        // (LCM) of two numbers.
        public static System.Numerics.BigInteger LCM(System.Numerics.BigInteger a, System.Numerics.BigInteger b)
        {
            return a * b / GCD(a, b);
        }

        private MyRational Normalize()
        {
            if (this.Denominator == 0)
            { 
                throw new System.DivideByZeroException("In function " + nameof(Normalize));
            }

            System.Numerics.BigInteger n = GCD(this.Numerator, this.Denominator);
            this.Numerator /= n;         // lowest
            this.Denominator /= n;       //    terms
            if (this.Denominator < 0)
            { 
                // make denom positive
                this.Denominator = -this.Denominator;
                this.Numerator = -this.Numerator;
            }

            return this;
        }

        /**
         * Return a number that is positive, zero or negative, respectively, if
         *   the value of this Rational is bigger than f,
         *   the values of this Rational and f are equal or
         *   the value of this Rational is smaller than f.
         */
        public System.Numerics.BigInteger CompareTo(MyRational other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            return this.Numerator * other.Denominator - this.Denominator * other.Numerator; //numerator of this - f
        }

        public System.Numerics.BigInteger CompareTo(object other)
        {
            if (!(other is MyRational))
                throw new System.ArgumentException("other is not a MyRational");

            return CompareTo((MyRational)other);
        }

        public bool Equals(MyRational other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

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


        //// ToDecimal chunk
        ///** Return a decimal approximation to the fraction. */
        //public decimal ToDecimal()
        //{
        //    return ((decimal)this.Numerator) / this.Denominator;
        //}


        public System.Numerics.BigInteger Sign
        {
            get
            {
                this.Normalize();
                if (this.Numerator == 0)
                    return 0;

                if(this.Denominator == 0)
                    throw new System.DivideByZeroException("In function " + nameof(Sign));

                if (this.Numerator > 0)
                    return 1;

                return -1;
            }
        }


        public static MyRational Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            value.Trim();
            value = value.Replace(",", "");
            int pos = value.IndexOf('.');
            value = value.Replace(".", "");

            if (pos < 0)
            {
                //no decimal poSystem.Numerics.BigInteger
                System.Numerics.BigInteger numerator = System.Numerics.BigInteger.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                return new MyRational(numerator);
            }
            else
            {
                //decimal poSystem.Numerics.BigInteger (length - pos - 1)
                System.Numerics.BigInteger numerator = System.Numerics.BigInteger.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                System.Numerics.BigInteger denominator = System.Numerics.BigInteger.Pow(10, value.Length - pos);

                return new MyRational(numerator, denominator);
            }
        }


        // Negate chunk
        /** Return a new Rational which is this Rational negated.*/
        public MyRational Negate()
        {
            this.Numerator *= -1;
            this.Normalize();
            return this;

            // return new MyRational(-this.Numerator, this.Denominator);
        }

        // Reciprocal chunk
        /** Return a new Rational which is the reciprocal of this Rational.*/
        //public MyRational Reciprocal()
        //{
        //    return new MyRational(this.Denominator, this.Numerator);
        //}

        public MyRational Inverse()
        {
            System.Numerics.BigInteger temp = this.Numerator;
            this.Numerator = this.Denominator;
            this.Denominator = temp;

            return this;
        }


        public MyRational Increment()
        {
            this.Numerator += this.Denominator;
            return this;
        }
        public MyRational Decrement()
        {
            this.Numerator -= this.Denominator;
            return this;
        }

        public MyRational Abs()
        {
            if (this.Numerator < 0)
                this.Numerator *= -1;

            if (this.Denominator < 0)
                this.Denominator *= -1;



            return this;
        }



        // Add chunk
        /** Return a new Rational which is the sum of this Rational and f. */
        public MyRational Add(MyRational f)
        {
            return new MyRational(this.Numerator * f.Denominator + this.Denominator * f.Numerator, this.Denominator * f.Denominator);
        }

        /** Return a new Rational which is the difference of this Rational and f.*/
        public MyRational Subtract(MyRational f)
        {
            return new MyRational(this.Numerator * f.Denominator - this.Denominator * f.Numerator, this.Denominator * f.Denominator);
        }

        // Multiply chunk
        /** Return a new Rational which is the product of this Rational and f. */
        public MyRational Multiply(MyRational f)
        {                           // end Multiply heading chunk
            return new MyRational(this.Numerator * f.Numerator, this.Denominator * f.Denominator);
        }
        // Divide chunk
        /** Return a new Rational which is the quotient of this Rational and f. */
        public MyRational Divide(MyRational f)
        {
            return new MyRational(this.Numerator * f.Denominator, this.Denominator * f.Numerator);
        }



        //public System.Numerics.BigInteger Remainder
        //{
        //    get
        //    {
        //        return this.Numerator % this.Denominator;
        //    }
        //}

        //public MyRational Decimals()
        //{
        //    return new MyRational(this.Remainder, this.Denominator);
        //}


        // 5/3 *2/7 = 35/6 = 5 r 5
        public MyRational Div(MyRational other)
        {
            System.Numerics.BigInteger num = this.Numerator * other.Denominator;
            System.Numerics.BigInteger den = this.Denominator * other.Numerator;

            return new MyRational(num / den);
        }




        // The 'modulus' operation is defined as:
        // a % n ==> a - (a / n) * n
        // 5/3 *2/7 = 35/6 = 5 r 5
        // this % other ==> this - (this / other) * other
        public MyRational Remainder(MyRational other)
        {
            // this / other

            var a_n_o = this.Divide(other).Multiply(other);
            
            

            var mr = this.Subtract();


            return null;

        }


    }


}

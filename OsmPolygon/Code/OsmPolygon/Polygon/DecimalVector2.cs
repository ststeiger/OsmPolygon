/*
Copyright (c) 2006 - 2008 The Open Toolkit library.

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */


using System.Diagnostics.Contracts;


namespace OpenToolkit.Mathematics
{


    /// <summary>
    /// Represents a 2D vector using two single-precision decimaling-point numbers.
    /// </summary>
    /// <remarks>
    /// The DecimalVector2 structure is suitable for interoperation with unmanaged code requiring two consecutive decimals.
    /// </remarks>
    [System.Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct DecimalVector2 
        : System.IEquatable<DecimalVector2>
    {
        /// <summary>
        /// The X component of the DecimalVector2.
        /// </summary>
        public decimal X;

        /// <summary>
        /// The Y component of the DecimalVector2.
        /// </summary>
        public decimal Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalVector2"/> struct.
        /// </summary>
        /// <param name="value">The value that will initialize this instance.</param>
        public DecimalVector2(decimal value)
        {
            X = value;
            Y = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalVector2"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate of the net DecimalVector2.</param>
        /// <param name="y">The y coordinate of the net DecimalVector2.</param>
        public DecimalVector2(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }


        // http://mathworld.wolfram.com/Point-LineDistance3-Dimensional.html
        // // Yay, 2D crossp = determinant = scalar result | DistanceOfPointToLine(p, x1, x2)
        public static decimal DistanceOfPointToLine(DecimalVector2 point, DecimalVector2 lineStartPoint, DecimalVector2 lineEndPoint)
        {
            DecimalVector2 x2minusx1 = Subtract(lineEndPoint, lineStartPoint); // vec2-vec1
            DecimalVector2 x1minusx0 = Subtract(lineStartPoint, point); // vec2-vec1

            decimal DeltaNorm = PerpDot(x2minusx1, x1minusx0); 

            decimal x2minusx1Norm = x2minusx1.Length;
            decimal nReturnValue = DeltaNorm / x2minusx1Norm;
            return nReturnValue;
        } // End Function DistanceOfPointToLine 


        public static bool IsPointOnLine(DecimalVector2 linePoint1, DecimalVector2 linePoint2, DecimalVector2 pointInQuestion)
        {
            DecimalVector2 pointA = new DecimalVector2();
            DecimalVector2 pointB = new DecimalVector2();

            if (linePoint1.X < linePoint2.X)
            {
                pointA.X = linePoint1.X;
                pointB.X = linePoint2.X;
            }
            else
            {
                pointA.X = linePoint2.X;
                pointB.X = linePoint1.X;
            }

            if (linePoint1.Y < linePoint2.Y)
            {
                pointA.Y = linePoint1.Y;
                pointB.Y = linePoint2.Y;
            }
            else
            {
                pointA.Y = linePoint2.Y;
                pointB.Y = linePoint1.Y;
            }


            if (pointInQuestion.X >= pointA.X && pointInQuestion.Y >= pointA.Y && pointInQuestion.X <= pointB.X && pointInQuestion.Y <= pointB.Y)
            {
                return true;
            }

            return false;
        } // End Function IsPointOnLine 


        // Q + v => intersection with pointToAdd-vertical to v
        // cVector_2d.GetPointVerticalIntersection(aptDefinitionPoints[i], vec2_VecLine, cptPointToAdd);
        public static DecimalVector2? GetPointVerticalIntersection(DecimalVector2 pointQ, DecimalVector2 vecInputLine, DecimalVector2 pointP)
        {
            // Q: Line start/endpoint
            // P + a * vecNormalVector = Intersection = Q + b * vecInputLine
            // a = (-(Px*vy-Py*vx-Qx*vy+Qy*vx))/(nx*vy-ny*vx)
            // b = (-(Px*ny-Py*nx-Qx*ny+Qy*nx))/(nx*vy-ny*vx)

            DecimalVector2 vecNormalVector = vecInputLine.PerpendicularLeft;
            decimal denominator = vecNormalVector.X * vecInputLine.Y - vecNormalVector.Y * vecInputLine.X;

            DecimalVector2 intersectionPoint = new DecimalVector2();
            if (denominator == 0.0m)
            {
                // no intersection
                return null;
            }

            decimal a = (-(pointP.X * vecInputLine.Y - pointP.Y * vecInputLine.X - pointQ.X * vecInputLine.Y + pointQ.Y * vecInputLine.X)) / denominator;
            decimal b = (-(pointP.X * vecNormalVector.Y - pointP.Y * vecNormalVector.X - pointQ.X * vecNormalVector.Y + pointQ.Y * vecNormalVector.X)) / denominator;

            intersectionPoint.X = pointP.X + a * vecNormalVector.X;
            intersectionPoint.Y = pointP.Y + a * vecNormalVector.Y;

            return intersectionPoint;
        } // End Function GetPointVerticalIntersection 


        /// <summary>
        /// Gets or sets the value at the index of the Vector.
        /// </summary>
        /// <param name="index">The index of the component from the Vector.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is less than 0 or greater than 1.</exception>
        public decimal this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return X;
                }

                if (index == 1)
                {
                    return Y;
                }

                throw new System.IndexOutOfRangeException("You tried to access this vector at index: " + index);
            }

            set
            {
                if (index == 0)
                {
                    X = value;
                }
                else if (index == 1)
                {
                    Y = value;
                }
                else
                {
                    throw new System.IndexOutOfRangeException("You tried to set this vector at index: " + index);
                }
            }
        }

        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <see cref="LengthFast"/>
        /// <seealso cref="LengthSquared"/>
        public decimal Length
        {
            get
            {
                return MathHelper.Sqrt((X * X) + (Y * Y));
            }
        }

        /// <summary>
        /// Gets an approximation of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property uses an approximation of the square root function to calculate vector magnitude, with
        /// an upper error bound of 0.001.
        /// </remarks>
        /// <see cref="Length"/>
        /// <seealso cref="LengthSquared"/>
        public decimal LengthFast
        {
            get
            {
                return 1.0m / MathHelper.InverseSqrtFast((X * X) + (Y * Y));
            }
        }




        /// <summary>
        /// Gets the square of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property avoids the costly square root operation required by the Length property. This makes it more suitable
        /// for comparisons.
        /// </remarks>
        /// <see cref="Length"/>
        /// <seealso cref="LengthFast"/>
        public decimal LengthSquared => (X * X) + (Y * Y);

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        public DecimalVector2 PerpendicularRight => new DecimalVector2(Y, -X); // Normal's vector 2

        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        public DecimalVector2 PerpendicularLeft => new DecimalVector2(-Y, X); // normal's vector 1

        /// <summary>
        /// Returns a copy of the DecimalVector2 scaled to unit length.
        /// </summary>
        /// <returns>The normalized copy.</returns>
        public DecimalVector2 Normalized()
        {
            var v = this;
            v.Normalize();
            return v;
        }

        /// <summary>
        /// Scales the DecimalVector2 to unit length.
        /// </summary>
        public void Normalize()
        {
            var scale = 1.0m / Length;
            X *= scale;
            Y *= scale;
        }

        /// <summary>
        /// Scales the DecimalVector2 to approximately unit length.
        /// </summary>
        public void NormalizeFast()
        {
            var scale = MathHelper.InverseSqrtFast((X * X) + (Y * Y));
            X *= scale;
            Y *= scale;
        }

        /// <summary>
        /// Defines a unit-length DecimalVector2 that points towards the X-axis.
        /// </summary>
        public static readonly DecimalVector2 UnitX = new DecimalVector2(1, 0);

        /// <summary>
        /// Defines a unit-length DecimalVector2 that points towards the Y-axis.
        /// </summary>
        public static readonly DecimalVector2 UnitY = new DecimalVector2(0, 1);

        /// <summary>
        /// Defines a zero-length DecimalVector2.
        /// </summary>
        public static readonly DecimalVector2 Zero = new DecimalVector2(0, 0);

        /// <summary>
        /// Defines an instance with all components set to 1.
        /// </summary>
        public static readonly DecimalVector2 One = new DecimalVector2(1, 1);

        /// <summary>
        /// Defines the size of the DecimalVector2 struct in bytes.
        /// </summary>
        public static readonly int SizeInBytes = System.Runtime.InteropServices.Marshal.SizeOf<DecimalVector2>();

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <returns>Result of operation.</returns>
        [Pure]
        public static DecimalVector2 Add(DecimalVector2 a, DecimalVector2 b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">Left operand.</param>
        /// <param name="b">Right operand.</param>
        /// <param name="result">Result of operation.</param>
        public static void Add(ref DecimalVector2 a, ref DecimalVector2 b, out DecimalVector2 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
        }

        /// <summary>
        /// Subtract one Vector from another.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>Result of subtraction.</returns>
        [Pure]
        public static DecimalVector2 Subtract(DecimalVector2 a, DecimalVector2 b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }

        /// <summary>
        /// Subtract one Vector from another.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Result of subtraction.</param>
        public static void Subtract(ref DecimalVector2 a, ref DecimalVector2 b, out DecimalVector2 result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static DecimalVector2 Multiply(DecimalVector2 vector, decimal scale)
        {
            Multiply(ref vector, scale, out vector);
            return vector;
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(ref DecimalVector2 vector, decimal scale, out DecimalVector2 result)
        {
            result.X = vector.X * scale;
            result.Y = vector.Y * scale;
        }

        /// <summary>
        /// Multiplies a vector by the components a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static DecimalVector2 Multiply(DecimalVector2 vector, DecimalVector2 scale)
        {
            Multiply(ref vector, ref scale, out vector);
            return vector;
        }

        /// <summary>
        /// Multiplies a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(ref DecimalVector2 vector, ref DecimalVector2 scale, out DecimalVector2 result)
        {
            result.X = vector.X * scale.X;
            result.Y = vector.Y * scale.Y;
        }

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static DecimalVector2 Divide(DecimalVector2 vector, decimal scale)
        {
            Divide(ref vector, scale, out vector);
            return vector;
        }

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(ref DecimalVector2 vector, decimal scale, out DecimalVector2 result)
        {
            result.X = vector.X / scale;
            result.Y = vector.Y / scale;
        }

        /// <summary>
        /// Divides a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static DecimalVector2 Divide(DecimalVector2 vector, DecimalVector2 scale)
        {
            Divide(ref vector, ref scale, out vector);
            return vector;
        }

        /// <summary>
        /// Divide a vector by the components of a vector (scale).
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Divide(ref DecimalVector2 vector, ref DecimalVector2 scale, out DecimalVector2 result)
        {
            result.X = vector.X / scale.X;
            result.Y = vector.Y / scale.Y;
        }

        /// <summary>
        /// Returns a vector created from the smallest of the corresponding components of the given vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>The component-wise minimum.</returns>
        [Pure]
        public static DecimalVector2 ComponentMin(DecimalVector2 a, DecimalVector2 b)
        {
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            return a;
        }

        /// <summary>
        /// Returns a vector created from the smallest of the corresponding components of the given vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">The component-wise minimum.</param>
        public static void ComponentMin(ref DecimalVector2 a, ref DecimalVector2 b, out DecimalVector2 result)
        {
            result.X = a.X < b.X ? a.X : b.X;
            result.Y = a.Y < b.Y ? a.Y : b.Y;
        }

        /// <summary>
        /// Returns a vector created from the largest of the corresponding components of the given vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>The component-wise maximum.</returns>
        [Pure]
        public static DecimalVector2 ComponentMax(DecimalVector2 a, DecimalVector2 b)
        {
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            return a;
        }

        /// <summary>
        /// Returns a vector created from the largest of the corresponding components of the given vectors.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">The component-wise maximum.</param>
        public static void ComponentMax(ref DecimalVector2 a, ref DecimalVector2 b, out DecimalVector2 result)
        {
            result.X = a.X > b.X ? a.X : b.X;
            result.Y = a.Y > b.Y ? a.Y : b.Y;
        }

        /// <summary>
        /// Returns the DecimalVector2 with the minimum magnitude. If the magnitudes are equal, the second vector
        /// is selected.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>The minimum DecimalVector2.</returns>
        [Pure]
        public static DecimalVector2 MagnitudeMin(DecimalVector2 left, DecimalVector2 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        /// <summary>
        /// Returns the DecimalVector2 with the minimum magnitude. If the magnitudes are equal, the second vector
        /// is selected.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <param name="result">The magnitude-wise minimum.</param>
        public static void MagnitudeMin(ref DecimalVector2 left, ref DecimalVector2 right, out DecimalVector2 result)
        {
            result = left.LengthSquared < right.LengthSquared ? left : right;
        }

        /// <summary>
        /// Returns the DecimalVector2 with the maximum magnitude. If the magnitudes are equal, the first vector
        /// is selected.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>The maximum DecimalVector2.</returns>
        [Pure]
        public static DecimalVector2 MagnitudeMax(DecimalVector2 left, DecimalVector2 right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }

        /// <summary>
        /// Returns the DecimalVector2 with the maximum magnitude. If the magnitudes are equal, the first vector
        /// is selected.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <param name="result">The magnitude-wise maximum.</param>
        public static void MagnitudeMax(ref DecimalVector2 left, ref DecimalVector2 right, out DecimalVector2 result)
        {
            result = left.LengthSquared >= right.LengthSquared ? left : right;
        }

        /// <summary>
        /// Clamp a vector to the given minimum and maximum vectors.
        /// </summary>
        /// <param name="vec">Input vector.</param>
        /// <param name="min">Minimum vector.</param>
        /// <param name="max">Maximum vector.</param>
        /// <returns>The clamped vector.</returns>
        [Pure]
        public static DecimalVector2 Clamp(DecimalVector2 vec, DecimalVector2 min, DecimalVector2 max)
        {
            vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
            return vec;
        }

        /// <summary>
        /// Clamp a vector to the given minimum and maximum vectors.
        /// </summary>
        /// <param name="vec">Input vector.</param>
        /// <param name="min">Minimum vector.</param>
        /// <param name="max">Maximum vector.</param>
        /// <param name="result">The clamped vector.</param>
        public static void Clamp(ref DecimalVector2 vec, ref DecimalVector2 min, ref DecimalVector2 max, out DecimalVector2 result)
        {
            result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
        }

        /// <summary>
        /// Compute the euclidean distance between two vectors.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>The distance.</returns>
        [Pure]
        public static decimal Distance(DecimalVector2 vec1, DecimalVector2 vec2)
        {
            Distance(ref vec1, ref vec2, out decimal result);
            return result;
        }

        /// <summary>
        /// Compute the euclidean distance between two vectors.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <param name="result">The distance.</param>
        public static void Distance(ref DecimalVector2 vec1, ref DecimalVector2 vec2, out decimal result)
        {
            result = MathHelper.Sqrt(((vec2.X - vec1.X) * (vec2.X - vec1.X)) + ((vec2.Y - vec1.Y) * (vec2.Y - vec1.Y)));
        }

        /// <summary>
        /// Compute the squared euclidean distance between two vectors.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>The squared distance.</returns>
        [Pure]
        public static decimal DistanceSquared(DecimalVector2 vec1, DecimalVector2 vec2)
        {
            DistanceSquared(ref vec1, ref vec2, out decimal result);
            return result;
        }

        /// <summary>
        /// Compute the squared euclidean distance between two vectors.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <param name="result">The squared distance.</param>
        public static void DistanceSquared(ref DecimalVector2 vec1, ref DecimalVector2 vec2, out decimal result)
        {
            result = ((vec2.X - vec1.X) * (vec2.X - vec1.X)) + ((vec2.Y - vec1.Y) * (vec2.Y - vec1.Y));
        }

        /// <summary>
        /// Scale a vector to unit length.
        /// </summary>
        /// <param name="vec">The input vector.</param>
        /// <returns>The normalized copy.</returns>
        [Pure]
        public static DecimalVector2 Normalize(DecimalVector2 vec)
        {
            var scale = 1.0m / vec.Length;
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a vector to unit length.
        /// </summary>
        /// <param name="vec">The input vector.</param>
        /// <param name="result">The normalized vector.</param>
        public static void Normalize(ref DecimalVector2 vec, out DecimalVector2 result)
        {
            var scale = 1.0m / vec.Length;
            result.X = vec.X * scale;
            result.Y = vec.Y * scale;
        }

        /// <summary>
        /// Scale a vector to approximately unit length.
        /// </summary>
        /// <param name="vec">The input vector.</param>
        /// <returns>The normalized copy.</returns>
        [Pure]
        public static DecimalVector2 NormalizeFast(DecimalVector2 vec)
        {
            var scale = MathHelper.InverseSqrtFast((vec.X * vec.X) + (vec.Y * vec.Y));
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }

        /// <summary>
        /// Scale a vector to approximately unit length.
        /// </summary>
        /// <param name="vec">The input vector.</param>
        /// <param name="result">The normalized vector.</param>
        public static void NormalizeFast(ref DecimalVector2 vec, out DecimalVector2 result)
        {
            var scale = MathHelper.InverseSqrtFast((vec.X * vec.X) + (vec.Y * vec.Y));
            result.X = vec.X * scale;
            result.Y = vec.Y * scale;
        }

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <returns>The dot product of the two inputs.</returns>
        [Pure]
        public static decimal Dot(DecimalVector2 left, DecimalVector2 right)
        {
            return (left.X * right.X) + (left.Y * right.Y);
        }

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <param name="result">The dot product of the two inputs.</param>
        public static void Dot(ref DecimalVector2 left, ref DecimalVector2 right, out decimal result)
        {
            result = (left.X * right.X) + (left.Y * right.Y);
        }

        /// <summary>
        /// Calculate the perpendicular dot (scalar) product of two vectors.
        /// aka. 2-dimensional cross-product 
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <returns>The perpendicular dot product of the two inputs.</returns>
        [Pure]
        public static decimal PerpDot(DecimalVector2 left, DecimalVector2 right)
        {
            return (left.X * right.Y) - (left.Y * right.X);
        }

        /// <summary>
        /// Calculate the perpendicular dot (scalar) product of two vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <param name="result">The perpendicular dot product of the two inputs.</param>
        public static void PerpDot(ref DecimalVector2 left, ref DecimalVector2 right, out decimal result)
        {
            result = (left.X * right.Y) - (left.Y * right.X);
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// </summary>
        /// <param name="a">First input vector.</param>
        /// <param name="b">Second input vector.</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>a when blend=0, b when blend=1, and a linear combination otherwise.</returns>
        [Pure]
        public static DecimalVector2 Lerp(DecimalVector2 a, DecimalVector2 b, decimal blend)
        {
            a.X = (blend * (b.X - a.X)) + a.X;
            a.Y = (blend * (b.Y - a.Y)) + a.Y;
            return a;
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors.
        /// </summary>
        /// <param name="a">First input vector.</param>
        /// <param name="b">Second input vector.</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise.</param>
        public static void Lerp(ref DecimalVector2 a, ref DecimalVector2 b, decimal blend, out DecimalVector2 result)
        {
            result.X = (blend * (b.X - a.X)) + a.X;
            result.Y = (blend * (b.Y - a.Y)) + a.Y;
        }

        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates.
        /// </summary>
        /// <param name="a">First input Vector.</param>
        /// <param name="b">Second input Vector.</param>
        /// <param name="c">Third input Vector.</param>
        /// <param name="u">First Barycentric Coordinate.</param>
        /// <param name="v">Second Barycentric Coordinate.</param>
        /// <returns>a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise.</returns>
        [Pure]
        public static DecimalVector2 BaryCentric(DecimalVector2 a, DecimalVector2 b, DecimalVector2 c, decimal u, decimal v)
        {
            return a + (u * (b - a)) + (v * (c - a));
        }

        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates.
        /// </summary>
        /// <param name="a">First input Vector.</param>
        /// <param name="b">Second input Vector.</param>
        /// <param name="c">Third input Vector.</param>
        /// <param name="u">First Barycentric Coordinate.</param>
        /// <param name="v">Second Barycentric Coordinate.</param>
        /// <param name="result">
        /// Output Vector. a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c
        /// otherwise.
        /// </param>
        public static void BaryCentric
        (
            ref DecimalVector2 a,
            ref DecimalVector2 b,
            ref DecimalVector2 c,
            decimal u,
            decimal v,
            out DecimalVector2 result
        )
        {
            result = a; // copy

            var temp = b; // copy
            Subtract(ref temp, ref a, out temp);
            Multiply(ref temp, u, out temp);
            Add(ref result, ref temp, out result);

            temp = c; // copy
            Subtract(ref temp, ref a, out temp);
            Multiply(ref temp, v, out temp);
            Add(ref result, ref temp, out result);
        }

        /// <summary>
        /// Gets or sets an OpenToolkit.DecimalVector2 with the Y and X components of this instance.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public DecimalVector2 Yx
        {
            get => new DecimalVector2(Y, X);
            set
            {
                Y = value.X;
                X = value.Y;
            }
        }

        /// <summary>
        /// Adds the specified instances.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Result of addition.</returns>
        [Pure]
        public static DecimalVector2 operator +(DecimalVector2 left, DecimalVector2 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        /// <summary>
        /// Subtracts the specified instances.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>Result of subtraction.</returns>
        [Pure]
        public static DecimalVector2 operator -(DecimalVector2 left, DecimalVector2 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }

        /// <summary>
        /// Negates the specified instance.
        /// </summary>
        /// <param name="vec">Operand.</param>
        /// <returns>Result of negation.</returns>
        [Pure]
        public static DecimalVector2 operator -(DecimalVector2 vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            return vec;
        }

        /// <summary>
        /// Multiplies the specified instance by a scalar.
        /// </summary>
        /// <param name="vec">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of multiplication.</returns>
        [Pure]
        public static DecimalVector2 operator *(DecimalVector2 vec, decimal scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }

        /// <summary>
        /// Multiplies the specified instance by a scalar.
        /// </summary>
        /// <param name="scale">Left operand.</param>
        /// <param name="vec">Right operand.</param>
        /// <returns>Result of multiplication.</returns>
        [Pure]
        public static DecimalVector2 operator *(decimal scale, DecimalVector2 vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }

        /// <summary>
        /// Component-wise multiplication between the specified instance by a scale vector.
        /// </summary>
        /// <param name="scale">Left operand.</param>
        /// <param name="vec">Right operand.</param>
        /// <returns>Result of multiplication.</returns>
        [Pure]
        public static DecimalVector2 operator *(DecimalVector2 vec, DecimalVector2 scale)
        {
            vec.X *= scale.X;
            vec.Y *= scale.Y;
            return vec;
        }

        /// <summary>
        /// Divides the specified instance by a scalar.
        /// </summary>
        /// <param name="vec">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the division.</returns>
        [Pure]
        public static DecimalVector2 operator /(DecimalVector2 vec, decimal scale)
        {
            vec.X /= scale;
            vec.Y /= scale;
            return vec;
        }

        /// <summary>
        /// Compares the specified instances for equality.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        [Pure]
        public static bool operator ==(DecimalVector2 left, DecimalVector2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares the specified instances for inequality.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>True if both instances are not equal; false otherwise.</returns>
        [Pure]
        public static bool operator !=(DecimalVector2 left, DecimalVector2 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalVector2"/> struct using a tuple containing the component
        /// values.
        /// </summary>
        /// <param name="values">A tuple containing the component values.</param>
        /// <returns>A new instance of the <see cref="DecimalVector2"/> struct with the given component values.</returns>
        [Pure]
        public static implicit operator DecimalVector2((decimal X, decimal Y) values)
        {
            return new DecimalVector2(values.X, values.Y);
        }

        private static readonly string ListSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("({0}{2} {1})", X, Y, ListSeparator);
        }

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        [Pure]
        public override bool Equals(object obj)
        {
            if (!(obj is DecimalVector2))
            {
                return false;
            }

            return Equals((DecimalVector2)obj);
        }

        /// <summary>
        /// Indicates whether the current vector is equal to another vector.
        /// </summary>
        /// <param name="other">A vector to compare with this vector.</param>
        /// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
        [Pure]
        public bool Equals(DecimalVector2 other)
        {
            return
                X == other.X &&
                Y == other.Y;
        }

        /// <summary>
        /// Deconstructs the vector into it's individual components.
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        [Pure]
        public void Deconstruct(out decimal x, out decimal y)
        {
            x = X;
            y = Y;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace OsmPolygon.Concave.COORDS
{

    public class Vector3D
    {
        private double x;
        private double y;
        private double z;


        public Vector3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public Vector3D(double[] v)
            // throws DimensionMismatchException
        {
            if (v.Length != 3)
            {
                throw new System.ArgumentException("DimensionMismatchException (v.length, 3");
            }
            else
            {
                this.x = v[0];
                this.y = v[1];
                this.z = v[2];
            }
        }


        public Vector3D(double alpha, double delta)
        {
            double cosDelta = System.Math.Cos(delta);
            this.x = System.Math.Cos(alpha) * cosDelta;
            this.y = System.Math.Sin(alpha) * cosDelta;
            this.z = System.Math.Sin(delta);
        }


        public Vector3D(double a, Vector3D u)
        {
            this.x = a * u.x;
            this.y = a * u.y;
            this.z = a * u.z;
        }


        public Vector3D(double a1, Vector3D u1, double a2, Vector3D u2)
        {
            this.x = MathArrays.linearCombination(a1, u1.x, a2, u2.x);
            this.y = MathArrays.linearCombination(a1, u1.y, a2, u2.y);
            this.z = MathArrays.linearCombination(a1, u1.z, a2, u2.z);
        }

        public Vector3D(double a1, Vector3D u1, double a2, Vector3D u2, double a3, Vector3D u3)
        {
            this.x = MathArrays.linearCombination(a1, u1.x, a2, u2.x, a3, u3.x);
            this.y = MathArrays.linearCombination(a1, u1.y, a2, u2.y, a3, u3.y);
            this.z = MathArrays.linearCombination(a1, u1.z, a2, u2.z, a3, u3.z);
        }

        public Vector3D(double a1, Vector3D u1, double a2, Vector3D u2, double a3, Vector3D u3, double a4, Vector3D u4)
        {
            this.x = MathArrays.linearCombination(a1, u1.x, a2, u2.x, a3, u3.x, a4, u4.x);
            this.y = MathArrays.linearCombination(a1, u1.y, a2, u2.y, a3, u3.y, a4, u4.y);
            this.z = MathArrays.linearCombination(a1, u1.z, a2, u2.z, a3, u3.z, a4, u4.z);
        }

        public double getNorm()
        {
            return FastMath.sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }

        public double getNorm1()
        {
            return FastMath.abs(this.x) + FastMath.abs(this.y) + FastMath.abs(this.z);
        }

        public double getNormSq()
        {
            return this.x * this.x + this.y * this.y + this.z * this.z;
        }

        public double getNormInf()
        {
            return FastMath.max(FastMath.max(FastMath.abs(this.x), FastMath.abs(this.y)), FastMath.abs(this.z));
        }

        public double[] toArray()
        {
            return new double[] { this.x, this.y, this.z };
        }


        public double getAlpha()
        {
            return FastMath.atan2(this.y, this.x);
        }

        public double getDelta()
        {
            return FastMath.asin(this.z / this.getNorm());
        }

        public Vector3D add(Vector3D v)
        {
            Vector3D v3 = (Vector3D)v;
            return new Vector3D(this.x + v3.x, this.y + v3.y, this.z + v3.z);
        }

        public Vector3D add(double factor, Vector3D v)
        {
            return new Vector3D(1.0D, this, factor, (Vector3D)v);
        }

        public Vector3D subtract(Vector3D v)
        {
            Vector3D v3 = (Vector3D)v;
            return new Vector3D(this.x - v3.x, this.y - v3.y, this.z - v3.z);
        }

        public Vector3D subtract(double factor, Vector3D v)
        {
            return new Vector3D(1.0D, this, -factor, (Vector3D)v);
        }


        public Vector3D normalize() 
            // throws MathArithmeticException
        {
                double s = this.getNorm();
                if (s == 0.0D) {
                    throw new System.InvalidOperationException("CANNOT_NORMALIZE_A_ZERO_NORM_VECTOR");
                } else {
                    return this.scalarMultiply(1.0D / s);
            }
        }

        public Vector3D orthogonal()
            //throws MathArithmeticException
        {
            double threshold = 0.6D * this.getNorm();
            if (threshold == 0.0D)
            {
                throw new System.InvalidOperationException(" MathArithmeticException ZERO_NORM");
            }
            else
            {
                double inverse;
                if (FastMath.abs(this.x) <= threshold)
                {
                    inverse = 1.0D / FastMath.sqrt(this.y * this.y + this.z * this.z);
                    return new Vector3D(0.0D, inverse * this.z, -inverse * this.y);
                }
                else if (FastMath.abs(this.y) <= threshold)
                {
                    inverse = 1.0D / FastMath.sqrt(this.x * this.x + this.z * this.z);
                    return new Vector3D(-inverse * this.z, 0.0D, inverse * this.x);
                }
                else
                {
                    inverse = 1.0D / FastMath.sqrt(this.x * this.x + this.y * this.y);
                    return new Vector3D(inverse * this.y, -inverse * this.x, 0.0D);
                }
            }
        }


        public double dotProduct(Vector3D v)
        {
            Vector3D v3 = (Vector3D)v;
            return MathArrays.linearCombination(this.x, v3.x, this.y, v3.y, this.z, v3.z);
        }


        public Vector3D crossProduct(Vector3D v)
        {
            Vector3D v3 = (Vector3D)v;
            return new Vector3D(MathArrays.linearCombination(this.y, v3.z, -this.z, v3.y), MathArrays.linearCombination(this.z, v3.x, -this.x, v3.z), MathArrays.linearCombination(this.x, v3.y, -this.y, v3.x));
        }


        public double distance(Vector3D v)
        {
            Vector3D v3 = (Vector3D)v;
            double dx = v3.x - this.x;
            double dy = v3.y - this.y;
            double dz = v3.z - this.z;
            return FastMath.sqrt(dx * dx + dy * dy + dz * dz);
        }


        public double distance1(Vector3D v)
        {
            Vector3D v3 = (Vector3D)v;
            double dx = FastMath.abs(v3.x - this.x);
            double dy = FastMath.abs(v3.y - this.y);
            double dz = FastMath.abs(v3.z - this.z);
            return dx + dy + dz;
        }

        public double distanceInf(Vector3D v)
        {
            Vector3D v3 = (Vector3D)v;
            double dx = FastMath.abs(v3.x - this.x);
            double dy = FastMath.abs(v3.y - this.y);
            double dz = FastMath.abs(v3.z - this.z);
            return FastMath.max(FastMath.max(dx, dy), dz);
        }

        public double distanceSq(Vector3D v)
        {
            Vector3D v3 = (Vector3D)v;
            double dx = v3.x - this.x;
            double dy = v3.y - this.y;
            double dz = v3.z - this.z;
            return dx * dx + dy * dy + dz * dz;
        }


        public static double dotProduct(Vector3D v1, Vector3D v2)
        {
            return v1.dotProduct(v2);
        }


        public static Vector3D crossProduct(Vector3D v1, Vector3D v2)
        {
            return v1.crossProduct(v2);
        }

        public static double distance1(Vector3D v1, Vector3D v2)
        {
            return v1.distance1(v2);
        }

        public static double distance(Vector3D v1, Vector3D v2)
        {
            return v1.distance(v2);
        }

        public static double distanceInf(Vector3D v1, Vector3D v2)
        {
            return v1.distanceInf(v2);
        }

        public static double distanceSq(Vector3D v1, Vector3D v2)
        {
            return v1.distanceSq(v2);
        }

        public Vector3D negate()
        {
            return new Vector3D(-this.x, -this.y, -this.z);
        }

        public Vector3D scalarMultiply(double a)
        {
            return new Vector3D(a * this.x, a * this.y, a * this.z);
        }


        public Boolean isNaN()
        {
            return Double.IsNaN(this.x) || Double.IsNaN(this.y) || Double.IsNaN(this.z);
        }

        public bool equals(Object other)
        {
            if (this == other)
            {
                return true;
            }
            else if (other is Vector3D) {
                Vector3D rhs = (Vector3D)other;
                if (rhs.isNaN())
                {
                    return this.isNaN();
                }
                else
                {
                    return this.x == rhs.x && this.y == rhs.y && this.z == rhs.z;
                }
            } else
            {
                return false;
            }
        }

    }


    public class Plane // implements Hyperplane<Euclidean3D>, Embedding<Euclidean3D, Euclidean2D> 
    {
        private double originOffset;
        private Vector3D origin;
        private Vector3D u;
        private Vector3D v;
        private Vector3D w;


        public Plane(Vector3D normal)
        // throws MathArithmeticException
        {
            this.setNormal(normal);
            this.originOffset = 0.0D;
            this.setFrame();
        }

        public Plane(Vector3D p, Vector3D normal)
        // throws MathArithmeticException
        {
            this.setNormal(normal);
            this.originOffset = -p.dotProduct(this.w);
            this.setFrame();
        }


        private void setFrame()
        {
            this.origin = new Vector3D(-this.originOffset, this.w);
            this.u = this.w.orthogonal();
            this.v = Vector3D.crossProduct(this.w, this.u);
        }

        private void setNormal(Vector3D normal)
        // throws MathArithmeticException
        {
            double norm = normal.getNorm();
            if (norm < 1.0E-10D)
            {
                throw new System.InvalidOperationException("ZERO_NORM");
            }
            else
            {
                this.w = new Vector3D(1.0D / norm, normal);
            }


        }

        public double getOffset(Plane plane)
        {
            return this.originOffset + (this.sameOrientationAs(plane) ? -plane.originOffset : plane.originOffset);
        }

        public double getOffset(Vector3D point)
        {
            return point.dotProduct(this.w) + this.originOffset;
        }

        public bool sameOrientationAs(Plane other)
        {
            bool ret = other.w.dotProduct(this.w) > 0.0D;
            return ret;
        }

    }

}

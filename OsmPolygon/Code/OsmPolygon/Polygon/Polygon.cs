
using OpenToolkit.Mathematics;


namespace GeoApis
{


    class Polygon
    {

        public string DbUID;
        public string OsmId;


        public System.Collections.Generic.List<DecimalVector2> Points;
        public System.Collections.Generic.List<DecimalVector2> Vectors;
        

        public Polygon()
        {
            this.Points = new System.Collections.Generic.List<DecimalVector2>();
            this.Vectors = new System.Collections.Generic.List<DecimalVector2>();
        } // End Constructor 


        public Polygon(System.Collections.Generic.IEnumerable<LatLng> points)
            :this()
        {
            foreach (LatLng p in points)
            {
                this.Points.Add(new DecimalVector2(p.lat, p.lng));
            } // Next p 

            this.InitVectors();
        } // End Constructor 



        public Polygon(string points)
            :this()
        {
            string[] pointArray = points.Split(';');

            foreach (string pointPair in pointArray)
            {
                string[] coords = pointPair.Split(',');

                System.Decimal x = 0;
                System.Decimal y = 0;

                System.Decimal.TryParse(coords[0], out x);
                System.Decimal.TryParse(coords[1], out y);


                this.Points.Add(new OpenToolkit.Mathematics.DecimalVector2(x, y));
            } // Next p 

            this.InitVectors();
        }


        public void InitVectors()
        {
            for (int i = 0; i < this.Points.Count - 1; i++)
            {
                DecimalVector2 p1 = this.Points[i];
                DecimalVector2 p2 = this.Points[i + 1];

                this.Vectors.Add(DecimalVector2.Subtract(p2, p1));
            } // Next i 
        }


        public LatLng[] ToLatLngPoints()
        {
            LatLng[] ret = new LatLng[this.Points.Count];
            for (int i = 0; i < this.Points.Count; ++i)
            {
                ret[i] = new LatLng(this.Points[i].X, this.Points[i].Y);
            } // Next i 

            return ret;
        } // End Function ToLatLngPoints 


        /// <summary>
        /// MSSQL is CLOCKWISE  (MS-SQL wants the polygon points in CLOCKWISE sequence) 
        /// </summary>
        /// <returns>Returns the edge-points of the polygon, CLOCKWISE.</returns>
        public LatLng[] ToClockWiseLatLngPoints()
        {
            LatLng[] poly = ToLatLngPoints();

            if (!this.isClockwise)
                poly.Reverse();

            return poly;
        } // End Function ToClockWiseLatLngPoints 


        /// <summary>
        /// OSM is COUNTER-clockwise  (OSM wants the polygon points in COUNTERclockwise sequence) 
        /// </summary>
        /// <returns>Returns the edge-points of the polygon, COUNTER-clockwise.</returns>
        public LatLng[] ToCounterClockWiseLatLngPoints()
        {
            LatLng[] poly = ToLatLngPoints();

            if (this.isClockwise)
                poly.Reverse();

            return poly;
        } // End Function ToCounterClockWiseLatLngPoints 
        

        public decimal GetMinimumDistance(DecimalVector2 point)
        {
            decimal? distance = null;
            
            for (int i = 0; i < this.Vectors.Count; ++i)
            {
                DecimalVector2? intersection = DecimalVector2.GetPointVerticalIntersection(this.Points[i], this.Vectors[i], point);

                if (intersection.HasValue)
                {
                    if(!DecimalVector2.IsPointOnLine(this.Points[i], this.Points[i + 1], intersection.Value))
                        continue;

                    //decimal dist = DecimalVector2.DistanceOfPointToLine(point, this.Points[i], this.Points[i + 1]);
                    decimal dist = DecimalVector2.Subtract(point, intersection.Value).Length;

                    if (distance.HasValue)
                    {
                        if (dist < distance.Value)
                            distance = dist;
                    }
                    else
                        distance = dist;
                } // End if (intersection.HasValue) 

            } // Next i 


            // if (distance.HasValue) return distance.Value; return 1000000000000000;
            

            // System.Console.WriteLine(IsClosed);

            for (int i = 0; i < this.Points.Count; ++i)
            {
                decimal dist = DecimalVector2.Subtract(point, this.Points[i]).Length;

                if (distance.HasValue)
                {
                    if (dist < distance.Value)
                        distance = dist;
                }
                else
                    distance = dist;
            } // Next i 

            return distance.Value;
        } // End Function GetMinimumDistance 


        public void PopulateV1()
        {
            Points.Add(new DecimalVector2(1, 1));
            Points.Add(new DecimalVector2(5, 1));
            Points.Add(new DecimalVector2(5, 2));
            // Points.Add(new DecimalVector2(1, 2));
            // Points.Add(new DecimalVector2(1, 1));
        } // End Sub PopulateV1 


        public bool IsClosed
        {
            get
            {
                decimal x0 = this.Points[0].X;
                decimal y0 = this.Points[0].Y;

                decimal xN = this.Points[this.Points.Count - 1].X;
                decimal yN = this.Points[this.Points.Count - 1].Y;

                if (x0 == xN && y0 == yN)
                    return true;

                return false;
            }
        } // End Property IsClosed 
        

        public void Close()
        {
            Points.Add(new DecimalVector2(this.Points[0].X, this.Points[0].Y));
        }


        public void UnClose()
        {
            Points.RemoveAt(Points.Count - 1);
        }


        public void EnsureClosed()
        {
            if (!this.IsClosed)
                this.Close();
        }


        public void EnsureUnclosed()
        {
            if (this.IsClosed)
                UnClose();
        }


        public bool isClockwise
        {
            get
            {
                decimal sum = 0;

                for (int i = 0; i < this.Points.Count - 1; i++)
                {
                    DecimalVector2 cur = this.Points[i], next = this.Points[i + 1];
                    sum += (next.X - cur.X) * (next.Y + cur.Y);
                } // Next i 

                return sum > 0;
            }
        } // End Property isClockwise 


        public decimal MathematicalArea
        {
            get
            {
                decimal nArea = 0m;

                for (int i = 0; i < this.Points.Count; ++i)
                {
                    if (this.Points[i] != null)
                    {
                        if (i != this.Points.Count - 1)
                        {
                            nArea += Points[i].X * Points[i + 1].Y - Points[i + 1].X * Points[i].Y;
                        }
                        else
                        {
                            nArea += Points[i].X * Points[0].Y - Points[0].X * Points[i].Y;
                        }

                    } // End if(this.Points[i] != null)

                } // Next point

                nArea *= 0.5m;
                return nArea;
            }
        } // End Property MathematicalArea 


        public decimal Area
        {
            get
            {
                return System.Math.Abs(this.MathematicalArea);
            }
        } // End Property Area 


        // Geschlossen
        public DecimalVector2 Centroid
        {
            get
            {
                decimal accumulatedArea = 0.0m;
                decimal centerX = 0.0m;
                decimal centerY = 0.0m;

                bool wasClosed = this.IsClosed;
                this.EnsureClosed();

                for (int i = 0, j = this.Points.Count - 1; i < this.Points.Count; j = i++)
                {
                    decimal temp = this.Points[i].X * this.Points[j].Y - this.Points[j].X * this.Points[i].Y;
                    accumulatedArea += temp;
                    centerX += (this.Points[i].X + this.Points[j].X) * temp;
                    centerY += (this.Points[i].Y + this.Points[j].Y) * temp;
                } // Next i 

                if (!wasClosed)
                    this.EnsureUnclosed();
                
                if (System.Math.Abs(accumulatedArea) < 1E-7m)
                    return new DecimalVector2();  // Avoid division by zero

                accumulatedArea *= 3m;
                return new DecimalVector2(centerX / accumulatedArea, centerY / accumulatedArea);
            }

        } // End Property Centroid 


        // Nicht geschlossen
        public DecimalVector2 Midpoint
        {
            get
            {
                decimal x = 0m;
                decimal y = 0m;

                bool wasClosed = this.IsClosed;
                this.EnsureUnclosed();

                for (int i = 0; i < this.Points.Count; ++i)
                {
                    x += this.Points[i].X;
                    y += this.Points[i].Y;
                } // Next i 

                x /= (this.Points.Count);
                y /= (this.Points.Count);

                if (wasClosed)
                    this.EnsureClosed();

                return new DecimalVector2(x, y);
            }

        } // End Property MidPoint 


    } // End Class Polygon 


} // End Namespace 

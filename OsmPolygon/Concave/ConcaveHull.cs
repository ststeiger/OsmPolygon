
using System.Linq;
using System.Collections.Generic;

using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.Triangulate;
using NetTopologySuite.Triangulate.QuadEdge;


namespace NetTopologySuite.Hull
{


    // https://towardsdatascience.com/the-concave-hull-c649795c0f0f
    // https://github.com/matsim-up/freight-sa/blob/master/src/main/java/org/matsim/up/freight/clustering/containers/ConcaveHull.java
    // http://www.rotefabrik.free.fr/concave_hull/
    public class ConcaveHull
    {

        private GeometryFactory geomFactory;
        private readonly GeometryCollection geometries;
        private double threshold;
        
        
        public Dictionary<LineSegment, int> segments = new Dictionary<LineSegment, int>();
        public Dictionary<int, Edge> edges = new Dictionary<int, Edge>();
        public Dictionary<int, Triangle> triangles = new Dictionary<int, Triangle>();
        public SortedDictionary<int, Edge> lengths = new SortedDictionary<int, Edge>();

        public Dictionary<int, Edge> shortLengths = new Dictionary<int, Edge>();

        public Dictionary<Coordinate, int> coordinates = new Dictionary<Coordinate, int>();
        public Dictionary<int, Vertex> vertices = new Dictionary<int, Vertex>();
        


        public ConcaveHull(Geometry geometry, double threshold)
        {
            this.geometries = transformIntoPointGeometryCollection(geometry);
            this.threshold = threshold;
            this.geomFactory = geometry.Factory;
        }


        public ConcaveHull(GeometryCollection geometries, double threshold)
        {
            this.geometries = transformIntoPointGeometryCollection(geometries);
            this.threshold = threshold;
            this.geomFactory = geometries.Factory;
        }

        private static GeometryCollection transformIntoPointGeometryCollection(Geometry geom)
        {
            NetTopologySuite.Utilities.UniqueCoordinateArrayFilter filter = new NetTopologySuite.Utilities.UniqueCoordinateArrayFilter();
            geom.Apply(filter);
            Coordinate[] coord = filter.Coordinates;

            Geometry[] geometries = new Geometry[coord.Length];
            for (int i = 0; i < coord.Length; i++)
            {
                Coordinate[] c = new Coordinate[] { coord[i] };
                CoordinateArraySequence cs = new CoordinateArraySequence(c);
                geometries[i] = new Point(cs, geom.Factory);
            }

            return new GeometryCollection(geometries, geom.Factory);
        }

        // Transform into GeometryCollection. 
	    // @param geom input geometry
	    // @return a geometry collection
        private static GeometryCollection transformIntoPointGeometryCollection(GeometryCollection gc)
        {
            NetTopologySuite.Utilities.UniqueCoordinateArrayFilter filter = new NetTopologySuite.Utilities.UniqueCoordinateArrayFilter();
            gc.Apply(filter);
            Coordinate[] coord = filter.Coordinates;

            Geometry[] geometries = new Geometry[coord.Length];
            for (int i = 0; i < coord.Length; i++)
            {
                Coordinate[] c = new Coordinate[] { coord[i] };
                CoordinateArraySequence cs = new CoordinateArraySequence(c);
                geometries[i] = new Point(cs, gc.Factory);
            }

            return new GeometryCollection(geometries, gc.Factory);
        }


        public Geometry GetResult()
        {
            QuadEdgeSubdivision qes = BuildDelaunay();
            IList<QuadEdge> quadEdges = qes.GetEdges();
            IList<QuadEdgeTriangle> qeTriangles = QuadEdgeTriangle.CreateOn(qes);
            IEnumerable<Vertex> qeVertices = qes.GetVertices(false);

            int iV = 0;
            foreach (Vertex v in qeVertices)
            {
                this.coordinates[v.Coordinate] = iV;
                this.vertices[iV] = new Vertex(iV, v.Coordinate); 
                iV++;
            }
            
            List<QuadEdge> qeFrameBorder = new List<QuadEdge>();
            List<QuadEdge> qeFrame = new List<QuadEdge>();
            List<QuadEdge> qeBorder = new List<QuadEdge>();
            
            foreach (QuadEdge qe in quadEdges)
            {
                if (qes.IsFrameBorderEdge(qe))
                {
                    qeFrameBorder.Add(qe);
                }

                if (qes.IsFrameEdge(qe))
                {
                    qeFrame.Add(qe);
                }
            } // Next qe 

            // border
            for (int j = 0; j < qeFrameBorder.Count; j++)
            {
                QuadEdge q = qeFrameBorder[j];
                if (!qeFrame.Contains(q))
                {
                    qeBorder.Add(q);
                }

            } // Next j 

            // deletion of exterior edges
            foreach (QuadEdge qe in qeFrame)
            {
                qes.Delete(qe);
            }


            Dictionary<QuadEdge, double> qeDistances = new Dictionary<QuadEdge, double>();
            foreach (QuadEdge qe in quadEdges)
            {
                qeDistances[qe] = qe.ToLineSegment().Length;
            }

            QuadEdgeComparer dc = new QuadEdgeComparer();
            //SortedDictionary<QuadEdge, double> qeSorted = new SortedDictionary<QuadEdge, double>(qeDistances, dc);
            SortedDictionary<QuadEdge, double> qeSorted = new SortedDictionary<QuadEdge, double>(dc);
            foreach (KeyValuePair<QuadEdge, double> thisDistance in qeDistances)
            {
                // qeSorted.Add(x.Key, x.Value);
                qeSorted[thisDistance.Key] = thisDistance.Value;
            }
            
            // edges creation
            int i = 0;
            foreach (QuadEdge qe in qeSorted.Keys)
            {
                LineSegment s = qe.ToLineSegment();
                s.Normalize();

                int idS = this.coordinates[s.P0];
                int idD = this.coordinates[s.P1];
                Vertex oV = this.vertices[idS];
                Vertex eV = this.vertices[idD];

                Edge edge;
                if (qeBorder.Contains(qe))
                {
                    oV.IsBorder = true;
                    eV.IsBorder = true;
                    
                    edge = new Edge(i, s, oV, eV, true);

                    if (s.Length < this.threshold)
                    {
                        this.shortLengths[i] = edge;
                    }
                    else
                    {
                        this.lengths[i] = edge;
                    }
                }
                else
                {
                    edge = new Edge(i, s, oV, eV, false);
                }

                this.edges[i] = edge;
                this.segments[s] = i;
                i++;
            } // Next qe 

            // hm of linesegment and hm of edges // with id as key
            // hm of triangles using hm of ls and connection with hm of edges

            i = 0;
            foreach (QuadEdgeTriangle qet in qeTriangles)
            {
                LineSegment sA = qet.GetEdge(0).ToLineSegment();
                LineSegment sB = qet.GetEdge(1).ToLineSegment();
                LineSegment sC = qet.GetEdge(2).ToLineSegment();

                sA.Normalize();
                sB.Normalize();
                sC.Normalize();

                Edge edgeA = this.edges[this.segments[sA]];
                Edge edgeB = this.edges[this.segments[sB]];
                Edge edgeC = this.edges[this.segments[sC]];

                Triangle triangle = new Triangle(i, qet.IsBorder() ? true : false);
                triangle.AddEdge(edgeA);
                triangle.AddEdge(edgeB);
                triangle.AddEdge(edgeC);

                edgeA.AddTriangle(triangle);
                edgeB.AddTriangle(triangle);
                edgeC.AddTriangle(triangle);

                this.triangles[i] = triangle;
                i++;
            } // Next qet 


            // add triangle neighbourood
            foreach (Edge edge in this.edges.Values)
            {
                if (edge.Triangles.Count != 1)
                {
                    Triangle tA = edge.Triangles[0];
                    Triangle tB = edge.Triangles[1];
                    tA.AddNeighbour(tB);
                    tB.AddNeighbour(tA);
                }
            }

            // concave hull algorithm
            int index = 0;
            while (index != -1)
            {
                index = -1;

                Edge e = null;

                // find the max length (smallest id so first entry)
                int si = this.lengths.Count;

                if (si != 0)
                {
                    KeyValuePair<int, Edge> entry = this.lengths.First();

                    int ind = entry.Key;
                    if (entry.Value.Geometry.Length > this.threshold)
                    {
                        index = ind;
                        e = entry.Value;
                    }

                } // End if (si != 0) 

                if (index != -1)
                {
                    Triangle triangle = e.Triangles[0];
                    List<Triangle> neighbours = triangle.Neighbours;

                    // irregular triangle test
                    if (neighbours.Count == 1)
                    {
                        this.shortLengths[e.Id] = e;
                        this.lengths.Remove(e.Id);
                    }
                    else
                    {
                        Edge e0 = triangle.Edges[0];
                        Edge e1 = triangle.Edges[1];

                        // test if all the vertices are on the border
                        if (e0.OV.IsBorder && e0.EV.IsBorder
                            && e1.OV.IsBorder && e1.EV.IsBorder)
                        {
                            this.shortLengths[e.Id] = e;
                            this.lengths.Remove(e.Id);
                        }
                        else
                        {
                            // management of triangles
                            Triangle tA = neighbours[0];
                            Triangle tB = neighbours[1];
                            tA.Border = true; // FIXME not necessarily useful
                            tB.Border = true; // FIXME not necessarily useful
                            this.triangles.Remove(triangle.Id);
                            tA.RemoveNeighbour(triangle);
                            tB.RemoveNeighbour(triangle);
                            
                            // new edges
                            List<Edge> ee = triangle.Edges;
                            Edge eA = ee[0];
                            Edge eB = ee[1];
                            Edge eC = ee[2];

                            if (eA.Border)
                            {
                                this.edges.Remove(eA.Id);
                                eB.Border = true;

                                eB.OV.IsBorder = true;
                                eB.EV.IsBorder = true;
                                
                                eC.Border = true;

                                eC.OV.IsBorder = true;
                                eC.EV.IsBorder = true;

                                // clean the relationships with the triangle
                                eB.RemoveTriangle(triangle);
                                eC.RemoveTriangle(triangle);

                                if (eB.Geometry.Length < this.threshold)
                                {
                                    this.shortLengths[eB.Id] = eB;
                                }
                                else
                                {
                                    this.lengths[eB.Id] = eB;
                                }

                                if (eC.Geometry.Length < this.threshold)
                                {
                                    this.shortLengths[eC.Id] = eC;
                                }
                                else
                                {
                                    this.lengths[eC.Id] = eC;
                                }

                                this.lengths.Remove(eA.Id);
                            } // End if (eA.Border) 
                            else if (eB.Border)
                            {
                                this.edges.Remove(eB.Id);
                                eA.Border = true;
                                eA.OV.IsBorder = true;
                                eA.EV.IsBorder = true;
                                eC.Border = true;
                                eC.OV.IsBorder = true;
                                eC.EV.IsBorder = true;

                                // clean the relationships with the triangle
                                eA.RemoveTriangle(triangle);
                                eC.RemoveTriangle(triangle);

                                if (eA.Geometry.Length < this.threshold)
                                {
                                    this.shortLengths[eA.Id] = eA;
                                }
                                else
                                {
                                    this.lengths[eA.Id] = eA;
                                }
                                if (eC.Geometry.Length < this.threshold)
                                {
                                    this.shortLengths[eC.Id] = eC;
                                }
                                else
                                {
                                    this.lengths[eC.Id] = eC;
                                }

                                this.lengths.Remove(eB.Id);

                            } // End else if (eB.Border)
                            else
                            {
                                this.edges.Remove(eC.Id);
                                eA.Border = true; 

                                eA.OV.IsBorder = true;
                                eA.EV.IsBorder = true;
                                eB.Border = true;
                                eB.OV.IsBorder = true;
                                eB.EV.IsBorder = true;
                                
                                // clean the relationships with the triangle
                                eA.RemoveTriangle(triangle);
                                eB.RemoveTriangle(triangle);

                                if (eA.Geometry.Length < this.threshold)
                                {
                                    this.shortLengths[eA.Id] = eA;
                                }
                                else
                                {
                                    this.lengths[eA.Id] = eA;
                                }

                                if (eB.Geometry.Length < this.threshold)
                                {
                                    this.shortLengths[eB.Id] = eB;
                                }
                                else
                                {
                                    this.lengths[eB.Id] = eB;
                                }

                                this.lengths.Remove(eC.Id);
                            } // End Else of if (e0.OV.Border && e0.EV.Border && e1.OV.Border && e1.EV.Border)
                            
                        } // End Else of if 
                        
                    } // End Else of if (neighbours.Count == 1)
                    
                } // End if (index != -1) 

            } // Whend 

            // concave hull creation
            List<LineString> edges = new List<LineString>();
            foreach (Edge e in this.lengths.Values)
            {
                LineString l = e.Geometry.ToGeometry(this.geomFactory);
                edges.Add(l);
            }

            foreach (Edge e in this.shortLengths.Values)
            {
                LineString l = e.Geometry.ToGeometry(this.geomFactory);
                edges.Add(l);
            }

            // merge
            NetTopologySuite.Operation.Linemerge.LineMerger lineMerger = new NetTopologySuite.Operation.Linemerge.LineMerger();
            lineMerger.Add(edges);
            
            LineString merge = null;
            
            using (IEnumerator<Geometry> en = lineMerger.GetMergedLineStrings().GetEnumerator())
            {
                en.MoveNext();
                merge = (LineString)en.Current;
            }
            
            if (merge.IsRing)
            {
                LinearRing lr = new LinearRing(merge.CoordinateSequence, this.geomFactory);
                Polygon concaveHull = new Polygon(lr, null, this.geomFactory);
                return concaveHull;
            }
            
            return merge;
        }
        
        
        private QuadEdgeSubdivision BuildDelaunay()
        {
            DelaunayTriangulationBuilder builder = new DelaunayTriangulationBuilder();
            builder.SetSites(this.geometries);
            QuadEdgeSubdivision subDiv = builder.GetSubdivision();
            return subDiv;
        }
        
        
    }
    
    
}
